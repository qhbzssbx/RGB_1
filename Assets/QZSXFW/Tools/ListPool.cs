using System.Collections.Generic;
using UnityEngine.Pool;

/// <summary>
/// ��UGUIԴ����Ų������
/// </summary>
/// <typeparam name="T"></typeparam>
internal static class ListPool<T>
    {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>(null, l => l.Clear());

        public static List<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            s_ListPool.Release(toRelease);
        }
    }