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

namespace HSeditor
{
    /// <summary>
    /// Interaktionslogik für ItemDisplay.xaml
    /// </summary>
    public partial class ItemDisplay : Window
    {
        Item Item { get; set; }
        List<ItemsControl> ItemControls = new List<ItemsControl>();
        TextBlock Damage;
        public ItemDisplay(Item item)
        {
            this.Owner = MainWindow.INSTANCE;
            InitializeComponent();
            this.Item = item.DeepCopy();
            this.mainBorder.DataContext = this.Item;
            //textBoxLevel.MaxLength = Item.Rarity.MaxUpgradeLevel.ToString().Length;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                int x = Int32.Parse(e.Text);
                int y = Int32.Parse(((TextBox)sender).Text + e.Text);
            }
            catch
            {
                e.Handled = true;
            }
        }

        private void textBoxUberAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == "")
                tb.Text = "1";

        }

        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste ||
                    e.Command == ApplicationCommands.ContextMenu)
            {
                e.Handled = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.UpdateStats();
        }

        private void ItemsControl_Initialized(object sender, EventArgs e)
        {
            this.ItemControls.Add((ItemsControl)sender);
        }

        private void textBoxLevel_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.UpdateStats();
        }

        void UpdateStats()
        {
            try
            {
                //if (textBoxLevel.Text == "" || textBoxQuality.Text == "" || ItemControls.Count == 0) return;
                //this.Item.Forge(Item.ItemLevel, Int32.Parse(textBoxQuality.Text), Int32.Parse(textBoxLevel.Text), Item.RollID, Item.Ability, Item.AbilityLevel, Item.Sockets.GetRuneList());
                //if (this.ItemControls[0] == null) return;
                //this.ItemControls[0].Items.Refresh();
                //this.ItemControls[1].Items.Refresh();
                //this.Damage.Text = Item.Stats.Damage.ToString();
            }
            catch { }

        }

        private void TextBlock_Initialized(object sender, EventArgs e)
        {
            this.Damage = (TextBlock)sender;
        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
