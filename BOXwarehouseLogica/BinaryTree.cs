using System;
using System.Collections.Generic;
using System.Text;

namespace BOXwarehouseLogica
{
    internal class BinaryTree<T> where T : IComparable<T> //IEnumerable<T>
    {
        internal Node root;
        public void Add(T item)
        {
            if (root == null) //empty tree
            {
                root = new Node(item);
                return;
            }
            Node tmp = root;
            Node parent = null;
            while (tmp != null)
            {
                parent = tmp;
                if (item.CompareTo(tmp.data) < 0)
                {
                    tmp = tmp.left;
                }
                else
                {
                    tmp = tmp.right;
                }
            }
            Node n = new Node(item);
            if (item.CompareTo(parent.data) < 0) //add it as a left son
                parent.left = n;
            else //add it as a right son
                parent.right = n;
        }
        public bool Search(T item, out T res)
        {
            res = default;
            if (root == null) //empty tree
            {
                return false;
            }
            Node tmp = root;
            //  Node parent = null;
            while (tmp.data.CompareTo(item) != 0)
            {
                //  parent = tmp;
                if (item.CompareTo(tmp.data) < 0)
                {
                    tmp = tmp.left;
                }
                else
                {
                    tmp = tmp.right;
                }
                if (tmp == null) break;
            }
            if (tmp == null) return false;
            res = tmp.data;
            return true;
        }
        public bool Delete(T item)
        {
            bool result = false;
            if (root == null)
                return result;
            Node tmp = root;
            Node parent = tmp;

            while (tmp != null)
            {
                if (item.CompareTo(tmp.data) < 0)
                {
                    parent = tmp;
                    tmp = tmp.left;
                }
                else if (item.CompareTo(tmp.data) == 0)
                {
                    result = true;
                    break;
                }
                else
                {
                    parent = tmp;
                    tmp = tmp.right;
                }
            }
            if (tmp == null) return result;
            if (tmp.left == null && tmp.right == null)
            {
                if (parent == tmp)
                {
                    root = null;
                }
                if (parent.left == tmp)
                {
                    parent.left = null;
                }
                if (parent.right == tmp)
                {
                    parent.right = null;
                }
            }
            if (tmp.left != null && tmp.right == null)
            {
                if (parent == tmp)
                {
                    root = tmp.left;
                }
                if (parent.left == tmp)
                {
                    parent.left = tmp.left;
                }
                if (parent.right == tmp)
                {
                    parent.right = tmp.left;
                }
            }
            if (tmp.left == null && tmp.right != null)
            {
                if (parent == tmp)
                {
                    root = tmp.right;
                }
                if (parent.left == tmp)
                {
                    parent.left = tmp.right;
                }
                if (parent.right == tmp)
                {
                    parent.right = tmp.right;
                }
            }
            if (tmp.left != null && tmp.right != null)
            {
                Node beforeParrent1 = tmp;
                Node parrent1 = tmp.right;
                Node tmp1 = parrent1.left;
                while (tmp1 != null)

                {
                    beforeParrent1 = parrent1;
                    parrent1 = tmp1;
                    tmp1 = tmp1.left;
                }
                if (parent == tmp)
                {
                    root.data = parrent1.data;
                    if (parrent1.right == null)
                    {
                        if (beforeParrent1.right == parrent1) beforeParrent1.right = null;
                        if (beforeParrent1.left == parrent1) beforeParrent1.left = null;
                    }
                    if (parrent1.right != null)
                    {
                        if (beforeParrent1.right == parrent1) beforeParrent1.right = parrent1.right;
                        if (beforeParrent1.left == parrent1) beforeParrent1.left = parrent1.right;
                    }
                }
                else
                {
                    tmp.data = parrent1.data;

                    if (parrent1.right == null)
                    {
                        if (beforeParrent1.right == parrent1)
                        {
                            beforeParrent1.right = null;
                        }
                        else
                        {
                            beforeParrent1.left = null;
                        }
                    }
                    else
                    {
                        if (beforeParrent1.right == parrent1)
                        {
                            beforeParrent1.right = parrent1.right;
                        }
                        else
                        {
                            beforeParrent1.left = parrent1.right;
                        }
                    }
                }
            }
            return result;
        }

        //searching for all suitable values in tree
        public DoubleLinkedList<T> ScanBetweenTwoValues(T min, T max)
        {
            Node searchNode = root;
            DoubleLinkedList<T> suitableSizes = new DoubleLinkedList<T>();
            return ScanBetweenTwoValues(min, max, searchNode, suitableSizes);
        }
        private DoubleLinkedList<T> ScanBetweenTwoValues(T min, T max, Node searchNode, DoubleLinkedList<T> suitableSizes)
        {

            if (searchNode == null) return null;

            if (min.CompareTo(searchNode.data) <= 0 && max.CompareTo(searchNode.data) >= 0)
            {
                suitableSizes.AddLast(searchNode.data);
            }
            if (min.CompareTo(searchNode.data) < 0)
            {
                ScanBetweenTwoValues(min, max, searchNode.left, suitableSizes);
            }
            if (max.CompareTo(searchNode.data) > 0)
            {
                ScanBetweenTwoValues(min, max, searchNode.right, suitableSizes);
            }
            return suitableSizes;
        }
        public void ScanInOrder(Action<T> actionForSingleItem)
        {
            ScanInOrder(root, actionForSingleItem);
        }
        private void ScanInOrder(Node n, Action<T> actionForSingleItem)
        {
            if (n == null) return;
            ScanInOrder(n.left, actionForSingleItem);
            actionForSingleItem(n.data);
            ScanInOrder(n.right, actionForSingleItem);
        }

        public int Depth() //glubina
        {
            return Depth(root);
        }
        private int Depth(Node n) //or we can do  => (n == null) ? 0 : Math.Max(Depth(n.left), Depth(n.right)) + 1;
        {
            return (n == null) ? 0 : Math.Max(Depth(n.left), Depth(n.right)) + 1;



            //or the same 
            // if (n == null) return 0;

            //int left = Depth(n.left);
            //int right = Depth(n.left);
            //return Math.Max(left, right);


            //or the same
            // if (n == null) return 0;
            // return Math.Max(Depth(n.left), Depth(n.right)) + 1;
        }
        internal class Node
        {
            internal T data;
            internal Node left, right;
            internal Node(T data)
            {
                this.data = data;
                //  left=right=null -no important
            }
        }
    }
}