using System;
using System.Collections.Generic;
using System.Text;

namespace BOXwarehouseLogica
{
    internal class DataSideY:IComparable<DataSideY>
    {
    internal double lengthY;
        internal int quantity;
        internal static int maxCount = 10;
       internal DoubleLinkedList<DataTimeForXY>.Node refOfBoxTimeQueue;
        public DataSideY(double length, int quantity = 1)
        {
            this.lengthY = length;
            this.quantity = quantity;
        }
        public int CompareTo(DataSideY other)
        {
            if (other == null) return 1;
            return this.lengthY.CompareTo(other.lengthY);
        }
    }
}
