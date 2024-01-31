﻿using System.Runtime.Serialization;

namespace SteamLib.ProtoCore.Exceptions;

[Serializable]
public class UnknownEResultException : Exception
{
    public int EResult { get; }
    public UnknownEResultException()
    {}

    public UnknownEResultException(int eResult) : base("Got unknown EResult: " + eResult)
    {
        EResult = eResult;
    }
    
    protected UnknownEResultException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}