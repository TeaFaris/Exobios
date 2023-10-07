using System.Collections;
using System.Collections.Generic;

public sealed class InsertList<T> : IEnumerable<T>
{
    public int InitialCount { get; private set; }

    public int Count => values.Count;

    private List<T> values;

    public IEnumerator<T> GetEnumerator() => values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)values).GetEnumerator();

    public InsertList(int count)
    {
        InitialCount = count;
        values = new List<T>(count);
    }

    public T this[int index]
    {
        get => values[index];
    }

    public void Insert(params T[] items)
    {
        values.InsertRange(0, items);

        if(Count >= InitialCount)
        {
            values.RemoveRange(InitialCount - 1, Count - InitialCount);
        }
    }
}