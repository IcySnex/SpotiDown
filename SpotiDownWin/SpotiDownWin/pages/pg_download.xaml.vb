Public Class pg_download
    Public Sub New(info As DownloadPageInfo)
        InitializeComponent()
        lb_name.Text = info.name
        img_backgground.Source = Helper.GetImage(info.image)
        img_image.ImageSource = Helper.GetImage(info.image)
        For Each t In info.tracks
            sl_container.Children.Add(New c_track(t))
        Next
    End Sub
    Private Sub btn_back(sender As Object, e As RoutedEventArgs)
        Helper.GetMain.tbc.ShowPage(Helper.p_home)
    End Sub
    Private Async Sub btn_download(sender As Object, e As RoutedEventArgs)
        btndownload.IsEnabled = False
        btndownload.Effect = New Effects.BlurEffect With {.Radius = 13}
        For i As Integer = 0 To sl_container.Children.Count - 1
            Dim c As c_track = sl_container.Children(0)
            c.btndownload.IsEnabled = False
            c.btndownload.Effect = New Effects.BlurEffect With {.Radius = 13}
            Try
                Await YoutubeHelper.WriteTrack(c.trackinfo)
            Catch ex As Exception
                MsgBox($"Failed to download song ({c.trackinfo.title}) (ex: {ex.Message})", MsgBoxStyle.Information, "Done!")
            End Try
            sl_container.Children.Remove(c)
        Next
        btndownload.IsEnabled = True
        btndownload.Effect = Nothing
        MsgBox($"All songs sucessfully downloaded!", MsgBoxStyle.Information, "Done!")
        Helper.GetMain.tbc.ShowPage(Helper.p_home)
    End Sub
End Class
