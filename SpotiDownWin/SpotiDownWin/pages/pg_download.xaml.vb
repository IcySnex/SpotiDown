Imports System.ComponentModel

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
    Private Sub btn_download(sender As Object, e As RoutedEventArgs)
        MsgBox(sl_container.Children.Count)
    End Sub
End Class
