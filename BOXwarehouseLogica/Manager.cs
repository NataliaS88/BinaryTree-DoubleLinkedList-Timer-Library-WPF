using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BOXwarehouseLogica
{
    public class Manager
    {
        private BinaryTree<DataSideX> mainTree = new BinaryTree<DataSideX>();
        private DoubleLinkedList<DataTimeForXY> boxTimeQueue = new DoubleLinkedList<DataTimeForXY>();
        static Action<string> myShowMessage;
        private Timer timer;
        private const int startCheckInMin = 1;
        private const int repeatCheckInMin = 1;
        DataSideX searchX;
        DataSideX foundX;
        DataSideY searchY;
        DataSideY foundY;
        public Manager(Action<string> showMessage)
        {
            timer = new Timer(RemoveOldBoxes, null, new TimeSpan(0, startCheckInMin, 8), new TimeSpan(0, repeatCheckInMin, 0));
            myShowMessage = showMessage;
        }
        //adding to end boxTimeQueue
        public void AddBox(double x, double y, int amount)
        {
            if (x > 0 && y > 0 && amount > 0)
            {
                searchX = new DataSideX(x);
                searchY = new DataSideY(y, amount);
                if (mainTree.Search(searchX, out foundX))
                {
                    if (foundX.tree.Search(searchY, out foundY))
                    {
                        if (foundY.quantity == DataSideY.maxCount) myShowMessage($"Stock of this boxes is full, we cant add more. {amount} boxes we will return to supplier");
                        else
                        {
                            DataTimeForXY old = new DataTimeForXY(foundX.lengthX, foundY.lengthY);
                            RemoveNodeFromTimeQueue(foundY);
                            foundY.quantity += amount;
                            foundY.refOfBoxTimeQueue = AddNodeToTimeQueue(foundX.lengthX, foundY.lengthY);
                            if (foundY.quantity > DataSideY.maxCount)
                            {
                                int boxToReturn = foundY.quantity - DataSideY.maxCount;
                                foundY.quantity = DataSideY.maxCount;
                                myShowMessage($"We already have such box in stock, we wanted to add {amount} boxes but we have limit {DataSideY.maxCount} boxes, " +
                                    $"so {boxToReturn} boxes we will return to supplier. Our stock now {DataSideY.maxCount} boxes.");
                            }
                            else
                                myShowMessage($"We already have such box in stock, we added {amount} to stock.  Our stock now {foundY.quantity} boxes.");
                        }
                    }
                    else
                    {
                        searchY.refOfBoxTimeQueue = AddNodeToTimeQueue(x, y);
                        if (searchY.quantity > DataSideY.maxCount)
                        {
                            int boxToReturn = searchY.quantity - DataSideY.maxCount;
                            searchY.quantity = DataSideY.maxCount;
                            foundX.tree.Add(searchY);
                            myShowMessage($"We added new box (depth ={foundX.lengthX}, height={foundY.lengthY}) to stock.   But we have limit {DataSideY.maxCount} boxes, so {boxToReturn} boxes we will return to supplier. Now we have in stock {DataSideY.maxCount} such boxes.");
                        }
                        else
                        {
                            foundX.tree.Add(searchY);
                            myShowMessage($"We added new box (depth ={foundX.lengthX}, height={searchY.lengthY}) to stock. Now we have in stock {searchY.quantity} such boxes.");
                        }
                    }
                }
                else
                {
                    searchY.refOfBoxTimeQueue = AddNodeToTimeQueue(x, y);
                    if (searchY.quantity > DataSideY.maxCount)
                    {
                        int boxToReturn = searchY.quantity - DataSideY.maxCount;
                        searchY.quantity = DataSideY.maxCount;
                        searchX.tree.Add(searchY);
                        mainTree.Add(searchX);
                        myShowMessage($"We added new box to stock. But we have limit {DataSideY.maxCount} boxes, so {boxToReturn} boxes we will return to supplier. Now we have in stock { searchY.quantity} such boxes.");
                    }
                    else
                    {
                        searchX.tree.Add(searchY);
                        mainTree.Add(searchX);
                        myShowMessage($"We added new box (depth ={searchX.lengthX}, height={searchY.lengthY}) to stock.  Now we have in stock {searchY.quantity} such boxes.");
                    }
                }
            }
        }
        public void ShowBoxInfo(double x, double y)
        {
            searchX = new DataSideX(x);
            searchY = new DataSideY(y);
            if (mainTree.Search(searchX, out foundX))
            {
                if (foundX.tree.Search(searchY, out foundY))
                {
                    myShowMessage($"We have box that you are looking: depth={x}, height={y}, amount in stock={foundY.quantity}");
                }
                else
                    myShowMessage($"Sorry. We don't have box that you are looking:  depth={x}, height={y}");
            }
            else
                myShowMessage($"Sorry. We don't have box that you are looking: width and depth={x}, height={y}");
        }
        public void SellBoxes(double x, double y, int amount = 1, double increasingX = 20, double increasingY = 20,
            bool isDividable = true, bool isAcceptLess = true)
        {
            bool sold = false;
            int soldAmount = 0;
            double maxX = (x * increasingX) / 100 + x;
            double maxY = (y * increasingY) / 100 + y;
            DataSideX minDataX = new DataSideX(x);
            DataSideX maxDataX = new DataSideX(maxX);
            DataSideY minDataY = new DataSideY(y);
            DataSideY maxDataY = new DataSideY(maxY);
            if (mainTree.root == null)
            {
                myShowMessage($"The stock is empty.");
                return;
            }

            //first we check if we have suitable box with side x
            DoubleLinkedList<DataSideX> suitableSizesX = mainTree.ScanBetweenTwoValues(minDataX, maxDataX);
            if (suitableSizesX.end == null)
            {
                myShowMessage($"Sorry we haven't box with sizes like you need.");
            }
            else
            {
                //if we have suitable boxes by x side - first we will try to find exact box and amount that user ask
                suitableSizesX = mainTree.ScanBetweenTwoValues(minDataX, minDataX);
                sold = LokingExactSizeAndAmount(minDataY, amount, ref sold, suitableSizesX);
                StringBuilder sb = new StringBuilder();
                if (!sold)
                {
                    //if we have didn't find exact box - we will try to find exact amount that user ask without sorting
                    suitableSizesX = mainTree.ScanBetweenTwoValues(minDataX, maxDataX);
                    sold = LookingExactAmount(minDataY, amount, ref sold, maxDataY, suitableSizesX);
                }
                //if we have didn't find exact box from previous seaching- we will try to find some boxes that suitable for user input 
                if (!sold && isDividable && isAcceptLess)
                {
                    sb.AppendLine("We have box/es like you need but with sorting.");
                    soldAmount = 0;
                    int rest = amount;
                    foreach (var itemX in suitableSizesX)
                    {
                        DoubleLinkedList<DataSideY> suitableSizesY = itemX.tree.ScanBetweenTwoValues(minDataY, maxDataY);
                        foreach (var itemY in suitableSizesY)
                        {
                            if (soldAmount == amount)
                            {
                                sold = true;
                                sb.AppendLine($"We sold to you {soldAmount} box/es.");
                                myShowMessage(sb.ToString());
                                break;
                            }
                            else if (itemY.quantity <= rest)
                            {
                                sb.AppendLine($"x={itemX.lengthX} y={itemY.lengthY} quantity={itemY.quantity}");
                                soldAmount = soldAmount + itemY.quantity;
                                RemoveFromTree(itemX, itemY, itemY.quantity);
                                rest = amount - soldAmount;
                            }
                            else
                            {
                                sb.AppendLine($"x={itemX.lengthX} y={itemY.lengthY} quantity={rest}");
                                soldAmount = soldAmount + rest;
                                RemoveFromTree(itemX, itemY, rest);
                                rest = amount - soldAmount;
                            }
                        }
                        if (soldAmount == amount)
                        {
                            sold = true;
                            sb.AppendLine($"We sold to you {soldAmount} box/es.");
                            myShowMessage(sb.ToString());
                            break;
                        }
                    }
                    if (!sold)
                    {
                        sb.AppendLine($"But we cant find {rest} box/es. So we sold to you only {soldAmount} box/es.");
                        myShowMessage(sb.ToString());
                    }
                }
                else if (!sold && isDividable && !isAcceptLess)
                {
                    soldAmount = 0;
                    int rest = amount;

                    foreach (var itemX in suitableSizesX)
                    {
                        DoubleLinkedList<DataSideY> suitableSizesY = itemX.tree.ScanBetweenTwoValues(minDataY, maxDataY);
                        foreach (var itemY in suitableSizesY)
                        {
                            soldAmount += itemY.quantity;
                            if (soldAmount >= amount)
                            {
                                sb.AppendLine("We have box/es like you need but with sorting.");
                                break;
                            }
                        }
                        if (soldAmount >= amount) break;
                    }
                    if (soldAmount < amount) myShowMessage($"Sorry we haven't {amount} box/es like you need.");
                    else
                    {
                        soldAmount = 0;
                        foreach (var itemX in suitableSizesX)
                        {
                            DoubleLinkedList<DataSideY> suitableSizesY = itemX.tree.ScanBetweenTwoValues(minDataY, maxDataY);
                            foreach (var itemY in suitableSizesY)
                            {
                                if (soldAmount == amount)
                                {
                                    sold = true;
                                    sb.AppendLine($"We sold to you {soldAmount} box/es like you wanted.");
                                    myShowMessage(sb.ToString());
                                    break;
                                }
                                if (itemY.quantity <= rest)
                                {
                                    sb.AppendLine($"x={itemX.lengthX} y={itemY.lengthY} quantity={itemY.quantity}");
                                    soldAmount += itemY.quantity;
                                    RemoveFromTree(itemX, itemY, itemY.quantity);
                                    rest = amount - soldAmount;
                                }
                                else
                                {
                                    sb.AppendLine($"x={itemX.lengthX} y={itemY.lengthY} quantity={rest}");
                                    soldAmount += rest;
                                    RemoveFromTree(itemX, itemY, rest);
                                    rest = amount - soldAmount;
                                }
                            }
                            if (soldAmount == amount)
                            {
                                sold = true;
                                sb.AppendLine($"We sold to you {soldAmount} box/es like you wanted.");
                                myShowMessage(sb.ToString());
                                break;
                            }
                        }
                    }
                }
                else if (!sold && !isDividable && isAcceptLess)
                {
                    int maxAmount = 0;
                    foreach (var itemX in suitableSizesX)
                    {
                        DoubleLinkedList<DataSideY> suitableSizesY = itemX.tree.ScanBetweenTwoValues(minDataY, maxDataY);
                        foreach (var itemY in suitableSizesY)
                        {
                            if (itemY.quantity > maxAmount) maxAmount = itemY.quantity;
                        }
                    }
                    {
                        foreach (var itemX in suitableSizesX)
                        {
                            DoubleLinkedList<DataSideY> suitableSizesY = itemX.tree.ScanBetweenTwoValues(minDataY, maxDataY);
                            foreach (var itemY in suitableSizesY)
                            {
                                if (itemY.quantity == maxAmount)
                                {
                                    sold = true;
                                    RemoveFromTree(itemX, itemY, maxAmount);
                                    myShowMessage($"We have only {maxAmount} box/es like you need (x={itemX.lengthX} y={itemY.lengthY}). You can take it/them.");
                                }
                                if (sold) break;
                            }
                            if (sold) break;
                        }
                    }
                }
                else
                {
                    if (!sold)
                        myShowMessage($"Sorry we haven't {amount} box/es like you need.");
                }
            }
        }
        //first searching -exact sises and exact amount
        private bool LokingExactSizeAndAmount(DataSideY minDataY, int amount, ref bool sold, DoubleLinkedList<DataSideX> suitableSizesX)
        {
            sold = false;
            foreach (var itemX in suitableSizesX)
            {
                DoubleLinkedList<DataSideY> suitableSizesY = itemX.tree.ScanBetweenTwoValues(minDataY, minDataY);
                foreach (var itemY in suitableSizesY)
                {
                    if (itemY.quantity < amount) return false;
                    else if (itemY.quantity >= amount)
                    {
                        sold = true;
                        if (itemY.quantity == amount)
                            myShowMessage($"We have last {amount} box/es (depth={itemX.lengthX}, height={itemY.lengthY}) for you. You can get it/them. We deleted this type of boxes from stock.");
                        else
                            myShowMessage($"We have  {amount} box/es for you. Also we have {itemY.quantity} more box/es (depth={itemX.lengthX}, height={itemY.lengthY}). If you need - you can buy it/them.");

                        RemoveFromTree(itemX, itemY, amount);
                    }
                    if (sold) break;
                }
                if (sold) break;
            }
            return sold;
        }
        //second searching - exact amount
        private bool LookingExactAmount(DataSideY minDataY, int amount, ref bool sold, DataSideY maxDataY, DoubleLinkedList<DataSideX> suitableSizesX)
        {
            sold = false;
            foreach (var itemX in suitableSizesX)
            {
                DoubleLinkedList<DataSideY> suitableSizesY = itemX.tree.ScanBetweenTwoValues(minDataY, maxDataY);
                foreach (var itemY in suitableSizesY)
                {
                    if (itemY.quantity < amount) return false;
                    else if (itemY.quantity >= amount)
                    {
                        sold = true;
                        if (itemY.quantity == amount)
                            myShowMessage($"We have last {amount} box/es (depth={itemX.lengthX}, height={itemY.lengthY}) for you. You can get it/them. We deleted this type of boxes from stock.");
                        else
                            myShowMessage($"We have  {amount} box/es for you. Also we have {itemY.quantity} more box/es (depth={itemX.lengthX}, height={itemY.lengthY}). If you need - you can buy it/them.");
                        RemoveFromTree(itemX, itemY, amount);
                    }
                    if (sold) break;
                }
                if (sold) break;
            }
            return sold;
        }
        private void RemoveFromTree(DataSideX x, DataSideY y, int amount)
        {
            if (mainTree.Search(x, out foundX))
            {
                if (foundX.tree.Search(y, out foundY))
                {
                    if (foundY.quantity >= amount)
                    {
                        if (foundY.quantity == amount)
                        {
                            RemoveNodeFromTimeQueue(foundY);
                            foundX.tree.Delete(foundY);
                            if (foundX.tree.root == null) mainTree.Delete(foundX);
                        }
                        if (foundY.quantity > amount)
                        {
                            RemoveNodeFromTimeQueue(foundY);
                            foundY.quantity -= amount;
                            foundY.refOfBoxTimeQueue = AddNodeToTimeQueue(x.lengthX, y.lengthY);
                        }
                    }
                }
            }
        }
        private DoubleLinkedList<DataTimeForXY>.Node AddNodeToTimeQueue(double x, double y)
        {
            DataTimeForXY newBox = new DataTimeForXY(x, y);
            boxTimeQueue.AddLast(newBox);
            return boxTimeQueue.end;
        }
        private void RemoveNodeFromTimeQueue(DataSideY y)
        {
            boxTimeQueue.RemoveAtAll(y.refOfBoxTimeQueue);
        }
        //removing old boxes after each 1 minute
        private void RemoveOldBoxes(object state)
        {
            bool isDelete = false;
            StringBuilder sb = new StringBuilder();
            DateTime a = DateTime.Now;

            if (boxTimeQueue.start == null)
            {
                sb.Append($"The stock is empty! We didn't have old boxes to remove.");
                myShowMessage(sb.ToString());
                return;
            }
            else
            {
                sb.Append("We removed such boxes:");
                while (boxTimeQueue.end != null && a.Subtract(boxTimeQueue.GetFirst().value.lastMovingTime) >= new TimeSpan(0, repeatCheckInMin, 0))
                {
                    DataSideX toRemoveX = new DataSideX(boxTimeQueue.GetFirst().value.x);
                    DataSideY toRemoveY = new DataSideY(boxTimeQueue.GetFirst().value.y);
                    if (mainTree.Search(toRemoveX, out foundX))
                    {
                        if (foundX.tree.Search(toRemoveY, out foundY))
                        {
                            sb.AppendLine($"{foundY.quantity} box/es with depth={foundX.lengthX}, height={foundY.lengthY}");
                            RemoveFromTree(foundX, foundY, foundY.quantity);
                            isDelete = true;
                        }
                    }
                    if (boxTimeQueue.start == null) break;
                }
                if (isDelete) myShowMessage(sb.ToString());
                else myShowMessage($"We didn't have old boxes to remove.");
            }
        }
        //show all boxes in stock info
        public void PrintAllBoxTypes()
        {
            StringBuilder sb = new StringBuilder();
            string s1 = $"We have in stock: {System.Environment.NewLine}";
            string s = "";
            mainTree.ScanInOrder((p) => p.tree.ScanInOrder((y) => s += $"x={p.lengthX.ToString()} y={y.lengthY.ToString()}" +
            $" amount={y.quantity.ToString()} time={y.refOfBoxTimeQueue.value.lastMovingTime.ToString()} {System.Environment.NewLine}"));
            if (s.Length == 0)
            {
                myShowMessage("The stock is empty!");
            }
            else
            {
                s = s1 + s;
                myShowMessage(s);
            }
        }
    }
}
