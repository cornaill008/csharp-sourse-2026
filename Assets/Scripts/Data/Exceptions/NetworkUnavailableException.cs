using System;

namespace App.Data.Exceptions
{
    public sealed class NetworkUnavailableException : Exception
    {
        public NetworkUnavailableException(string message) : base(message)
        {
        }
    }
}
