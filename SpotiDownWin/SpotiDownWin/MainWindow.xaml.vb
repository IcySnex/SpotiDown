Class MainWindow
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        tbc.ShowPage(Helper.p_home)
    End Sub
    Private Sub btn_home(sender As Object, e As RoutedEventArgs)
        tbc.ShowPage(Helper.p_home)
    End Sub
    Private Sub btn_settings(sender As Object, e As RoutedEventArgs)
        tbc.ShowPage(Helper.p_settings)
    End Sub
End Class
