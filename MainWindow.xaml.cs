using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            using var s = new FileStream("names.json", FileMode.Open);
            Names = JsonSerializer.Deserialize<string[]>(s);
            ArgumentNullException.ThrowIfNull(Names);
            NameChecks = new();
            foreach (var n in Names) NameChecks.Add(new NameCheck(n, false));
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "[0-9.eE_,]");
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Bills.Count > billsGrid.SelectedIndex)
                Bills.RemoveAt(billsGrid.SelectedIndex);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Bills.Add(new Bill()
            {
                Item = itemTextBox.Text,
                Price = float.Parse(priceTextBox.Text),
                SharedPeople = NameChecks.Where(x => x.Check).Select(x => x.Name).ToArray(),
                Payer = payerComboBox.Text
            });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}