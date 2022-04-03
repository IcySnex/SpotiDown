using Microsoft.UI.Xaml.Media.Imaging;
using SpotiDown.Controls;
using SpotiDown.Interfaces;
using System.Collections.Generic;

namespace SpotiDown.Models
{
    public class LibraryContainer
    {
        public LibraryContainer(string Name, string Image, List<ILibrary>? Children)
        {
            this.Name = Name;
            this.Image = Image;
            this.Children = Children;
        }

        public string Name { get; set; }
        public string Image { get; set; }
        public List<ILibrary>? Children { get; set; } = new();
    }
}
