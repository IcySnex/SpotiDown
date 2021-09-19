Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports Xamarin.Forms

Public Class Helper
    Public Shared config As config = GetConfig()
    Public Shared ihs As IHelperService = DependencyService.Get(Of IHelperService)

    Public Shared Function GetConfig() As config
        If File.Exists(GetPath("config.json")) Then
            Return JsonConvert.DeserializeObject(Of config)(File.ReadAllText(GetPath("config.json")))
        Else
            Return New config
        End If
    End Function
    Public Shared Sub SaveConfig()
        File.WriteAllText(GetPath("config.json"), JsonConvert.SerializeObject(config))
    End Sub

    Public Shared Function GetPath(file As String) As String
        Return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), file)
    End Function

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
End Class

Public Interface IHelperService
    Function writeStream(path As String, stream As Stream) As Task(Of Boolean)
    Function writeMetadata(path As String, trackinfo As SpotifyTrack) As Task
    Function downloadFile(url As String, path As String) As Task
End Interface

Public Class DownloadPageInfo
    Public Property name As String
    Public Property image As String
    Public Property tracks As New List(Of SpotifyTrack)
End Class