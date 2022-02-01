using System.Collections.Generic;

public static class ListHelper
{
    public static void RandomShuffle<T>(this IList<T> list)
    {
        var count = list.Count;
        var last = count - 1;
        for (var index = 0; index < last; ++index)
        {
            var r = UnityEngine.Random.Range(index, count);
            var swap = list[index];
            list[index] = list[r];
            list[r] = swap;
        }
    }
}