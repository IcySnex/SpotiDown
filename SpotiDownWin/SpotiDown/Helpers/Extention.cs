using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode.Common;

namespace SpotiDown.Helpers;

static class Extention
{
    public static IEnumerable<T> WithCancellation<T>(this IEnumerable<T> en, CancellationToken token)
    {
        foreach (var item in en)
        {
            token.ThrowIfCancellationRequested();
            yield return item;
        }
    }

    public static string? IndexIfExists(this IEnumerable<Thumbnail> en, int Index)
    {
        int C = en.Count();
        if (en is null || C == 0)
            return default;
        if (C >= Index)
            return en.ElementAt(1).Url;
        else 
            return en.IndexIfExists(Index - 1);
    }
}