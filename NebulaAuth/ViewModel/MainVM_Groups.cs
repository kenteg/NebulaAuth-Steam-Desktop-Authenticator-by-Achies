﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NebulaAuth.Model;
using NebulaAuth.Model.Entities;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace NebulaAuth.ViewModel;

public partial class MainVM //Groups
{
    [ObservableProperty]
    private ObservableCollection<string> _groups = new();


    public string? SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            if (SetProperty(ref _selectedGroup, value))
                PerformSearch();
        }
    }

    private string? _selectedGroup;

    public string SearchText
    {
        get => _searchText;
        set
        {
            if(SetProperty(ref _searchText, value))
                PerformSearch();
        }

    }
    private string _searchText = string.Empty;




    [RelayCommand]
    private void AddGroup(string? value)
    {
        var mafile = SelectedMafile;
        if (mafile == null) return;
        if (string.IsNullOrEmpty(value)) return;
        mafile.Group = value;
        Storage.UpdateMafile(mafile);
        QueryGroups();
        SelectedGroup = value;
        OnPropertyChanged(nameof(SelectedMafile)); //For bindings

    }

    [RelayCommand]
    private void AddToGroup(object[]? value)
    {
        if (value == null) return;
        if (value.Length < 2) return;
        var group = (string?)value[0];
        var mafile = (Mafile?)value[1];

        if (group == null || mafile == null) return;
        mafile.Group = group;
        Storage.UpdateMafile(mafile);
        QueryGroups();
        PerformSearch();
        OnPropertyChanged(nameof(SelectedMafile));
    }

    [RelayCommand]
    private void RemoveGroup(Mafile? mafile)
    {
        if (mafile?.Group == null) return;
        var mafGroup = mafile.Group;
        mafile.Group = null;
        Storage.UpdateMafile(mafile);
        OnPropertyChanged(nameof(SelectedMafile)); //For bindings
        QueryGroups();
        if (Groups.All(g => g.Equals(mafGroup) == false))
        {
            SelectedGroup = null;
        }
        PerformSearch();
    }


    private void QueryGroups()
    {
        var groups = Storage.MaFiles
            .Where(m => string.IsNullOrWhiteSpace(m.Group) == false)
            .Select(m => m.Group)
            .Distinct()
            .Order()
            .ToList();

        Groups = new ObservableCollection<string>(groups!);
    }
    private void PerformSearch()
    {
        if (string.IsNullOrWhiteSpace(SelectedGroup) && string.IsNullOrWhiteSpace(SearchText))
        {
            MaFiles = Storage.MaFiles;
            return;
        }
        long? searchSteamId = null;
        if (long.TryParse(SearchText, out var steamId))
        {
            searchSteamId = steamId;
        }
        var query = Storage.MaFiles.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(SearchPredicate);
        }
        if (!string.IsNullOrWhiteSpace(SelectedGroup))
        {
            query = query.Where(m => m.Group != null && m.Group.Equals(SelectedGroup));
        }
        var perform = query.ToList();
        MaFiles = new ObservableCollection<Mafile>(perform);
        if (SelectedMafile != null && !MaFiles.Contains(SelectedMafile))
        {
            SelectedMafile = MaFiles.FirstOrDefault();
        }
        bool SearchPredicate(Mafile mafile)
        {
            if (!mafile.AccountName.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase))
            {
                return mafile.SessionData != null && mafile.SessionData.SteamId.Steam64.Id.Equals(searchSteamId);
            }
            return true;
        }
    }

}