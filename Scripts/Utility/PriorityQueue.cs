using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
    private List<(T item, int priority)> heap = new List<(T, int)>();

    public int Count => heap.Count;

    public void Enqueue(T item, int priority)
    {
        heap.Add((item, priority));
        HeapifyUp(heap.Count - 1);
    }

    public T Dequeue()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        T result = heap[0].item;
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);
        HeapifyDown(0);

        return result;
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (heap[index].priority >= heap[parent].priority)
                break;

            (heap[index], heap[parent]) = (heap[parent], heap[index]);
            index = parent;
        }
    }

    private void HeapifyDown(int index)
    {
        int lastIndex = heap.Count - 1;

        while (true)
        {
            int left = index * 2 + 1;
            int right = index * 2 + 2;
            int smallest = index;

            if (left <= lastIndex && heap[left].priority < heap[smallest].priority)
                smallest = left;
            if (right <= lastIndex && heap[right].priority < heap[smallest].priority)
                smallest = right;

            if (smallest == index) break;

            (heap[index], heap[smallest]) = (heap[smallest], heap[index]);
            index = smallest;
        }
    }
}
