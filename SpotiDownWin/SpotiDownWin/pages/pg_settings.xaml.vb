Public Class pg_settings
    Public Sub New()
        InitializeComponent()
        updateUI()
    End Sub
    Public Sub updateUI()
        switch_meta_lyrics.IsChecked = Helper.config.metadata.lyrics
        switch_meta_artwork.IsChecked = Helper.config.metadata.artwork

        cb_pref_quality.SelectedIndex = Helper.config.prefernces.quality
        tb_pref_downloadpath.Text = Helper.config.prefernces.downloadpath
        tb_pref_filename.Text = Helper.config.prefernces.filename

        tb_ffmpeg.Text = Helper.config.ffmpegpath

        tb_oauth_clientid.Text = Helper.config.oauth.id
        tb_oauth_clientsecret.Text = Helper.config.oauth.secret
    End Sub

    Private Sub switch_meta_lyrics_Toggled(sender As Object, e As RoutedEventArgs)
        Helper.config.metadata.lyrics = switch_meta_lyrics.IsChecked
        Helper.SaveConfig()
    End Sub
    Private Sub switch_meta_artwork_Toggled(sender As Object, e As RoutedEventArgs)
        Helper.config.metadata.artwork = switch_meta_artwork.IsChecked
        Helper.SaveConfig()
    End Sub

    Private Sub cb_pref_quality_SelectedChanged(sender As Object, e As SelectionChangedEventArgs)
        Helper.config.prefernces.quality = cb_pref_quality.SelectedIndex
        Helper.SaveConfig()
    End Sub
    Private Sub tb_pref_downloadpath_unfocused(sender As Object, e As RoutedEventArgs)
        Helper.config.prefernces.downloadpath = tb_pref_downloadpath.Text
        Helper.SaveConfig()
    End Sub
    Private Sub tb_pref_filename_unfocused(sender As Object, e As RoutedEventArgs)
        Helper.config.prefernces.filename = tb_pref_filename.Text
        Helper.SaveConfig()
    End Sub
    Private Sub tb_pref_filename_info(sender As Object, e As MouseButtonEventArgs)
        MsgBox(
            "If you use any of these attributes in the filename, they will get replaced with the matching info!
            
            {title} = Song Title
            {artist} = Song Main Artist
            {album} = Song Album 
            {release} = Song Release Year", MsgBoxStyle.Information, "Filename Replacements:")
    End Sub

    Private Sub tb_ffmpeg_unfocused(sender As Object, e As RoutedEventArgs)
        Helper.config.ffmpegpath = tb_ffmpeg.Text
        Helper.SaveConfig()
    End Sub

    Private Sub tb_oauth_clientid_unfocused(sender As Object, e As RoutedEventArgs)
        Helper.config.oauth.id = tb_oauth_clientid.Text
        Helper.SaveConfig()
    End Sub
    Private Sub tb_oauth_clientsecret_unfocused(sender As Object, e As RoutedEventArgs)
        Helper.config.oauth.secret = tb_oauth_clientsecret.Text
        Helper.SaveConfig()
    End Sub

    Private Sub btn_reset(sender As Object, e As RoutedEventArgs)
        Helper.config = New config
        Helper.SaveConfig()
        updateUI()
    End Sub
    Private Sub btn_show(sender As Object, e As RoutedEventArgs)
        Process.Start("config.json")
    End Sub
End Class
