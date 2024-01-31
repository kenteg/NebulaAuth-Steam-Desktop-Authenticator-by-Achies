﻿namespace SteamLib.SteamMobile.Confirmations;

public class RegisterApiKeyConfirmation : Confirmation
{
    public RegisterApiKeyConfirmation(long id, ulong nonce, long creatorId, string typeName) : base(id, nonce, 8, creatorId, ConfirmationType.RegisterApiKey, typeName)
    {
    }
}