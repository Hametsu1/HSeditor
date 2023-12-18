using HSeditor.Classes.Other;
using HSeditor.Classes.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaktionslogik für RunewordBaseSelection.xaml
    /// </summary>
    public partial class RunewordBaseSelection : UserControl
    {
        Item Runeword;
        DragHandler.Origin Origin;
        List<Item> Bases;
        List<RwType> Types;
        public RunewordBaseSelection(Item runeword, DragHandler.Origin origin)
        {
            InitializeComponent();
            this.Runeword = runeword;
            this.Origin = origin;
            this.GetBases();
            this.DataContext = this.Runeword;
            MainWindow.INSTANCE.ShowPopup(this);
        }

        private void GetBases()
        {
            int socketCount = Runeword.Sockets.GetRuneList().FindAll(o => o.ID != 0).Count;
            List<Item> bases = new List<Item>();
            MainWindow.INSTANCE.ItemHandler.Runewords.FindAll(o => o.RunewordID == Runeword.RunewordID).ForEach(rw =>
            {
                MainWindow.INSTANCE.ItemHandler.Generics.FindAll(o => o.Slot.ID == rw.Slot.ID && o.WeaponType.ID == rw.WeaponType.ID && o.MaxSockets == socketCount).ForEach(o => bases.Add(o));
            });
            bases = bases.OrderByDescending(o => o.Slot.ID == 3 ? o.WeaponType.ID == Runeword.WeaponType.ID : o.Slot.ID == Runeword.Slot.ID).ThenBy(o => o.Slot.ID).ThenBy(o => o.WeaponType.ID).ToList();
            this.Bases = bases;
        }

        private void listboxBases_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private Thickness GetTooltipPos(Border border, ItemTooltip toolTip)
        {
            toolTip.UpdateLayout();
            Point pos = border.TranslatePoint(new Point(0, 0), MainWindow.INSTANCE.toolTipGrid);
            double x = pos.X - toolTip.ActualWidth - 5;
            double y = pos.Y + (border.ActualHeight / 2) - (toolTip.ActualHeight / 2);

            if (y < 0) y = 0;
            else if ((y + toolTip.ActualHeight) > MainWindow.INSTANCE.toolTipGrid.ActualHeight) y = MainWindow.INSTANCE.toolTipGrid.ActualHeight - toolTip.ActualHeight;

            return new Thickness(x, y, 0, 0);
        }

        private void ShowTooltip(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;

            Item item = border.DataContext as Item;
            if (item == null) return;

            ItemTooltip toolTip = new ItemTooltip(item);
            MainWindow.INSTANCE.toolTipGrid.Children.Add(toolTip);
            toolTip.VerticalAlignment = VerticalAlignment.Top;
            toolTip.HorizontalAlignment = HorizontalAlignment.Left;
            toolTip.Margin = this.GetTooltipPos(border, toolTip);
            border.Tag = toolTip;
            toolTip.IsHitTestVisible = false;
        }

        private void HideTooltip(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            MainWindow.INSTANCE.toolTipGrid.Children.Remove((ItemTooltip)border.Tag);
        }

        private void Close()
        {
            MainWindow.INSTANCE.ClosePopup();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (border == null) return;

            Item item = border.DataContext as Item;
            if (item == null) return;

            Runeword.Sprite = item.Sprite;
            Runeword.Size = item.Size;
            Runeword.Tier = item.Tier;
            Runeword.Hands = item.Hands;
            Runeword.ID = item.ID;
            Runeword.Slot = item.Slot;
            Runeword.WeaponType = item.WeaponType;

            switch (Origin)
            {
                case DragHandler.Origin.Inventory:
                    MainWindow.INSTANCE.UpdateInventory();
                    break;
                case DragHandler.Origin.Equip:
                    MainWindow.INSTANCE.UpdateEquippedItems();
                    break;
                case DragHandler.Origin.Stash:
                    MainWindow.INSTANCE.UpdateStash();
                    break;
            }
            this.Close();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid sp = sender as Grid;
            Grid grid = ((Item)sp.DataContext).SizeGrid;
            grid.HorizontalAlignment = HorizontalAlignment.Right;
            grid.VerticalAlignment = VerticalAlignment.Bottom;
            grid.Margin = new Thickness(0, 0, 2, 2);
            sp.Children.Add(grid);
        }

        private void listboxTypes_Loaded(object sender, RoutedEventArgs e)
        {
            List<RwType> types = new List<RwType>();
            this.Bases.ForEach(o =>
            {
                string name = o.Slot.ID == 3 ? o.WeaponType.Name : o.Slot.Name;
                if (types.Find(o2 => o2.Name == name) == null) types.Add(new RwType(name, o.Slot.ID == 3));
            });
            this.Types = types;
            if (Types.Count() > 0)
            {
                this.Types[0].isSelected = true;
                this.UpdateBases(this.Types[0].Name);
            }
            listboxTypes.ItemsSource = this.Types;
        }

        private void UpdateBases(string typename)
        {
            listboxBases.ItemsSource = this.Bases.FindAll(baseItem =>
            {
                return (baseItem.Slot.ID == 3 && typename == baseItem.WeaponType.Name) || (baseItem.Slot.ID != 3 && typename == baseItem.Slot.Name);
            });
        }

        class RwType
        {
            public string Name { get; set; }
            public bool isWeapon { get; set; }
            public bool isSelected { get; set; }

            public RwType(string Name, bool isWeapon)
            {
                this.Name = Name;
                this.isWeapon = isWeapon;
                this.isSelected = false;
            }
        }

        private void Border_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (border == null) return;

            RwType rwtype = (RwType)border.DataContext;
            if (rwtype == null) return;

            this.Types.ForEach(o => o.isSelected = false);
            rwtype.isSelected = true;
            listboxTypes.Items.Refresh();
            this.UpdateBases(rwtype.Name);
        }
    }
}
