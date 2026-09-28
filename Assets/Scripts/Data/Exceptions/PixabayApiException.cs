using System;

namespace App.Data.Exceptions
{
    public sealed class PixabayApiException : Exception
    {
        public PixabayApiException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; }
    }
}
