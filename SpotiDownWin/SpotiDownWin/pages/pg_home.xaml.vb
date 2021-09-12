Public Class pg_home
    Private Sub tb_url_TextChanged(sender As Object, e As TextChangedEventArgs)
        lb_url.Visibility = If(tb_url.Text = "", Visibility.Visible, Visibility.Hidden)
    End Sub
End Class
