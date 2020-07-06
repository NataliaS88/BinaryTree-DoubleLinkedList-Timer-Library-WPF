using System;
using System.Collections.Generic;
using System.Text;

namespace BOXwarehouseLogica
{
    internal class DataTimeForXY
    {
        internal DateTime lastMovingTime;
        internal double x;
        internal double y;
        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType())) return false;
            DataTimeForXY found = (DataTimeForXY)obj;
            return (x == found.x) && (y == found.y);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public DataTimeForXY(double x, double y)
        {
            lastMovingTime = DateTime.Now;
            this.x = x;
            this.y = y;

        }
    }
}
