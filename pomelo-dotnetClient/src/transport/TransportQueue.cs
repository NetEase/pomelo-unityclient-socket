using System;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 消息队列
/// </summary>
public class TransportQueue<T>
{
    public TransportQueue()
        : this(20)
    {
    }

    public TransportQueue(int capacity)
    {
        _queue = new Queue<T>(capacity);
    }

    private Queue<T> _queue;

    public void Enqueue(T data)
    {
        ICollection locker = _queue;
        lock (locker.SyncRoot)
        {
            _queue.Enqueue(data);
        }
    }

    public T Dequeue()
    {
        ICollection locker = _queue;
        lock (locker.SyncRoot)
        {
            return _queue.Dequeue();
        }
    }

    public int Count { get { return _queue.Count; } }
}