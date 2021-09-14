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
        Dim btn As Button = CType(CType(Parent, StackPanel).Parent, StackPanel).Children(1)
        Try
            btndownload.IsEnabled = False
            btndownload.Effect = New Effects.BlurEffect With {.Radius = 13}
            btn.IsEnabled = False
            btn.Effect = New Effects.BlurEffect With {.Radius = 13}
            MsgBox($"Song sucessfully downloaded! (""{Await YoutubeHelper.WriteTrack(trackinfo)}"")", MsgBoxStyle.Information, "Done!")
            btn.IsEnabled = True
            btn.Effect = Nothing
            Dim container As StackPanel = Parent
            container.Children.Remove(Me)
            If container.Children.Count = 0 Then Helper.GetMain.tbc.ShowPage(Helper.p_home)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Something went wrong - Error")
            btndownload.IsEnabled = False
            btndownload.Effect = New Effects.BlurEffect With {.Radius = 13}
            btn.IsEnabled = True
            btn.Effect = Nothing
        End Try
    End Sub
End Class
