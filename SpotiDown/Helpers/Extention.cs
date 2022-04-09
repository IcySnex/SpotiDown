using System;
using System.Collections.Generic;
using System.IO;
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
}