namespace App.Data.Dto
{
    // Field names mirror the Pixabay API response verbatim (see Docs/diagram.puml).
    public sealed class PixabayHitDto
    {
        public long id { get; set; }
        public string tags { get; set; }
        public string previewURL { get; set; }
        public string webformatURL { get; set; }
        public string user { get; set; }
        public int likes { get; set; }
        public int views { get; set; }
    }
}
