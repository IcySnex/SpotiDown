using Newtonsoft.Json;
using System.Collections.Generic;

namespace SpotiDown.Models;

public class Genius
{
    public Genius_Response response { get; set; } = new();
}

public class Genius_Response
{
    public List<Genius_Response_Hit> hits { get; set; } = new();
}

public class Genius_Response_Hit
{
    public Genius_Response_Hit_Result result { get; set; } = new();
}

public class Genius_Response_Hit_Result
{
    [JsonProperty(PropertyName = "url")]
    public string Url { get; set; } = "";
    [JsonProperty(PropertyName = "artist_names")]
    public string Artist { get; set; } = "";
    [JsonProperty(PropertyName = "title")]
    public string Title { get; set; } = "";
    [JsonProperty(PropertyName = "song_art_image_thumbnail_url")]
    public string Artwork { get; set; } = "";
}