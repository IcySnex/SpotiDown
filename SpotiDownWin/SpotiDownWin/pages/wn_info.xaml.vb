Public Class wn_info

    Dim tracksender As c_track
    Public Sub New(sender As c_track)
        InitializeComponent()
        tracksender = sender
        If Helper.config.metadata.artwork Then img_image.ImageSource = Helper.GetImage(sender.trackinfo.artwork)
        lb_name.Text = sender.trackinfo.title
        lb_title.Text = sender.trackinfo.title
        lb_artist.Text = sender.trackinfo.artist
        lb_lenght.Text = Helper.FormatTimespan(sender.trackinfo.lenght)
        lb_album.Text = sender.trackinfo.album
        lb_release.Text = sender.trackinfo.release.ToString("MM/dd/yyyy")
        lb_url.Text = sender.trackinfo.url
        lb_youtube.Text = $"https://www.youtube.com/watch?v={sender.trackinfo.youtube}"
        tb_lyrics.Text = sender.trackinfo.lyrics
        AddHandler tb_lyrics.TextChanged, AddressOf tb_lyrics_TextChanged
    End Sub
    Private Sub btn_back(sender As Object, e As RoutedEventArgs)
        Close()
    End Sub
    Private Async Sub btn_download(sender As Object, e As RoutedEventArgs)
        Try
            btndownload.IsEnabled = False
            btndownload.Effect = New Effects.BlurEffect With {.Radius = 13}
            MsgBox($"Song sucessfully downloaded! (""{Await YoutubeHelper.WriteTrack(tracksender.trackinfo)}"")", MsgBoxStyle.Information, "Done!")
            btn_remove(sender, e)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Something went wrong - Error")
        End Try
    End Sub
    Private Sub btn_remove(sender As Object, e As RoutedEventArgs)
        Dim container As StackPanel = tracksender.Parent
        container.Children.Remove(tracksender)
        If container.Children.Count = 0 Then Helper.GetMain.tbc.ShowPage(Helper.p_home) Else Close()
        btn_back(sender, e)
    End Sub
    Private Sub tb_lyrics_TextChanged(sender As Object, e As TextChangedEventArgs)
        tracksender.trackinfo.lyrics = tb_lyrics.Text
    End Sub
End Class
