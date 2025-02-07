namespace ForthMachine;

public static class EnumerableExtensions
{
    public static void ForEach<TItem>(this IEnumerable<TItem> enumerable, Action<TItem> handler)
    {
        foreach (var item in enumerable)
            handler(item);
    }

    public static void ForEach<TItem, TResult>(this IEnumerable<TItem> enumerable, Func<TItem, TResult> handler)
    {
        foreach (var item in enumerable)
            handler(item);
    }
}