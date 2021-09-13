Public Class c_track
    Public trackinfo As SpotifyTrack
    Public Sub New(info As SpotifyTrack)
        InitializeComponent()
        trackinfo = info
        updateui()
    End Sub
    Private Sub updateui()
        lb_title.Text = trackinfo.title
        lb_artist.Text = trackinfo.artist
        If Helper.config.metadata.artwork Then img_artwork.ImageSource = Helper.GetImage(trackinfo.artwork)
        lb_lenght.Text = Helper.FormatTimespan(trackinfo.lenght)
    End Sub

    Private Sub btn_info(sender As Object, e As RoutedEventArgs)
        Dim frm As New wn_info(Me) With {.Owner = Helper.GetMain}
        frm.ShowDialog()
    End Sub
    Private Async Sub btn_download(sender As Object, e As RoutedEventArgs)
        Await YoutubeHelper.WriteTrack(trackinfo)
    End Sub
End Class
