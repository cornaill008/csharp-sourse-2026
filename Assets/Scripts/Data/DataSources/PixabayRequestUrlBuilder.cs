using System;

namespace App.Data.DataSources
{
    // Pure/testable: no network access, just string construction.
    public static class PixabayRequestUrlBuilder
    {
        public const int PerPage = 20;

        private const string BaseUrl = "https://pixabay.com/api/";

        public static string Build(string apiKey, string keyword, int page)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException("apiKey must not be null or empty.", nameof(apiKey));
            }

            return $"{BaseUrl}?key={Uri.EscapeDataString(apiKey)}&q={Uri.EscapeDataString(keyword)}&page={page}&per_page={PerPage}&image_type=photo";
        }
    }
}
