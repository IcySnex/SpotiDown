Imports System.Drawing
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports TagLib.Id3v2

Public Class Helper
    Public Shared config As config = GetConfig()

    Public Shared p_home As New pg_home
    Public Shared p_settings As New pg_settings
    Public Shared Function GetMain() As MainWindow
        Return Application.Current.MainWindow
    End Function

    Public Shared Function GetConfig() As config
        If File.Exists("config.json") Then
            Return JsonConvert.DeserializeObject(Of config)(File.ReadAllText("config.json"))
        Else
            Return New config
        End If
    End Function
    Public Shared Sub SaveConfig()
        File.WriteAllText("config.json", JsonConvert.SerializeObject(config))
    End Sub

    Public Shared Function RandomString(minCharacters As Integer, maxCharacters As Integer) As String
        Dim s As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
        Static r As New Random
        Dim sb As New StringBuilder
        For i As Integer = 1 To r.Next(minCharacters, maxCharacters)
            Dim idx As Integer = r.Next(0, s.Length)
            sb.Append(s.Substring(idx, 1))
        Next
        Return sb.ToString()
    End Function

    Public Shared Function MakeSafe(ByVal input As String) As String
        Return Regex.Replace(input, "[\/\\\?\*\<\>\|\:\""]", "#")
    End Function
    Public Shared Function MakeUriSafe(ByVal input As String) As String
        Return Regex.Replace(input, "[\s+\&]", "%20")
    End Function
    Public Shared Function MakeFileSafe(ByVal input As String) As String
        Return Regex.Replace(If(input.Trim.EndsWith("/"), input.Trim.Remove(input.Trim.Length - 1), input.Trim), "[\\\?\*\<\>\|\:\""]", "#")
    End Function
    Public Shared Function MakePathSafe(ByVal input As String) As String
        Dim s = MakeFileSafe(input).Trim
        If s.StartsWith("/") = False Then s = $"/{s}"
        If s.EndsWith("/") Then s = s.Remove(s.Length - 1)
        Return s
    End Function

    Public Shared Function FormatTimespan(timespan As TimeSpan) As String
        If timespan.Hours > 0 Then
            Return $"{timespan.Hours}h, {timespan.Minutes}m, {timespan.Seconds}s"
        ElseIf timespan.Minutes > 0 Then
            Return $"{timespan.Minutes}m, {timespan.Seconds}s"
        Else
            Return $"{timespan.Seconds}s"
        End If
    End Function

    Public Shared Function CreateSpotifyTrack(title_ As String, artist_ As String, artwork_ As String, lenght_ As TimeSpan) As SpotifyTrack
        Return New SpotifyTrack With {.title = title_, .artist = artist_, .artwork = artwork_, .lenght = lenght_}
    End Function

    Public Shared Async Function DownloadHtmlUTF8(url As String) As Task(Of String)
        Return Await (New WebClient With {.Encoding = Encoding.UTF8}).DownloadStringTaskAsync(url)
    End Function

    Public Shared Function GetImage(url As String) As BitmapImage
        Dim bi As New BitmapImage
        bi.BeginInit()
        bi.UriSource = New Uri(url)
        bi.EndInit()
        Return bi
    End Function

    Public Shared Async Function FileDeleteAsync(path As String) As Task
        Dim fi = New FileInfo(path)
        If fi.Exists Then
            fi.Delete()
            fi.Refresh()
            While fi.Exists
                Await Task.Delay(100)
                fi.Refresh()
            End While
        End If
    End Function

    Public Shared Async Function writeStream(filepath As String, stream As Stream) As Task(Of Boolean)
        If File.Exists($"{config.ffmpegpath}ffmpeg.exe") = False Then
            If MessageBox.Show($"ffmpeg.exe doesnt exist in the config path (""{config.ffmpegpath}""). Do you want to download the ffmpeg executeables?)", "Something went wrong - Error", MsgBoxStyle.YesNo, MessageBoxImage.Error) = MsgBoxResult.Yes Then
                If String.IsNullOrEmpty(config.ffmpegpath) = False AndAlso Directory.Exists(config.ffmpegpath) = False Then Directory.CreateDirectory(config.ffmpegpath)
                Dim wc As New WebClient
                Await wc.DownloadFileTaskAsync("https://github.com/IcySnex/SpotiDown/raw/main/other/ffmpeg.exe", $"{config.ffmpegpath}ffmpeg.exe")
            Else Return False
            End If
        End If
        If File.Exists(filepath) Then Await FileDeleteAsync(filepath)
        Dim ffmpeg As New Process With {.StartInfo = New ProcessStartInfo With {
                .FileName = $"{config.ffmpegpath}ffmpeg",
                .Arguments = $"-hide_banner -loglevel quiet -i - -q:a 0 ""{filepath}""",
                .UseShellExecute = False, .CreateNoWindow = True, .RedirectStandardInput = True}}
        ffmpeg.Start()
        Dim input = ffmpeg.StandardInput.BaseStream
        Await stream.CopyToAsync(input)
        input.Close()
        ffmpeg.WaitForExit()
        Return True
    End Function

    Public Shared Async Function writeMetadata(path As String, trackinfo As SpotifyTrack) As Task
        Tag.DefaultVersion = 3
        Tag.ForceDefaultVersion = True
        Dim file As TagLib.File = TagLib.File.Create(path)
        file.Tag.AlbumArtists = {trackinfo.artist.Split(",")(0)}
        file.Tag.Publisher = trackinfo.artist
        file.Tag.Performers = trackinfo.artist.Split(",")
        file.Tag.Composers = trackinfo.artist.Split(",")
        file.Tag.Copyright = "Spotify"
        file.Tag.Lyrics = trackinfo.lyrics
        file.Tag.Title = trackinfo.title
        file.Tag.Genres = {"Music"}
        file.Tag.Album = trackinfo.album
        file.Tag.Track = Convert.ToUInt32(trackinfo.trackNumber)
        file.Tag.Disc = Convert.ToUInt32(trackinfo.discNumber)
        file.Tag.DiscCount = Convert.ToUInt32(trackinfo.discNumber)
        file.Tag.Year = Convert.ToUInt32(trackinfo.release.Year)
        file.Tag.Comment = $"Song downloaded via SpotiDown.
SpotiDown is created by IcySnex (https://github.com/IcySnex/SpotiDown).
Using this tool is at your own risk!

Full Copyright for this song goes to the artist(s): {trackinfo.artist}.
Track meta data fetched from Spotify, Track downloaded from YouTube, Lyrics fetched from genius.com!

Spotify URL: {trackinfo.url}
YouTube URL: https://www.youtube.com/watch?v={trackinfo.youtube}"
        file.Tag.Pictures = {New TagLib.Picture(Await New WebClient().DownloadDataTaskAsync(trackinfo.artwork))}
        file.Save()
    End Function
End Class

Public Class DownloadPageInfo
    Public Property name As String
    Public Property image As String
    Public Property tracks As New List(Of SpotifyTrack)
End Class