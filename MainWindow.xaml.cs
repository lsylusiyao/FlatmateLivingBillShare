using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SimpleExpressionEvaluator;

namespace FlatmateLivingBillShare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        /// <summary>
        /// all the flatmate names
        /// </summary>
        public ObservableCollection<NameCheck> NameChecks { set; get; }

        public string[] Names { set; get; }

        public ObservableCollection<Bill> Bills { set; get; } = new();

        private BillCalculation billCalc;

        public ObservableCollection<BillResult> Result { set; get; } = new();

        private ExpressionEvaluator engine = new(CultureInfo.CurrentCulture);

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            using var s = new FileStream("names.json", FileMode.Open);
            Names = JsonSerializer.Deserialize<string[]>(s);
            ArgumentNullException.ThrowIfNull(Names);
            NameChecks = new();
            foreach (var n in Names) NameChecks.Add(new NameCheck(n, false));
            billCalc = new(Names);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !NumberRegex().IsMatch(e.Text);
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Bills.Count > billsGrid.SelectedIndex)
                Bills.RemoveAt(billsGrid.SelectedIndex);
        }

        private void Button_Click(object sender, RoutedEventArgs e) // Submit button
        {
            if (priceTextBox.Text.Length == 0)
            {
                MessageBox.Show("Enter the price before submit.", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (payerComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Choose the payer before submit.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!NameChecks.Where(x => x.Check).Select(x => x.Name).Any())
            {
                MessageBox.Show("Choose the shared people before submit.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            decimal result;
            try
            {
                result = engine.Evaluate(priceTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Something wrong in the price. Check the expression first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Bills.Add(new Bill()
            {
                Item = itemTextBox.Text,
                Price = Convert.ToSingle(result),
                SharedPeople = NameChecks.Where(x => x.Check).Select(x => x.Name).ToArray(),
                Payer = payerComboBox.Text
            });
            itemTextBox.Text = string.Empty;
            priceTextBox.Text = string.Empty;
            // keep the choice of payer for convenience
            NameChecks.Clear();
            foreach (var n in Names) NameChecks.Add(new NameCheck(n, false));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // Calculate
        {
            Result.Clear();
            billCalc.Bills = Bills;
            var resultDict = billCalc.Calculate();
            foreach (var a in resultDict)
            {
                foreach (var b in a.Value)
                {
                    if (b.Value >= 0) Result.Add(new(b.Key, a.Key, b.Value));
                    else Result.Add(new(a.Key, b.Key, -b.Value));
                }
            }
            MessageBox.Show("Calculate Finish!");
            var sb = new StringBuilder();
            foreach (var n in Names)
            {
                sb.AppendLine($"{n} costs: {billCalc.EachMateCost[n]:F2}.");
            }
            MessageBox.Show(sb.ToString());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) //export
        {
            billCalc.Export();
            MessageBox.Show("Export finish.");
        }

        [GeneratedRegex("[0-9-.,+*/()]")]
        private static partial Regex NumberRegex();
    }
}