using HSeditor.Classes.Items;
using HSeditor.Classes.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HSeditor.Windows
{
    public partial class EquipmentView : UserControl
    {
        Equipment Equipment;
        Dictionary<Image, int> Images = new Dictionary<Image, int>();

        public static Point WeaponSize = new Point(110, 130);
        public static Point ArmorSize = new Point(80, 100);
        public static Point ShieldSize = new Point(110, 130);
        public static Point GloveSize = new Point(80, 80);
        public static Point BootSize = new Point(80, 80);
        public static Point HelmetSize = new Point(80, 80);
        public static Point RingSize = new Point(50, 50);
        public static Point BeltSize = new Point(80, 50);
        public static Point RelicSize = new Point(50, 50);
        public static Point AmuletSize = new Point(50, 50);
        public static Point CharmSize = new Point(80, 80);
        public static Point Default = new Point(80, 80);

        public static Point GetSize(int slotid)
        {
            List<Point> points = new List<Point> { HelmetSize, ArmorSize, BootSize, WeaponSize, GloveSize, AmuletSize, ShieldSize, RingSize, BeltSize, CharmSize, Default, Default, Default, Default, Default, RelicSize };
            if (slotid >= points.Count) return points.Last();
            else return points[slotid];
        }

        public EquipmentView(Equipment equipment)
        {
            InitializeComponent();
            this.Equipment = equipment;
            foreach (FrameworkElement border in this.gridEquipment.Children)
            {
                if (border.Tag == null) continue;
                EquipmentSlot slot = MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipmentSlot(Convert.ToInt32(border.Tag));
                if (slot == null) continue;
                border.DataContext = slot;
            }
        }


        private void HideRunes()
        {
            foreach (ItemsControl sp in gridEquipment.Children.OfType<ItemsControl>()) sp.Visibility = Visibility.Collapsed;
        }

        private void ShowRunes()
        {
            foreach (EquipmentSlot slot in Equipment.GetEquipment().FindAll(o => o.Item != null))
            {
                ItemsControl panel = gridEquipment.Children.OfType<ItemsControl>().ToList().Find(o => o.Tag.ToString() == slot.ID.ToString());
                if (panel == null) continue;

                panel.ItemsSource = null;
                panel.ItemsSource = slot.Item.Sockets.Runes;
                panel.Visibility = Visibility.Visible;
            }
        }

        public void SetAllowDrop(Item? item)
        {
            HideRunes();
            foreach (Border border in gridEquipment.Children.OfType<Border>())
            {
                EquipmentSlot slot = border.DataContext as EquipmentSlot;

                bool allowDrop = false;
                bool reduceOpacity = true;

                if (item == null || item.Slot.ID == slot.Slot.ID)
                {
                    allowDrop = true;
                    reduceOpacity = false;
                }

                // Allow weapons in shield slot
                if (item != null && item.Slot.Name == "Weapon" && slot.Slot.Name == "Shield")
                {
                    allowDrop = true;
                    reduceOpacity = false;
                }

                // Allow socketables in a slots if an item is equipped
                // Enable rune preview
                if (item != null && item.Slot.Name == "Socketable" && slot.Item != null)
                {
                    allowDrop = true;
                    reduceOpacity = true;
                    this.ShowRunes();
                }

                border.AllowDrop = allowDrop;
                FindImage(slot.ID).Opacity = reduceOpacity ? 0.3 : 1;
                border.Opacity = reduceOpacity ? 0.3 : 1;
                if (slot.Item == null)
                    FindImage(Convert.ToInt32(border.Tag)).Opacity = 0.4;
            }
        }

        public Image FindImage(int tag)
        {
            if (Images.Count == 0)
            {
                gridEquipment.Children.OfType<Image>().ToList().ForEach(o => Images.Add(o, Convert.ToInt32(o.Tag)));
            }
            foreach (var img in Images)
                if (img.Value == tag) return img.Key;

            return null;
        }

        public Image FindImage(object tag)
        {
            return FindImage(Convert.ToInt32(tag));
        }



        private Thickness GetTooltipPos(Border border, ItemTooltip toolTip)
        {
            toolTip.UpdateLayout();
            Point pos = border.TranslatePoint(new Point(0, 0), MainWindow.INSTANCE.MainGrid);
            Thickness thickness = new Thickness(pos.X, pos.Y, 0, 0);
            double x = (thickness.Left + (border.ActualWidth / 2)) - (toolTip.ActualWidth / 2);
            double y = thickness.Top - toolTip.ActualHeight - 5;
            //if (y < 0)
            //   y = thickness.Top + border.ActualHeight + 5;

            if (y < 0)
            {
                double diffTop = Math.Abs(y);
                y = thickness.Top + border.ActualHeight + 5;
                if (y + toolTip.ActualHeight > MainWindow.INSTANCE.toolTipGrid.ActualHeight)
                {
                    double diffBottom = Math.Abs(y + toolTip.ActualHeight - MainWindow.INSTANCE.toolTipGrid.ActualHeight);
                    y = diffBottom < diffTop ? MainWindow.INSTANCE.toolTipGrid.ActualHeight - toolTip.ActualHeight : 0;
                }
            }


            return new Thickness(x, y, 0, 0);
        }

        private void ShowTooltip(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            Image image = FindImage(border.Tag);
            if (border.Tag == null) return;
            EquipmentSlot slot = MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipmentSlot(Convert.ToInt32(border.Tag));
            if (slot == null || slot.Item == null) return;

            ItemTooltip toolTip = new ItemTooltip(slot.Item);
            MainWindow.INSTANCE.toolTipGrid.Children.Add(toolTip);
            toolTip.VerticalAlignment = VerticalAlignment.Top;
            toolTip.HorizontalAlignment = HorizontalAlignment.Left;
            toolTip.Margin = this.GetTooltipPos(border, toolTip);
            image.Tag = toolTip;
            toolTip.IsHitTestVisible = false;
        }

        private void HideTooltip(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            if (border.Tag == null) return;
            Image image = FindImage(border.Tag);
            if (image.Tag is ItemTooltip)
                MainWindow.INSTANCE.toolTipGrid.Children.Remove((ItemTooltip)image.Tag);
        }

        private void equipmentSlotStartDrag(object sender, MouseButtonEventArgs e)
        {
            MainWindow.INSTANCE.equipmentSlotStartDrag(sender, e);
        }

        private void equipmentSlotDragOver(object sender, DragEventArgs e)
        {
            MainWindow.INSTANCE.equipmentSlotDragOver(sender, e);
        }

        private void equipmentSlotDragLeave(object sender, DragEventArgs e)
        {
            MainWindow.INSTANCE.equipmentSlotDragLeave(sender, e);
        }

        private void equipmentSlotDragDrop(object sender, DragEventArgs e)
        {
            MainWindow.INSTANCE.equipmentSlotDragDrop(sender, e);
        }

        private void Grid_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            MainWindow.INSTANCE.Grid_GiveFeedback(sender, e);
        }

        private void equipmentLoaded(object sender, RoutedEventArgs e)
        {
            MainWindow.INSTANCE.equipmentLoaded(sender, e);
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            Image img = sender as Image;
            if (!this.Images.ContainsKey(img))
                this.Images.Add(img, Convert.ToInt32(img.Tag));
        }

        private void Image_DragOver(object sender, DragEventArgs e)
        {
            Image image = sender as Image;
            image.Height = 28;
            image.Width = 28;
            image.Margin = new Thickness(3.5);
        }

        private void Image_DragLeave(object sender, DragEventArgs e)
        {
            Image image = sender as Image;
            image.Height = 25;
            image.Width = 25;
            image.Margin = new Thickness(5);
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            Image image = sender as Image;
            if (image == null) return;

            ItemDrag drag = e.Data.GetData(typeof(ItemDrag)) as ItemDrag;
            if (drag == null) return;

            Rune rune = image.DataContext as Rune;
            if (rune == null) return;

            rune.ID = drag.Item.ID;
            rune.Name = drag.Item.Name;
            rune.Sprite = drag.Item.Sprite;
            MainWindow.INSTANCE.UpdateEquippedItems();
        }
    }
}
