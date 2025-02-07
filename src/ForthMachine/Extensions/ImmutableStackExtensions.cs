using System.Collections.Immutable;

namespace ForthMachine;

public static class ImmutableStackExtensions
{
    public static ImmutableStack<TItem> PopOrDefault<TItem>(this ImmutableStack<TItem> stack, TItem defaultItem, out TItem item)
    {
        if (stack.IsEmpty)
        {
            item = defaultItem;
            return stack;
        }

        return stack.Pop(out item);
    }

    public static TItem PeekOrDefault<TItem>(this ImmutableStack<TItem> stack, TItem defaultItem) =>
        stack.IsEmpty ? defaultItem : stack.Peek();
}