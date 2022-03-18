using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using Windows.UI;

namespace SpotiDown;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        Helpers.Window.SetMinSize(this, 540, 550);
        Helpers.Window.SetSize(this, 1100, 600);
        Helpers.Window.SetIcon(this, "ico.ico");
        Title = "SpotiDown";

        Application.Current.Resources["SystemAccentColorLight2"] = Color.FromArgb(255, 113, 0, 219);
        Application.Current.Resources["SystemAccentColorDark1"] = Color.FromArgb(255, 113, 0, 219);

        InitializeComponent();
        Navigate((NavigationViewItem)Navigation.MenuItems[0]);
    }

    private void Navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) => 
        Navigate((NavigationViewItem)args.SelectedItemContainer);

    private void Navigate(NavigationViewItem item)
    {
        if (item is null || item.Tag is null)
            return;

        NavigationFrame.Navigate(Type.GetType(item.Tag.ToString()!), item.Content);
        Navigation.SelectedItem = item;
    }

    private void Navigation_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        try
        {
            Navigation.SelectedItem = (NavigationViewItem)Navigation.MenuItems.First(n => n is NavigationViewItem && ((NavigationViewItem)n).Tag.ToString() == NavigationFrame.BackStack.Last().SourcePageType.FullName!);
            NavigationFrame.BackStack.RemoveAt(NavigationFrame.BackStackDepth - 1);
            NavigationFrame.BackStack.RemoveAt(NavigationFrame.BackStackDepth - 1);
        }
        catch { }
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        Helpers.Local.Config.Save();
    }
}
