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

namespace HSeditor.Windows
{
    /// <summary>
    /// Interaktionslogik für ContextMenuItemlist.xaml
    /// </summary>
    public partial class ContextMenuItemlist : ContextMenu
    {
        private double startHeight;
        private double startWidth;
        Item item;
        bool stash;
        bool set;

        public ContextMenuItemlist(Item item)
        {
            InitializeComponent();
            this.item = item;
            this.stash = (MainWindow.INSTANCE.Stash != null && MainWindow.INSTANCE.Stash.WindowState == WindowState.Maximized);
            this.set = this.item.Set != null && this.item.Set.ID != -1;
            this.DataContext = this.item;
        }



        private void Open(object sender, RoutedEventArgs e)
        {
            this.UpdateLayout();
            if (this.startHeight == 0) this.startHeight = this.ActualHeight;
            if (this.startWidth == 0) this.startWidth = this.ActualWidth;

            this.Width = this.startWidth * MainWindow.INSTANCE.SizeFactor;
            this.Height = this.startHeight * MainWindow.INSTANCE.SizeFactor;
        }

        private void ContextMenu_Equip_Loaded(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.Visibility = MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipmentSlotFromItem(item) == null ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ContextMenu_Add_Loaded(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.Content = stash ? "Add to Stash" : "Add to Inventory";
        }

        private void ContextMenu_Fill_Loaded(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.Content = stash ? "Fill Stash" : "Fill Inventory";
        }

        private void ContextMenu_AddAll_Loaded(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.Content = stash ? "Add all to Stash" : "Add all to Inventory";
            btn.Margin = set ? btn.Margin : new Thickness(5);
        }

        private void ContextMenu_AddSet_Loaded(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.Visibility = this.item.Set != null && this.item.Set.ID != -1 ? Visibility.Visible : Visibility.Collapsed;
            btn.Content = stash ? "Add Set to Stash" : "Add Set to Inventory";
        }

        private void ContextMenu_EquipSet_Loaded(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.Visibility = set ? Visibility.Visible : Visibility.Collapsed;
            btn.Content = stash ? "Equip Set" : "Equip Set";
        }
    }
}
