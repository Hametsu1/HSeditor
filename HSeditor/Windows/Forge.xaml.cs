using HSeditor.Classes.Items;
using HSeditor.Classes.Util;
using Newtonsoft.Json.Linq;
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
using System.Windows.Shapes;

namespace HSeditor.Windows
{
    /// <summary>
    /// Interaktionslogik für Forge.xaml
    /// </summary>
    public partial class Forge : UserControl
    {
        Item item;
        Item temp;
        ItemTooltip before;
        WindowState stashState;
        bool loaded = false;

        List<ComboBox> comboBoxes;
        public Forge(Item item)
        {
            this.item = item;
            this.item.SaveItem = this.item.GetItemObject();
            this.temp = item.DeepCopy();
            InitializeComponent();
            MainWindow.INSTANCE.ShowPopup(this);
            before = new ItemTooltip(temp);
            before.VerticalAlignment = VerticalAlignment.Center;
            before.HorizontalAlignment = HorizontalAlignment.Center;
            SetTooltip();
            comboBoxes = new List<ComboBox> { cbRune1, cbRune2, cbRune3, cbRune4, cbRune5, cbRune6 };
            SetRunes();
            if (MainWindow.INSTANCE.Stash != null)
            {
                stashState = MainWindow.INSTANCE.Stash.WindowState;
                MainWindow.INSTANCE.Stash.WindowState = WindowState.Minimized;
            }
        }

        public void SetTooltip()
        {
            spTooltip.Children.Clear();
            spTooltip.Children.Add(before);
            spTooltip.Children.Add(new Image { Source = new BitmapImage(new Uri(@"pack://application:,,,/HSeditor;component/Resources/arrowDown.png")), Height = 18, Width = 18, Margin = new Thickness(0, 8, 0, 8) });
            spTooltip.Children.Add(temp == null ? before : new ItemTooltip(temp));
        }

        private void Close()
        {
            if (MainWindow.INSTANCE.Stash != null)
            {
                MainWindow.INSTANCE.Stash.WindowState = stashState;
            }
            MainWindow.INSTANCE.ClosePopup();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tbSource_Loaded(object sender, RoutedEventArgs e)
        {
            tbSource.Text = temp.GetItemObject().ToString(Newtonsoft.Json.Formatting.Indented);
        }

        private void tbSource_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                JObject item2 = JObject.Parse(tbSource.Text);
                temp = MainWindow.INSTANCE.ItemHandler.ParseJSONObject(item2);
                temp.SaveItem = item2;
                SetTooltip();
                tbSource.Foreground = new SolidColorBrush(Colors.White);
            }
            catch
            {
                tbSource.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void ApplyProperties()
        {
            List<Item> items = new List<Item> { item };
            if (cbApplyTo.SelectedItem == "All" || cbApplyTo.SelectedItem == "Inventory")
                items.AddRange(MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.InventoryItems);
            if (cbApplyTo.SelectedItem == "All" || cbApplyTo.SelectedItem == "Equipment")
                items.AddRange(MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.Equipment.GetItems());
            if (cbApplyTo.SelectedItem == "All" || cbApplyTo.SelectedItem == "Stash")
                items.AddRange(MainWindow.INSTANCE.SaveFileHandler.Shop.Stash);

            List<string> properties = new List<string>();
            foreach (var property in temp.SaveItem)
            {
                if (!item.SaveItem.ContainsKey(property.Key) || item.SaveItem[property.Key].ToString() != temp.SaveItem[property.Key].ToString()) properties.Add(property.Key);
            }

            items.ForEach(item2 =>
            {
                properties.ForEach(property => item2.SaveItem[property] = temp.SaveItem[property]);
                item2.UpdateData();
            });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ApplyProperties();
            MainWindow.INSTANCE.UpdateInventory();
            MainWindow.INSTANCE.UpdateEquippedItems();
            MainWindow.INSTANCE.UpdateStash();
            this.Close();
        }


        private void SetRunes()
        {
            List<Rune> runes = MainWindow.INSTANCE.RuneHandler.Runes.OrderBy(o => o.ID != 0).ThenBy(o => o.Name).ToList();
            cbRuneAll.ItemsSource = runes;
            comboBoxes.ForEach(cb =>
            {
                cb.ItemsSource = runes;
                int cbIndex = Convert.ToInt32(cb.Tag);
                Rune rune = runes.Find(o => o.ID == item.Sockets.GetRuneList()[cbIndex].ID);
                cb.SelectedItem = rune;
            });
        }

        private void UpdateRunes()
        {
            List<Rune> runes = MainWindow.INSTANCE.RuneHandler.Runes;
            comboBoxes.ForEach(cb =>
            {
                cb.SelectedItem = runes.Find(o => o.ID == (int)temp.SaveItem[$"socket_{Convert.ToInt32(cb.Tag) + 1}"]);
            });
        }

        private void comboBoxClasses_Loaded(object sender, RoutedEventArgs e)
        {



        }

        private void comboBoxClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            Rune rune = cb.SelectedItem as Rune;
            if (rune.ID == temp.Sockets.GetRuneList()[Convert.ToInt32(cb.Tag)].ID) return;
            temp.SaveItem[$"socket_{Convert.ToInt32(cb.Tag) + 1}"] = rune.ID;
            temp.Sockets.SetRune(Convert.ToInt32(cb.Tag), rune);
            SetTooltip();
        }

        private void wpRunes_Loaded(object sender, RoutedEventArgs e)
        {
            this.Update();
        }

        private void cbRuneAll_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            Rune rune = cb.SelectedItem as Rune;
            for (int i = 1; i <= 6; i++) temp.SaveItem[$"socket_{i}"] = rune.ID;
            temp.Sockets = new Sockets(new List<Rune> { rune, rune, rune, rune, rune, rune });
            Update();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void cbApplyTo_Loaded(object sender, RoutedEventArgs e)
        {
            cbApplyTo.ItemsSource = new List<string> { "Selected", "Equipment", "Inventory", "Stash", "All" };
            cbApplyTo.SelectedIndex = 0;
        }

        private void Update()
        {
            this.UpdateRunes();
            textBoxSeed.Text = temp.SaveItem.ContainsKey("seed") ? temp.SaveItem["seed"].ToString() : temp.RollID.ToString();
            textBoxAmount.Text = temp.SaveItem.ContainsKey("amount") ? temp.SaveItem["amount"].ToString() : "1";
            SetTooltip();
        }

        private void textBoxSeed_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void textBoxSeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool parse = int.TryParse(textBoxSeed.Text, out int ignore);
            int roll = parse ? Convert.ToInt32(textBoxSeed.Text) : 0;
            if (!temp.SaveItem.ContainsKey("seed")) temp.SaveItem.Add(roll);
            else temp.SaveItem["seed"] = roll;
            temp.RollID = roll;
            this.SetTooltip();
        }

        private void textBoxAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!temp.SaveItem.ContainsKey("amount")) temp.SaveItem.Add("amount", textBoxAmount.Text);
            else temp.SaveItem["amount"] = textBoxAmount.Text;
            this.SetTooltip();
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TooltipHandler.SetToolTip((TextBlock)sender);
        }

        private void TextBlock_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        private void Viewbox_Loaded(object sender, RoutedEventArgs e)
        {
            Viewbox vb = sender as Viewbox;
            Border border = vb.Child as Border;
            vb.Width = border.ActualWidth * MainWindow.INSTANCE.SizeFactor;
            vb.Height = border.ActualHeight * MainWindow.INSTANCE.SizeFactor;
        }
    }
}
