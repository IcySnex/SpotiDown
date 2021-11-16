Imports System.IO
Imports System.Net

Public Class pg_ffmpeg
    Private Async Sub btn_run(sender As Object, e As RoutedEventArgs)
        If File.Exists($"{Helper.config.ffmpegpath}ffmpeg.exe") = False Then
            If MessageBox.Show($"ffmpeg.exe doesnt exist in the config path (""{Helper.config.ffmpegpath}""). Do you want to download the ffmpeg executeables?)", "Something went wrong - Error", MsgBoxStyle.YesNo, MessageBoxImage.Error) = MsgBoxResult.Yes Then
                If String.IsNullOrEmpty(Helper.config.ffmpegpath) = False AndAlso Directory.Exists(Helper.config.ffmpegpath) = False Then Directory.CreateDirectory(Helper.config.ffmpegpath)
                Dim wc As New WebClient
                Await wc.DownloadFileTaskAsync("https://github.com/IcySnex/SpotiDown/raw/main/other/ffmpeg.exe", $"{Helper.config.ffmpegpath}ffmpeg.exe")
            Else Return
            End If
        End If
        If String.IsNullOrWhiteSpace(tb_cmd.Text) Then
            MessageBox.Show("Command connot be empty!", "Something went wrong - Error", MsgBoxStyle.OkOnly, MessageBoxImage.Error)
            Return
        End If
        Dim proc As New Process With {.StartInfo = New ProcessStartInfo With {.RedirectStandardError = True, .RedirectStandardOutput = True, .UseShellExecute = False, .CreateNoWindow = True, .FileName = $"{Helper.config.ffmpegpath}ffmpeg", .Arguments = tb_cmd.Text}, .EnableRaisingEvents = True}
        AddHandler proc.ErrorDataReceived, Sub(s, es) Dispatcher.Invoke(Sub() tb_resukt.Text &= es.Data & vbNewLine)
        AddHandler proc.OutputDataReceived, Sub(s, es) Dispatcher.Invoke(Sub() tb_resukt.Text &= es.Data & vbNewLine)
        tb_resukt.Text = ""
        proc.Start()
        proc.BeginErrorReadLine()
        proc.BeginOutputReadLine()
    End Sub
End Class
