using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SpotiDown.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SpotiDown.Pages;

public sealed partial class FFMPEG : Page
{
    public FFMPEG()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Required;
    }


    SynchronizationContext Sync = SynchronizationContext.Current!;


    private async void Run_Click(object sender, RoutedEventArgs? e)
    {
        Output.Text = "FFMPEG PROCESSING STARTED!\n";

        Process FFMPEG = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "FFMPEG.exe",
                Arguments = Command.Text,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        FFMPEG.OutputDataReceived += (s, e) =>
            Sync.Post(val => Output.Text += $"{e.Data}\n", null);
        FFMPEG.ErrorDataReceived += (s, e) =>
            Sync.Post(val => Output.Text += $"{e.Data}\n", null);

        FFMPEG.Start();

        FFMPEG.BeginErrorReadLine();
        FFMPEG.BeginOutputReadLine();

        await FFMPEG.WaitForExitAsync();

        Output.Text += $"\nFFMPEG PROCESSING FINISHED!";
    }
    private void Command_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
            Run_Click(sender, null);
    }

    private async void Clear_Click(object sender, RoutedEventArgs e)
    {
        if (await Helpers.Window.Alert(Content.XamlRoot, "Clear output window", $"Do you really want to clear the entire content inside the FFMPEG output window?", "No", "Yes") == ContentDialogResult.Primary)
            Output.Text = "";
    }

    private void Output_TextChanged(object sender, TextChangedEventArgs e) =>
        ScrollContainer.ScrollToVerticalOffset(ScrollContainer.ScrollableHeight);
}
