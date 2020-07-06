using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BOXwarehouseLogica;

namespace BOXwarehouseUI
{
    public partial class MainWindow : Window
    {
        Manager boxManager;
        bool acceptLess = true;
        bool dividable = true;
        public MainWindow()
        {
            InitializeComponent();
            boxManager = new Manager(ShowMessageToTextBox);
        }
        public void ShowMessageToTextBox(string message)
        {
            this.Dispatcher.Invoke(() =>
            {
            displayWindow.Text = message;
            });
        }
        private void addBOX_Click(object sender, RoutedEventArgs e)
        {

            if ((!double.TryParse(addSideX.Text, out double valX)) || (!double.TryParse(addSideY.Text, out double valY)) ||
                (!int.TryParse(addAmount.Text, out int valAmount)))
            {
                displayWindow.Text = $"Input in side X or Y is not double or amount not int! Change the input, please.";
            }
            else
            {
                double.TryParse(addSideX.Text, out double valueX);
                double.TryParse(addSideY.Text, out double valueY);
                int.TryParse(addAmount.Text, out int amountToAdd);
                boxManager.AddBox(valueX, valueY, amountToAdd);
            }

        }
        private void buyAcceptLess_Click(object sender, RoutedEventArgs e)
        {
            acceptLess = !acceptLess;
        }
        private void buyDividable_Click(object sender, RoutedEventArgs e)
        {
            dividable = !dividable;
        }
        private void buyBOX_Click(object sender, RoutedEventArgs e)
        {

            if ((!double.TryParse(buySideX.Text, out double valX)) || (!double.TryParse(buySideY.Text, out double valY)) ||
                (!int.TryParse(buyAmount.Text, out int valAmount)) || (!double.TryParse(buyIncreaseX.Text, out double incrX)) ||
                (!double.TryParse(buyIncreaseY.Text, out double incrY)))
            {
                displayWindow.Text = $"Input in side X, Y, increasing X or increasing Y is not double or amount not int! Change the input, please.";
            }
            else
            {
                double.TryParse(buySideX.Text, out double valueX);
                double.TryParse(buySideY.Text, out double valueY);
                int.TryParse(buyAmount.Text, out int amount);
                double.TryParse(buyIncreaseX.Text, out double increaseX);
                double.TryParse(buyIncreaseY.Text, out double increaseY);
                if (dividable && acceptLess)
                    boxManager.SellBoxes(valueX, valueY, amount, increaseX, increaseY, true, true);
                else if (dividable && !acceptLess)
                    boxManager.SellBoxes(valueX, valueY, amount, increaseX, increaseY, true, false);
                else if (!dividable && acceptLess)
                    boxManager.SellBoxes(valueX, valueY, amount, increaseX, increaseY, false, true);
                else
                    boxManager.SellBoxes(valueX, valueY, amount, increaseX, increaseY, false, false);
            }

        }
        private void infoBOX_Click(object sender, RoutedEventArgs e)
        {
            if ((!double.TryParse(infoSizeX.Text, out double valX)) || (!double.TryParse(infoSizeY.Text, out double valY)))
                displayWindow.Text = $"Input in side X, Y is not double! Change the input, please.";
            else
            {
                double.TryParse(infoSizeX.Text, out double valueX);
                double.TryParse(infoSizeY.Text, out double valueY);
                boxManager.ShowBoxInfo(valueX, valueY);
            }
        }
        private void stockButton_Click(object sender, RoutedEventArgs e)
        {
            boxManager.PrintAllBoxTypes();
        }
    }
}
