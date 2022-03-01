using SpotiDown.Controls;
using SpotiDown.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiDown.Helpers;

public class Download
{
    public static Song Song = new(Enums.SongType.YouTube, "", "Song sex", "187", new(0, 2, 54), "Nothingles", DateTime.Now, "lalalalala", "https://yt3.ggpht.com/G-xQ-34A-GcfZm9ByvMnFEf1TjOatTKrJ3g0XaL1kXqCbin-7hTXhQBDe3VYtcAhx59iKG9C5jA=s900-c-k-c0x00ffffff-no-rj", 5, 1);

    public static List<Song> Queue = new() { Song, Song, Song };



}