using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Core.Objects.Wrappers
{
    public class FixedSizeObservableCollection<T> : ObservableCollection<T>
    {
        private object Lock = new object();
        public int Size { get; set; }
        public int LastRemoveIndex { get; private set; } = -1;
        public FixedSizeObservableCollection(int size)
        {
            Size = size;
        }

        public new void Add(T obj)
        {
            Add(obj, false);
        }

        public void Add(T obj, bool isUnique)
        {
            bool preRemoved = false;
            if (isUnique && base.Items.Any(o => o.Equals(obj)))
            {
                preRemoved = true;
                base.Items.Remove(obj);
            }
            base.Add(obj);
            if (preRemoved) return;
            lock (Lock)
            {
                if (base.Count > Size)
                {
                    int i = 0;
                    for (i = 0; i < Size; i++)
                    {
                        base[i] = base[i + 1];
                    }
                    base.RemoveAt(i);
                }
            }
        }

        public void Insert(T obj)
        {
            Insert(obj, false);
        }

        public void Insert(T obj, bool isUnique)
        {
            bool preRemoved = false;
            if (isUnique && base.Items.Any(o => o.Equals(obj)))
            {
                preRemoved = true;
                base.Items.Remove(obj);
            }
            base.Insert(0, obj);
            if (preRemoved) return;

            lock (Lock)
            {
                for (int i = base.Count - 1; i >= Size; i--)
                {
                    base.RemoveAt(i);
                }
            }
        }

        public new void Insert(int index, T obj)
        {
            Insert(index, obj, false);
        }

        public void Insert(int index, T obj, bool isUnique)
        {
            bool preRemoved = false;
            if (isUnique && base.Items.Any(o => o.Equals(obj)))
            {
                preRemoved = true;
                base.Items.Remove(obj);
            }
            base.Insert(index, obj);
            if (preRemoved) return;

            lock (Lock)
            {
                for (int i = base.Count - 1; i >= Size; i--)
                {
                    base.RemoveAt(i);
                }
            }
        }

        public new void Remove(T obj)
        {
            LastRemoveIndex = base.IndexOf(obj);
            if (LastRemoveIndex == -1) return;
            lock (Lock)
            {
                base.RemoveAt(LastRemoveIndex);
            }
        }

        public void RemoveLast()
        {
            lock (Lock)
            {
                base.RemoveAt(base.Count - 1);
            }
        }
        public void RemoveFirst()
        {
            lock (Lock)
            {
                base.RemoveAt(0);
            }
        }

        public T Last => base[base.Count - 1];
        public T First => base[0];


    }
}
