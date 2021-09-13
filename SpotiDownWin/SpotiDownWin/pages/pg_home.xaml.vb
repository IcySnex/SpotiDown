Public Class pg_home
    Private Sub tb_url_TextChanged(sender As Object, e As TextChangedEventArgs)
        lb_url.Visibility = If(tb_url.Text = "", Visibility.Visible, Visibility.Hidden)
    End Sub

    Private Async Sub btn_search(sender As Object, e As RoutedEventArgs)
        Try
            SpotifyHelper.client = Await SpotifyHelper.ConnectToSpotify()
            If String.IsNullOrWhiteSpace(tb_url.Text) = False AndAlso SpotifyHelper.IsValidUrl(tb_url.Text) Then
                Helper.GetMain.tbc.ShowPage(New pg_loading(tb_url.Text))
            Else
                MsgBox("Input Url is not a valid Spotify Url.", MsgBoxStyle.Critical, "Something went wrong - Error")
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Something went wrong - Error")
        End Try
    End Sub
    Private Sub btn_github(sender As Object, e As RoutedEventArgs)
        Process.Start("https://github.com/IcySnex/SpotiDown")
    End Sub
    Private Sub btn_youtube(sender As Object, e As RoutedEventArgs)
        Process.Start("https://www.youtube.com/channel/UCiaH5KyxTwmd57tbDLX5gmw")
    End Sub
    Private Sub btn_share(sender As Object, e As RoutedEventArgs)
        If MessageBox.Show("Do you want to copy the download URL to your clipboard?", "Share!", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then My.Computer.Clipboard.SetText("https://github.com/IcySnex/SpotiDown")
    End Sub
    Private Sub btn_discord(sender As Object, e As RoutedEventArgs)
        Process.Start("https://discord.gg/3ycHEVMhT4")
    End Sub
End Class
