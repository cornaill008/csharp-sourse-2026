using System;
using System.Collections.Generic;
using System.Linq;
using App.Data.Dto;
using App.Domain;

namespace App.Data.Mapping
{
    public static class ImageItemMapper
    {
        public static ImageItem ToModel(PixabayHitDto dto)
        {
            return new ImageItem(
                id: dto.id.ToString(),
                thumbnailUrl: dto.webformatURL,
                tags: ParseTags(dto.tags),
                authorName: dto.user);
        }

        // "a, b, c" -> ["a", "b", "c"]; trims whitespace and drops empty entries.
        public static IReadOnlyList<string> ParseTags(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return Array.Empty<string>();
            }

            return raw
                .Split(',')
                .Select(tag => tag.Trim())
                .Where(tag => tag.Length > 0)
                .ToList();
        }
    }
}
