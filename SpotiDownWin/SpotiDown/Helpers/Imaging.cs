using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotiDown.Helpers;

public class Imaging
{
    public static BitmapImage FromUrl(string Url) =>
        new BitmapImage(new Uri(Url));
}