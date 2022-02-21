using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI;

namespace SpotiDown;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        Helpers.Window.SetMinSize(this, 500, 550);
        Helpers.Window.SetSize(this, 1100, 600);
        Helpers.Window.SetIcon(this, "ico.ico");
        Title = "SpotiDown";

        Application.Current.Resources["SystemAccentColorLight2"] = Color.FromArgb(255, 113, 0, 219);
        Application.Current.Resources["SystemAccentColorDark1"] = Color.FromArgb(255, 113, 0, 219);

        InitializeComponent();
        SetCurrentNavigationViewItem((NavigationViewItem)Navigation.MenuItems[0]);
    }

    private void Navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) => 
        SetCurrentNavigationViewItem((NavigationViewItem)args.SelectedItemContainer);

    public void SetCurrentNavigationViewItem(NavigationViewItem item)
    {
        if (item is null | item.Tag is null)
            return;

        NavigationFrame.Navigate(Type.GetType(item.Tag.ToString()), item.Content);
        Navigation.Header = item.Content;
        Navigation.SelectedItem = item;
    }
}
