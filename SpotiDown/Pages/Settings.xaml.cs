using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SpotiDown.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpotiDown.Pages;

public sealed partial class Settings : Page
{
    public Settings()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Required;
    }

}