Imports System.Threading

Public Class pg_loading
    Dim cancel As New CancellationTokenSource
    Public Sub New(url As String)
        InitializeComponent()
        LoadDownloadPage(url)
    End Sub
    Private Sub btn_back(sender As Object, e As RoutedEventArgs)
        cancel.Cancel()
        Helper.GetMain.tbc.ShowPage(Helper.p_home)
    End Sub

    Private Async Sub LoadDownloadPage(url As String)
        Try
            Dim progress = New Progress(Of Integer)(Sub(percent)
                                                        lb_text.Text = $"Fetching songs from Spotify... ({percent}%)"
                                                        pb_percentage.Value = percent
                                                    End Sub)
            Dim task_GetDownloadPageInfo As Task(Of DownloadPageInfo) = Task.Run(Function() SpotifyHelper.GetDownloadPageInfo(url, progress, cancel.Token), cancel.Token)
            Helper.GetMain.tbc.ShowPage(New pg_download(Await task_GetDownloadPageInfo))
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Something went wrong - Error")
            Helper.GetMain.tbc.ShowPage(Helper.p_home)
        End Try
    End Sub
End Class
