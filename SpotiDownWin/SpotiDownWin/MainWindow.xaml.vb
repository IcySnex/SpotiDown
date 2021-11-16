Class MainWindow
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        tbc.ShowPage(Helper.p_home)
        SetDebugMode()
    End Sub
    Public Sub SetDebugMode()
        btn_ffmpeg.Visibility = If(Helper.config.debug, Visibility.Visible, Visibility.Collapsed)
    End Sub
    Private Sub btn_home(sender As Object, e As RoutedEventArgs)
        tbc.ShowPage(Helper.p_home)
    End Sub
    Private Sub btn_settings(sender As Object, e As RoutedEventArgs)
        tbc.ShowPage(Helper.p_settings)
    End Sub
    Private Sub btn_ffmpeg_(sender As Object, e As RoutedEventArgs)
        tbc.ShowPage(Helper.p_ffmpeg)
    End Sub
End Class
