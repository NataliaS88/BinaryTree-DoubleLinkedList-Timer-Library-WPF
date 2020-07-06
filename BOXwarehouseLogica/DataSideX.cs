using System;
using System.Collections.Generic;
using System.Text;

namespace BOXwarehouseLogica
{
    internal class DataSideX : IComparable<DataSideX>
    {
       internal double lengthX;
      internal  BinaryTree<DataSideY> tree;
        public DataSideX(double length  )
        {
            this.lengthX = length;
            tree = new BinaryTree<DataSideY>();
        }
        public int CompareTo(DataSideX other)
        {
            if (other == null) return 1;
            return this.lengthX.CompareTo(other.lengthX);
        }
    }
}
