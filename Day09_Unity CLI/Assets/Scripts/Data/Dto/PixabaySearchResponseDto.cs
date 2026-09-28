using System.Collections.Generic;

namespace App.Data.Dto
{
    public sealed class PixabaySearchResponseDto
    {
        public int totalHits { get; set; }
        public int total { get; set; }
        public List<PixabayHitDto> hits { get; set; }
    }
}
