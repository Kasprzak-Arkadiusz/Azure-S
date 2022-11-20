using System.Collections.Generic;
using Newtonsoft.Json;

namespace backend.Contracts;

public class TagImageResponse
{
    [JsonProperty("tags")] public List<Tag> Tags { get; set; }
    [JsonProperty("requestId")] public string RequestId { get; set; }
    [JsonProperty("metadata")] public Metadata Metadata { get; set; }
}

public class Tag
{
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("confidence")] public decimal Confidence { get; set; }
}

public class Metadata
{
    [JsonProperty("width")] public int Width { get; set; }
    [JsonProperty("height")] public int Height { get; set; }
    [JsonProperty("format")] public string Format { get; set; }
}