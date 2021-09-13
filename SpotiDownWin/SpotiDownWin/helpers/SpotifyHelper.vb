Imports System.Text.RegularExpressions
Imports System.Threading
Imports SpotifyAPI.Web

Public Class SpotifyHelper
    Public Shared client As SpotifyClient
    Public Shared NotValidSpotifyUrl As New Exception("Input Url is not a valid Spotify Url.")

    Public Shared Async Function ConnectToSpotify() As Task(Of SpotifyClient)
        Return New SpotifyClient((Await New OAuthClient().RequestToken(New ClientCredentialsRequest(Helper.config.oauth.id, Helper.config.oauth.secret))).AccessToken)
    End Function

    Public Shared Function IsValidUrl(url As String) As Boolean
        If String.IsNullOrWhiteSpace(url) Then Return False
        Return Regex.IsMatch(url, "http(s)?\:\/\/open\.spotify\.com\/(track|playlist|album)\/.+$")
    End Function

    Public Shared Function GetSpotifyUrlType(url As String) As SpotifyUrlType
        If IsValidUrl(url) = False Then
            Throw NotValidSpotifyUrl
        ElseIf url.ToLower.Contains("track") Then
            Return SpotifyUrlType.Track
        ElseIf url.ToLower.Contains("playlist") Then
            Return SpotifyUrlType.Playlist
        ElseIf url.ToLower.Contains("album") Then
            Return SpotifyUrlType.Album
        Else
            Throw NotValidSpotifyUrl
        End If
    End Function

    Public Shared Function GetIdByUrl(url As String) As String
        If IsValidUrl(url) = False Then Throw NotValidSpotifyUrl
        Return url.Split("?")(0).Split("/").Last
    End Function

    Public Shared Async Function GetSpotifytrackByFulltrack(track As FullTrack) As Task(Of SpotifyTrack)
        Return New SpotifyTrack With {
            .title = Helper.MakeSafe(track.Name),
            .artist = String.Join(", ", track.Artists.Select(Function(a) Helper.MakeSafe(a.Name))),
            .artwork = If(Helper.config.metadata.artwork, track.Album.Images(0).Url, ""),
            .lenght = TimeSpan.FromMilliseconds(track.DurationMs),
            .lyrics = If(Helper.config.metadata.lyrics, Await YoutubeHelper.GetLyrics($"{Helper.MakeUriSafe(track.Artists(0).Name)} {Helper.MakeUriSafe(track.Name)}"), "N/A"),
            .album = track.Album.Name,
            .url = track.ExternalUrls("spotify"),
            .youtube = Await YoutubeHelper.GetId($"{track.Artists(0).Name} {track.Name}"),
            .release = Date.Parse(track.Album.ReleaseDate),
            .trackNumber = track.TrackNumber,
            .discNumber = track.DiscNumber
        }
    End Function

    Public Shared Async Function GetDownloadPageInfo(url As String, progress As IProgress(Of Integer), cancel As CancellationToken) As Task(Of DownloadPageInfo)
        Dim type = GetSpotifyUrlType(url)
        Dim tracks As New List(Of SpotifyTrack)
        If type = SpotifyUrlType.Track Then
            Dim track = Await client.Tracks.Get(GetIdByUrl(url))
            tracks.Add(Await GetSpotifytrackByFulltrack(track))
            cancel.ThrowIfCancellationRequested()
            Return New DownloadPageInfo With {.name = track.Name, .Image = track.Album.Images(0).Url, .tracks = tracks}
        ElseIf type = SpotifyUrlType.Album Then
            Dim album = Await client.Albums.Get(GetIdByUrl(url))
            Dim total = Await client.PaginateAll(album.Tracks)
            For Each simpletrack In total
                Dim track = Await client.Tracks.Get(simpletrack.Id)
                tracks.Add(Await GetSpotifytrackByFulltrack(track))
                progress.Report(total.IndexOf(simpletrack) / total.Count * 100)
                cancel.ThrowIfCancellationRequested()
            Next
            Return New DownloadPageInfo With {.name = album.Name, .Image = album.Images(0).Url, .tracks = tracks}
        ElseIf type = SpotifyUrlType.Playlist Then
            Dim playlist = Await client.Playlists.Get(GetIdByUrl(url))
            Dim total = Await client.PaginateAll(playlist.Tracks)
            For Each playableitem In total
                If playableitem.Track.Type = ItemType.Track Then
                    Dim track = Await client.Tracks.Get(CType(playableitem.Track, FullTrack).Id)
                    tracks.Add(Await GetSpotifytrackByFulltrack(track))
                    progress.Report(total.IndexOf(playableitem) / total.Count * 100)
                    cancel.ThrowIfCancellationRequested()
                End If
            Next
            Return New DownloadPageInfo With {.name = playlist.Name, .Image = playlist.Images(0).Url, .tracks = tracks}
        Else Throw New Exception("Error", New Exception("Failed generating download page."))
        End If
    End Function

End Class


Public Enum SpotifyUrlType
    Track
    Playlist
    Album
End Enum

Public Class SpotifyTrack
    Public Property title As String
    Public Property artist As String
    Public Property artwork As String
    Public Property lenght As TimeSpan
    Public Property lyrics As String
    Public Property album As String
    Public Property url As String
    Public Property youtube As String
    Public Property release As Date
    Public Property trackNumber As Integer
    Public Property discNumber As Integer
End Class