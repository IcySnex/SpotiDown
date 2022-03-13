using HtmlAgilityPack;
using SpotiDown.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SpotiDown.Helpers;

public class Song
{
    public async Task<List<Genius_Response_Hit>> SearchLyrics(string Title, string Artist) =>
        Text.Deserialize<Genius>(await Local.DownloadString($"https://api.genius.com/search?q={Title} {Artist}&access_token=7lBNcWe_5ljK10TnPh7q3j31clowVoGLN9IEYFbO9AcKsngZ49Iz7aeZ0D9W4H1j")) is Genius Search ? Search.response.hits : new();
    
    public async Task<string> GetLyrics(Genius_Response_Hit_Result Song)
    {
        var Html = new HtmlDocument();
        Html.LoadHtml((await Local.DownloadString(Song.Url)).Replace("https://ajax.googleapis.com/ajax/libs/jquery/2.1.4/jquery.min.js", ""));

        var Clickables = Html.DocumentNode.SelectNodes("//a[contains(@class, 'ReferentFragmentVariantdesktop')]");
        if (Clickables != null)
            foreach (HtmlNode Node in Clickables)
                Node.ParentNode.ReplaceChild(Html.CreateTextNode(Node.ChildNodes[0].InnerHtml), Node);
        var Spans = Html.DocumentNode.SelectNodes("//span");
        if (Spans != null)
            foreach (HtmlNode Node in Spans)
                Node.Remove();

        var Nodes = Html.DocumentNode.SelectNodes("//div[@data-lyrics-container]");
        return WebUtility.HtmlDecode(string.Join("", Nodes.Select(Node => Node.InnerHtml)).Replace("<br>", "\n"));
    }
}