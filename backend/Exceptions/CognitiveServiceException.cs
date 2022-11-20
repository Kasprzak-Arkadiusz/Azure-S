using System;

namespace backend.Exceptions;

public class CognitiveServiceException : Exception
{
    public CognitiveServiceException(string message) : base(message) { }
}