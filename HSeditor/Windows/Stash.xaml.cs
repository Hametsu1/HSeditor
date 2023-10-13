using HSeditor.Classes.Other;
using HSeditor.Classes.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HSeditor.Windows
{
    /// <summary>
    /// Interaktionslogik für Stash.xaml
    /// </summary>
    public partial class Stash : Window
    {

        public double SizeFactor { get { return Util.GetScaleFactor(vbMain); } }
        public Stash()
        {
            InitializeComponent();
            this.Owner = MainWindow.INSTANCE;
        }


        public void Update()
        {
            if (MainWindow.INSTANCE.SaveFileHandler == null || MainWindow.INSTANCE.SaveFileHandler.SelectedFile == null) return;

            MainWindow.INSTANCE.InventoryBoxHandler.ClearSelectedItems();
            gridInvMainImages.Children.Clear();
            MainWindow.INSTANCE.InventoryBoxHandler.StashBoxes.ForEach(o => o.Item = null);

            string searchText = "";
            if (textBoxStashSearch.Text != "Search..." && textBoxStashSearch.Text != "") searchText = textBoxStashSearch.Text.ToLower().Replace(" ", String.Empty).Replace("@", String.Empty);

            foreach (Item item in MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.StashItems.Concat(MainWindow.INSTANCE.SaveFileHandler.Shop.Stash))
            {
                bool matchesFilter = textBoxStashSearch.Text.StartsWith("@") ? item.Slot.Name.ToLower().Replace(" ", String.Empty).Contains(searchText) : item.Name.ToLower().Replace(" ", String.Empty).Contains(searchText);
                if (item.Inv != MainWindow.INSTANCE.SelectedStash || (MainWindow.INSTANCE.activeDrag && MainWindow.INSTANCE.previewDrag != null && item == MainWindow.INSTANCE.previewDrag.Item)) continue;
                Point end = new Point(item.InvPos.Value.X + (item.Size.X - 1), item.InvPos.Value.Y + (item.Size.Y - 1));
                List<InventoryBox> boxes = MainWindow.INSTANCE.InventoryBoxHandler.GetInventoryBoxesInMatrix(item.InvPos.Value, end, MainWindow.INSTANCE.InventoryBoxHandler.StashBoxes);
                if (boxes.Count == 0) continue;
                if (boxes.Find(o => o.Item != null) != null) continue;

                Border border = MainWindow.INSTANCE.InventoryBoxHandler.GetImageOverlay(item, boxes);
                if (!matchesFilter) border.Opacity = 0.2;
                Image img = (Image)border.Child;
                border.Child = null;

                boxes.ForEach(o =>
                {
                    o.Item = item;
                    o.InvImage = border;
                });

                border.PreviewMouseLeftButtonDown += invStartDrag;
                border.GiveFeedback += Grid_GiveFeedback;
                border.ContextMenu = new ContextMenuInv(item, DragHandler.Origin.Stash, null);
                border.MouseEnter += ShowTooltip;
                border.MouseLeave += HideTooltip;

                Grid grid = new Grid { Width = border.Width, Height = border.Height };
                grid.Children.Add(img);
                if (item.Slot.ShowAmount && (int)item.SaveItem["amount"] > 1) grid.Children.Add(new Label { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Foreground = Util.ColorFromString("#FFFFFF"), FontSize = 10, FontWeight = FontWeights.Bold, Content = item.SaveItem["amount"], Margin = new Thickness(0, 0, 0, -3) });

                border.Child = grid;
                item.InvImage = border;
                item.Fields = boxes;

                gridInvMainImages.Children.Add(border);
            }
            if (labelStashCount != null) labelStashCount.Content = MainWindow.INSTANCE.SaveFileHandler.Shop.Stash.Count;
        }

        private Thickness GetTooltipPos(Border border, ItemTooltip toolTip)
        {
            toolTip.UpdateLayout();
            double x = (border.Margin.Left + (border.ActualWidth / 2)) - (toolTip.ActualWidth / 2);
            double y = border.Margin.Top - (toolTip.ActualHeight + 5);
            if (y < 0)
                y = border.Margin.Top + border.ActualHeight + 5;

            if ((x + toolTip.ActualWidth) > gridInvMain.ActualWidth)
            {
                x = 0;
                toolTip.HorizontalAlignment = HorizontalAlignment.Right;
            }

            if (x < 0)
            {
                x = 0;
                toolTip.HorizontalAlignment = HorizontalAlignment.Left;
            }

            return new Thickness(x, y, border.Margin.Right, border.Margin.Bottom);
        }

        private void ShowTooltip(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            Image image = (Image)(border.Child as Grid).Children[0];

            Item item = border.DataContext as Item;
            if (item == null) return;

            ItemTooltip toolTip = new ItemTooltip(item);
            gridInvMainImages.Children.Add(toolTip);
            toolTip.VerticalAlignment = VerticalAlignment.Top;
            toolTip.HorizontalAlignment = HorizontalAlignment.Left;
            toolTip.Margin = this.GetTooltipPos(border, toolTip);
            image.Tag = toolTip;
            toolTip.IsHitTestVisible = false;
        }

        private void HideTooltip(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            Image image = (Image)(border.Child as Grid).Children[0];
            gridInvMainImages.Children.Remove((ItemTooltip)image.Tag);
        }



        public void invStartDrag(object sender, MouseButtonEventArgs e)
        {
            Border border = (Border)sender;
            Item item = (Item)border.DataContext;
            if (item == null) return;

            if (e.ClickCount > 1)
            {
                EquipmentSlot slot = MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipmentSlotFromItem(item);
                if (slot == null) return;
                Item old = slot.Item;
                MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.Equipment.EquipItem(item.DeepCopy(), slot.ID);
                MainWindow.INSTANCE.SaveFileHandler.Shop.Stash.Remove(item);
                if (old != null)
                {
                    old.Inv = MainWindow.INSTANCE.SelectedStash;
                    MainWindow.INSTANCE.InventoryBoxHandler.StashBoxes.FindAll(o => o.Item == item).ForEach(o => o.Item = null);
                    InventoryBox box = MainWindow.INSTANCE.InventoryBoxHandler.FindSpace(old, MainWindow.INSTANCE.InventoryBoxHandler.StashBoxes);
                    if (box != null)
                    {
                        old.InvPos = box.Position;
                        MainWindow.INSTANCE.SaveFileHandler.Shop.Stash.Add(old);
                    }
                }
                MainWindow.INSTANCE.UpdateEquippedItems();
                MainWindow.INSTANCE.UpdateStash();
                return;
            }

            Point end = new Point(item.InvPos.Value.X + (item.Size.X - 1), item.InvPos.Value.Y + (item.Size.Y - 1));
            List<InventoryBox> boxes = MainWindow.INSTANCE.InventoryBoxHandler.GetInventoryBoxesInMatrix(item.InvPos.Value, end, MainWindow.INSTANCE.InventoryBoxHandler.StashBoxes);

            ItemDrag drag = new ItemDrag(null, item, DragHandler.Origin.Stash);
            drag.CurrentFields = boxes;
            drag.sender = sender as Border;
            drag.oriPos = e.GetPosition(this);
            MainWindow.INSTANCE.previewDrag = drag;
        }

        public void Grid_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            MainWindow.INSTANCE.Grid_GiveFeedback(sender, e);
        }

        private void BuildStash()
        {
            if (this.gridInvMain == null) return;
            MainWindow.INSTANCE.InventoryBoxHandler.ClearSelectedItems();
            this.gridInvMain.Children.Clear();
            this.gridInvMainImages.Children.Clear();
            MainWindow.INSTANCE.InventoryBoxHandler.StashBoxes.Clear();
            int height = (int)MainWindow.INSTANCE.InventoryBoxHandler.StashSize.Y;
            int width = (int)MainWindow.INSTANCE.InventoryBoxHandler.StashSize.X;
            gridInvMain.Width = width * 40;
            gridInvMainImages.Width = gridInvMain.Width;
            for (int i = 0; i < height; i++)
            {
                for (int x = 0; x < width; x++)
                {
                    Border grid = MainWindow.INSTANCE.InventoryBoxHandler.GetBoxBorder();

                    grid.DragEnter += invDragOver;
                    grid.Drop += invDragDrop;
                    grid.DragLeave += invDragLeave;
                    gridInvMain.Children.Add(grid);
                    InventoryBox inventoryBox = new InventoryBox(new Point(x, i), null, grid, InventoryBoxHandler.BoxType.Item);
                    grid.DataContext = inventoryBox;
                    MainWindow.INSTANCE.InventoryBoxHandler.StashBoxes.Add(inventoryBox);
                }
            }
            this.Update();
        }

        private void gridInvMain_Loaded(object sender, RoutedEventArgs e)
        {
            if (gridInvMain.Children.Count != 0) return;
            this.BuildStash();
        }

        public bool PlaceStash(Point start, Item item)
        {
            bool newItem = !MainWindow.INSTANCE.SaveFileHandler.Shop.Stash.Contains(item);
            item = newItem ? item.DeepCopy() : item;
            List<InventoryBox> newboxes = MainWindow.INSTANCE.InventoryBoxHandler.GetInventoryBoxesInMatrix(start, item, MainWindow.INSTANCE.InventoryBoxHandler.StashBoxes);
            if (newboxes.Count == 0 || newboxes.Count != (item.Size.X * item.Size.Y)) { this.Update(); return false; }
            InventoryBox occupied = newboxes.Find(o => o.Item != null && o.Item != item);
            //InventoryBoxHandler.BoxType type = newboxes[0].Type;
            if (occupied != null)
            {
                this.Update();
                return false;
            }
            // if(newboxes.Find(o => o.Type != type) != null) return;
            item.InvPos = start;
            item.Inv = MainWindow.INSTANCE.SelectedStash;
            //bool sameInv = item.Inv == (ItemHandler.InvType)controlSelectedInventory.SelectedIndex;

            newboxes.ForEach(o => o.Item = item);

            if (newItem)
                MainWindow.INSTANCE.SaveFileHandler.Shop.AddItemToStash(item, false);

            return true;
        }

        public void invDragDrop(object sender, DragEventArgs e)
        {
            ItemDrag drag = e.Data.GetData(typeof(ItemDrag)) as ItemDrag;
            if (drag == null || drag.Item == null) return;

            Border border = (Border)sender;
            InventoryBox box = (InventoryBox)border.DataContext;

            Item item = drag.Item;
            bool success = this.PlaceStash(box.Position, item);
            if (success)
            {
                switch (drag.Type)
                {
                    case DragHandler.Origin.Equip:
                        drag.EquipmentSlot.SetItem(null);
                        drag.EquipmentSlot.temp = null;
                        MainWindow.INSTANCE.UpdateEquippedItems();
                        break;
                    case DragHandler.Origin.Inventory:
                        MainWindow.INSTANCE.SaveFileHandler.SelectedFile.Inventory.RemoveItem(item);
                        MainWindow.INSTANCE.UpdateInventory();
                        break;
                }
                drag.CurrentFields.ForEach(o => o.Item = null);
                drag.HoverFields.ForEach(o => o.Grid.Background = o.DefaultBackground);

                if (item.InvImage != null)
                    item.InvImage.Visibility = Visibility.Visible;
            }


            ((EquipmentView)MainWindow.INSTANCE.gridEquipment.Children[0]).SetAllowDrop(null);
            MainWindow.INSTANCE.activeDrag = false;
            MainWindow.INSTANCE.previewDrag = null;
            this.Update();
        }

        public void HighlightFields(Border border, InventoryBox startBox)
        {
            MainWindow.INSTANCE.InventoryBoxHandler.ClearSelectedItems();
            MainWindow.INSTANCE.InventoryBoxHandler.StashBoxes.ForEach(o => o.Grid.Background = o.DefaultBackground);

            InventoryBox endBox = border.DataContext as InventoryBox;
            if (startBox == endBox) return;

            List<InventoryBox> selBoxes = MainWindow.INSTANCE.InventoryBoxHandler.GetCellsBetweenPoints(startBox.Position, endBox.Position, MainWindow.INSTANCE.InventoryBoxHandler.StashBoxes);
            selBoxes.ForEach(o =>
            {
                o.Grid.Background = Util.ColorFromString("#475a69");
                if (o.Item != null)
                {
                    // o.InvImage.Background = Util.ColorFromString("#20303c");
                    o.InvImage.BorderBrush = Util.ColorFromString("#26a0da");
                }
                MainWindow.INSTANCE.InventoryBoxHandler.SelectedItems.Add(o.InvImage);
            });
        }

        public void invDragOver(object sender, DragEventArgs e)
        {
            ItemDrag drag = e.Data.GetData(typeof(ItemDrag)) as ItemDrag;
            if (drag.Item == null) return;
            //e.Effects = DragDropEffects.None;
            Border border = (Border)sender;
            Point start = ((InventoryBox)border.DataContext).Position;
            Point end = new Point(start.X + (drag.Item.Size.X - 1), start.Y + (drag.Item.Size.Y - 1));
            List<InventoryBox> boxes = MainWindow.INSTANCE.InventoryBoxHandler.GetInventoryBoxesInMatrix(start, end, MainWindow.INSTANCE.InventoryBoxHandler.StashBoxes);
            bool error = boxes.Find(o => o.Item != null && o.Item != drag.Item) != null || boxes.Count != (drag.Item.Size.X * drag.Item.Size.Y);
            boxes.ForEach(o => o.Grid.Background = error ? MainWindow.INSTANCE.InventoryBoxHandler.ErrorColor : MainWindow.INSTANCE.InventoryBoxHandler.HighlightColor);
            drag.HoverFields = boxes;
            /*if (!error)
            {
                Border b = MainWindow.INSTANCE.InventoryBoxHandler.GetImageOverlay(drag.Item, boxes);
                b.IsHitTestVisible = false;
                drag.InvImagePreview = b;
                gridInvMainImages.Children.Add(b);
                e.Effects = DragDropEffects.Copy;
            }*/
        }

        public void invDragLeave(object sender, DragEventArgs e)
        {
            ItemDrag drag = e.Data.GetData(typeof(ItemDrag)) as ItemDrag;
            if (drag == null) return;
            drag.HoverFields.ForEach(o => o.Grid.Background = o.DefaultBackground);
            /*if (drag.InvImagePreview != null)
            {
                gridInvMainImages.Children.Remove(drag.InvImagePreview);
                drag.InvImagePreview = null;
            }*/
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                MainWindow.INSTANCE.customCursor = null;
                MainWindow.INSTANCE.previewDrag = null;
                if (MainWindow.INSTANCE.activeDrag)
                {
                    ((EquipmentView)MainWindow.INSTANCE.gridEquipment.Children[0]).SetAllowDrop(null);
                    MainWindow.INSTANCE.UpdateInventory();
                    MainWindow.INSTANCE.UpdateEquippedItems();
                    MainWindow.INSTANCE.UpdateStash();
                    MainWindow.INSTANCE.activeDrag = false;
                    MainWindow.INSTANCE.previewDrag = null;
                }
            }
            else if (MainWindow.INSTANCE.previewDrag != null)
            {
                Point curPos = e.GetPosition(this);
                var delta = curPos - MainWindow.INSTANCE.previewDrag.oriPos;
                Console.WriteLine(Math.Abs(delta.Length));
                if (Math.Abs(delta.Length) < 10) return;

                MainWindow.INSTANCE.InventoryBoxHandler.InventoryBoxes.Concat(MainWindow.INSTANCE.InventoryBoxHandler.StashBoxes).ToList().FindAll(o => o.InvImage != null).ForEach(o => { if (o.InvImage.Opacity == 1) o.InvImage.Opacity = 0.8; o.InvImage.IsHitTestVisible = false; });

                if (MainWindow.INSTANCE.previewDrag.CurrentFields != null)
                    MainWindow.INSTANCE.previewDrag.CurrentFields.ForEach(o => o.Item = null);

                if (MainWindow.INSTANCE.previewDrag.Item.InvImage != null)
                    MainWindow.INSTANCE.previewDrag.Item.InvImage.Visibility = Visibility.Collapsed;

                DataObject dataObject = new DataObject(typeof(ItemDrag), MainWindow.INSTANCE.previewDrag);
                ((EquipmentView)MainWindow.INSTANCE.gridEquipment.Children[0]).SetAllowDrop(MainWindow.INSTANCE.previewDrag.Item);
                MainWindow.INSTANCE.activeDrag = true;
                DragDrop.DoDragDrop(MainWindow.INSTANCE.previewDrag.sender, dataObject, DragDropEffects.Copy);
                MainWindow.INSTANCE.previewDrag = null;
            }
        }

        public void stashSelect(object sender, EventArgs e)
        {
            Border border = (Border)sender;
            InventoryBox box = border.DataContext as InventoryBox;

            ItemDrag drag = new ItemDrag(null, null, DragHandler.Origin.StashSelect);
            drag.CurrentFields = new List<InventoryBox> { box };

            DataObject dataObject = new DataObject(typeof(ItemDrag), drag);
            DragDrop.DoDragDrop((Border)sender, dataObject, DragDropEffects.Move);
        }

        private void TabItem_DragOver(object sender, DragEventArgs e)
        {
            ItemDrag drag = e.Data.GetData(typeof(ItemDrag)) as ItemDrag;
            if (drag == null) return;
            if (drag.Type == DragHandler.Origin.StashSelect || drag.Type == DragHandler.Origin.InvSelect) return;
            controlSelectedStash.SelectedItem = sender as TabItem;
        }

        private void controlSelectedInventory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.BuildStash();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.INSTANCE.Stash = null;
            MainWindow.INSTANCE.stashOpenend = false;
        }

        private void buttonSort_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.INSTANCE.InventoryBoxHandler.StashBoxes.ForEach(o => o.Item = null);
            MainWindow.INSTANCE.SaveFileHandler.Shop.Stash.FindAll(o => o.Inv == MainWindow.INSTANCE.SelectedStash).OrderByDescending(o => o.Size.X * o.Size.Y).ToList().ForEach(o =>
            {
                InventoryBox box = MainWindow.INSTANCE.InventoryBoxHandler.FindSpace(o, MainWindow.INSTANCE.InventoryBoxHandler.StashBoxes);
                o.InvPos = box.Position;
                MainWindow.INSTANCE.InventoryBoxHandler.GetInventoryBoxesInMatrix(box.Position, o, MainWindow.INSTANCE.InventoryBoxHandler.StashBoxes).ForEach(o2 => o2.Item = o);
            });
            MainWindow.INSTANCE.UpdateStash();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void textBoxStashSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxStashSearch.Text == "Search...")
                textBoxStashSearch.Text = "";
        }

        private void textBoxStashSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxStashSearch.Text == "")
                textBoxStashSearch.Text = "Search...";
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType() != typeof(TextBox))
            {
                ((TabItem)controlSelectedStash.SelectedItem).Focus();
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            textBoxStashSearch.Text = "";
            textBoxStashSearch.Focus();
            ((TabItem)controlSelectedStash.SelectedItem).Focus();
        }

        private void textBoxStashSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.Update();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Debug.WriteLine(this.Width + " " + this.Height);
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow.INSTANCE.Window_MouseUp(sender, e);
        }
    }
}
