using HSeditor.Classes.Items;
using HSeditor.Classes.Other;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HSeditor.Windows
{
    public partial class EquipmentView : UserControl
    {
        Equipment Equipment;

        public EquipmentView(Equipment equipment)
        {
            InitializeComponent();
            this.Equipment = equipment;
            foreach (Border border in this.gridEquipment.Children)
            {
                if (border.Tag == null) continue;
                EquipmentSlot slot = MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipmentSlot(Convert.ToInt32(border.Tag));
                if (slot == null) continue;
                border.DataContext = slot;
            }
        }

        public void SetAllowDrop(Item? item)
        {
            foreach (Border border in gridEquipment.Children.OfType<Border>())
            {
                EquipmentSlot slot = border.DataContext as EquipmentSlot;
                bool enabled;

                enabled = item == null || item.Slot.ID == slot.Slot.ID;
                if (item != null && item.Slot.Name == "Weapon" && slot.Slot.Name == "Shield") enabled = true;

                border.AllowDrop = enabled;
                border.Opacity = enabled ? 1 : 0.5;
                if (slot.Item == null)
                    ((Image)border.Child).Opacity = 0.4;
            }
        }



        private Thickness GetTooltipPos(Border border, ItemTooltip toolTip)
        {
            toolTip.UpdateLayout();
            Point pos = border.TranslatePoint(new Point(0, 0), MainWindow.INSTANCE.MainGrid);
            Thickness thickness = new Thickness(pos.X, pos.Y, 0, 0);
            double x = (thickness.Left + (border.ActualWidth / 2)) - (toolTip.ActualWidth / 2);
            double y = thickness.Top - toolTip.ActualHeight - 5;
            if (y < 0)
                y = thickness.Top + border.ActualHeight + 5;


            return new Thickness(x, y, 0, 0);
        }

        private void ShowTooltip(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            Image image = border.Child as Image;
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
            Image image = border.Child as Image;
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
    }
}
