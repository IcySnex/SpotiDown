using System;

namespace SpotiDown.Helpers;

public class Text
{
    public static string FormatTime(TimeSpan Time, string Format = "{hh} hrs, {mm} min, {ss} sec")
    {
        if (Time.Hours > 0)
            Format = Format.Replace("{hh}", Time.Hours.ToString());
        if (Time.Minutes > 0)
            Format = Format.Replace("{mm}", Time.Minutes.ToString());
        if (Time.Seconds > 0)
            Format = Format.Replace("{ss}", Time.Seconds.ToString());
        return Format;
    }
}