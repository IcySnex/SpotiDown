Imports System.IO
Imports System.Net
Imports System.Text.RegularExpressions
Imports HtmlAgilityPack
Imports Newtonsoft.Json
Imports YoutubeExplode

Public Class YoutubeHelper
    Public Shared client As New YoutubeClient()

    Public Shared Async Function GetId(search As String) As Task(Of String)
        Return Regex.Matches(Await Helper.DownloadHtmlUTF8($"https://www.youtube.com/results?search_query={search}"), "v=([a-zA-Z0-9-_]{11})")(0).Groups(1).Value
    End Function

    Public Shared Async Function GetLyrics(search As String) As Task(Of String)
        Try
            Dim url = JsonConvert.DeserializeObject(Of lyrics_search)(Await Helper.DownloadHtmlUTF8($"https://api.genius.com/search?q={search}&access_token=mfXhXfTm2AuMSZPztd8a6eLAX-8nTdzGmMJd_2N3yjmJHeax3TGOqrTW4FLfqwq9")).response.hits(0).result.url
            Dim html As New HtmlDocument
            html.LoadHtml((Await Helper.DownloadHtmlUTF8(url)).Replace("https://ajax.googleapis.com/ajax/libs/jquery/2.1.4/jquery.min.js", ""))
            Dim lyrics As String
            If html.DocumentNode.Descendants("div").Any(Function(d) d.Id = "lyrics-root-pin-spacer") Then
                For Each node As HtmlNode In html.DocumentNode.SelectNodes("//br")
                    node.ParentNode.ReplaceChild(html.CreateTextNode(vbNewLine), node)
                Next
                lyrics = String.Join("", html.GetElementbyId("lyrics-root-pin-spacer").ChildNodes(0).ChildNodes.Select(Function(n) n.InnerText)).Replace("EmbedShare URLCopyEmbedCopy", "")
            Else
                lyrics = String.Join("", html.DocumentNode.SelectNodes("//div[@class='lyrics']//*").Select(Function(n) n.InnerText)).Replace("EmbedShare URLCopyEmbedCopy", "")
            End If
            Return $"// LYRICS LOADED BY SPOTIDOWN{vbCrLf}// {url}{vbCrLf}{vbCrLf}{WebUtility.HtmlDecode(lyrics)}"
        Catch ex As Exception
            Return $"Erorr fetching lyrics from 'genius.com'{vbCrLf}Error message: *{ex.Message}*"
        End Try
    End Function

    Public Shared Async Function DownloadAudioStream(id As String) As Task(Of Stream)
        Dim avaiavles = (Await client.Videos.Streams.GetManifestAsync(id)).GetAudioOnlyStreams
        Dim diffList = avaiavles.Select(Function(x) New With {.n = x.Bitrate.KiloBitsPerSecond, .diff = Math.Abs(x.Bitrate.KiloBitsPerSecond - GetKbpsConfig())})
        Dim info = avaiavles.Where(Function(b) b.Bitrate.KiloBitsPerSecond = diffList.Where(Function(x) x.diff = diffList.Select(Function(y) y.diff).Min).First.n)(0)
        Return Await client.Videos.Streams.GetAsync(info)
    End Function

    Public Shared Function GetKbpsConfig() As Integer
        If Helper.config.prefernces.quality = 0 Then
            Return 32
        ElseIf Helper.config.prefernces.quality = 1 Then
            Return 64
        ElseIf Helper.config.prefernces.quality = 2 Then
            Return 128
        ElseIf Helper.config.prefernces.quality = 3 Then
            Return 160
        ElseIf Helper.config.prefernces.quality = 4 Then
            Return 256
        ElseIf Helper.config.prefernces.quality = 5 Then
            Return 320
        Else
            Return 160
        End If
    End Function

    Public Shared Async Function WriteTrack(trackinfo As SpotifyTrack) As Task(Of String)
        Dim filepath = Helper.config.prefernces.downloadpath & Helper.config.prefernces.filename.Replace("{title}", trackinfo.title).Replace("{artist}", trackinfo.artist.Split(",")(0)).Replace("{album}", trackinfo.album).Replace("{release}", trackinfo.release.Year) & ".mp3"
        If Directory.Exists(Path.GetDirectoryName(filepath)) = False Then Directory.CreateDirectory(Path.GetDirectoryName(filepath))
        If Await Helper.writeStream(filepath, Await DownloadAudioStream(trackinfo.youtube)) Then Await Helper.writeMetadata(filepath, trackinfo)
        Return filepath
    End Function
End Class

#Region "class"
Public Class lyrics_search
    Public Property response As New lyrics_search_response
End Class
Public Class lyrics_search_response
    Public Property hits As New List(Of lyrics_search_response_hits)
End Class
Public Class lyrics_search_response_hits
    Public Property result As New lyrics_search_response_hits_result
End Class
Public Class lyrics_search_response_hits_result
    Public Property url As String
End Class
#End Region