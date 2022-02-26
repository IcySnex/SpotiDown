using Newtonsoft.Json;
using System;
using System.IO;

namespace SpotiDown.Helpers;

public class Text
{
    public static string Serialize(object Input) =>
        JsonConvert.SerializeObject(Input);

    public static T Deserialize<T>(string Input, bool IsFile = true) =>
        JsonConvert.DeserializeObject<T>(IsFile ? File.ReadAllText(Input + ".json") : Input) is T Result ? Result : throw Exceptions.JsonInvalid;

    public static string FormatTime(TimeSpan Time, string Format = "{hh}h, {mm}m, {ss}s")
    {
        if (Time.Hours > 0)
            Format = Format.Replace("{hh}", Time.Hours.ToString());
        if (Time.Minutes > 0)
            Format = Format.Replace("{mm}", Time.Minutes.ToString());
        if (Time.Seconds > 0)
            Format = Format.Replace("{ss}", Time.Seconds.ToString());
        Format = Format.Replace("{hh}h, ", "").Replace("{mm}m, ", "").Replace("{ss}s", "");
        return Format;
    }
}