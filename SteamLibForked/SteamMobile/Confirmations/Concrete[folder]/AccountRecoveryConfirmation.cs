﻿namespace SteamLib.SteamMobile.Confirmations;

public class AccountRecoveryConfirmation : Confirmation
{
    public AccountRecoveryConfirmation(long id, ulong nonce, long creator, string typeName) : base(id, nonce, 6, creator, ConfirmationType.AccountRecovery, typeName)
    {
    }
}