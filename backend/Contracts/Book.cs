using Newtonsoft.Json;

namespace backend.Contracts;

public class Book
{
    [JsonProperty("cover")]
    public byte[] Cover { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("author")]
    public string Author { get; set; }
}