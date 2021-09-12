Class MainWindow
    Public Shared p_home As New pg_home
    Public Shared p_settings As New pg_settings
    Private Sub btn_home(sender As Object, e As RoutedEventArgs)
        tbc.ShowPage(p_home)
    End Sub
    Private Sub btn_settings(sender As Object, e As RoutedEventArgs)
        tbc.ShowPage(p_settings)
    End Sub
End Class
