using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BOXwarehouseLogica
{
    internal class DoubleLinkedList<T> : IEnumerable<T>
    {
        internal Node start;
        internal Node end;
        public void AddFist(T value)
        {
            Node n = new Node(value);
            n.next = start;
            start = n;
            if (n.next == null) end = n;
            if (start.next == end) end.previous = start;
        }
        public Node GetFirst()
        {
            return start;
        }
        public bool RemoveFirst(out T removedValue)
        {
            removedValue = default(T);
            if (start == null) return false;
            removedValue = start.value;
            start = start.next;
            if (start == null)
            {
                end = start;
            }
            return true;
        }
        //remove node from exact place
        public bool RemoveAtAll(Node n)
        {
            if (start == null) return false;
            else if (n == start)
            {
                RemoveFirst(out T removedValue);
            }
            else
            {
                n.previous.next = n.next;
            }
            return true;
        }
        public void AddLast(T value)
        {
            if (start == null)
                AddFist(value);
            else
            {
                Node n = new Node(value);
                Node tmp = end;
                end.next = n;
                end = n;
                end.previous = tmp;
            }
        }
        public bool RemoveLast(out T removedValue)
        {
            removedValue = default(T);
            if (start == null) return false;
            if (start == end)
            { return RemoveFirst(out T removed); }
            else
            {
                removedValue = end.value;
                end = end.previous;
                end.next = null;
                if (start.next == end)
                {
                    end.previous = start;
                }
                return true;
            }
        }
        public override string ToString()
        {
            Node tmp = start;
            StringBuilder sb = new StringBuilder();
            while (tmp != null)
            {
                sb.AppendLine(tmp.value.ToString());
                tmp = tmp.next;
            }
            return sb.ToString();
        }
        public IEnumerator<T> GetEnumerator()
        {
            var current = start;
            while (current != null)
            {
                yield return current.value;
                current = current.next;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this).GetEnumerator();
        }
        internal class Node
        {
            public T value;
            public Node next;
            public Node previous;
            public Node(T value)
            {
                this.value = value;
            }
        }
    }
}
