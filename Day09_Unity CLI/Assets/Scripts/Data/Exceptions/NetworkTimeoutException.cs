using System;

namespace App.Data.Exceptions
{
    public sealed class NetworkTimeoutException : Exception
    {
        public NetworkTimeoutException(string message) : base(message)
        {
        }
    }
}
