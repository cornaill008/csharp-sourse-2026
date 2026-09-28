namespace App.Tests.EditMode.Fixtures
{
    public static class PixabayJsonFixtures
    {
        public const string TwoHits = @"{
  ""totalHits"": 2,
  ""total"": 2,
  ""hits"": [
    { ""id"": 1, ""tags"": ""cat, animal, pet"", ""previewURL"": ""https://example.com/p/1.jpg"",
      ""webformatURL"": ""https://example.com/w/1.jpg"", ""user"": ""alice"", ""likes"": 10, ""views"": 100 },
    { ""id"": 2, ""tags"": ""dog, animal"", ""previewURL"": ""https://example.com/p/2.jpg"",
      ""webformatURL"": ""https://example.com/w/2.jpg"", ""user"": ""bob"", ""likes"": 5, ""views"": 50 }
  ]
}";

        public const string NoHits = @"{ ""totalHits"": 0, ""total"": 0, ""hits"": [] }";

        public const string EmptyTags = @"{
  ""totalHits"": 1,
  ""total"": 1,
  ""hits"": [
    { ""id"": 3, ""tags"": """", ""previewURL"": ""https://example.com/p/3.jpg"",
      ""webformatURL"": ""https://example.com/w/3.jpg"", ""user"": ""carol"", ""likes"": 0, ""views"": 0 }
  ]
}";
    }
}
