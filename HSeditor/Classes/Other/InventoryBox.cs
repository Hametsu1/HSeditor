using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HSeditor.Classes.Other
{
    public class InventoryBox
    {
        public Point Position { get; set; }
        public Item? Item { get; set; }
        public Border Grid { get; set; }
        public Brush DefaultBackground { get; set; }
        public Brush DefaultBorder { get; set; }
        public InventoryBoxHandler.BoxType Type { get; set; }
        public Border InvImage { get; set; }



        public InventoryBox(Point Position, Item Item, Border Grid, InventoryBoxHandler.BoxType type, Border invImage = null)
        {
            this.Position = Position;
            this.Item = Item;
            this.Grid = Grid;
            this.DefaultBackground = Grid.Background;
            this.DefaultBorder = Grid.BorderBrush;
            Type = type;
            InvImage = invImage;
        }
    }

    public class InventoryBoxHandler
    {
        public List<InventoryBox> InventoryBoxes { get; set; }
        public List<InventoryBox> StashBoxes { get; set; }

        public List<Border> SelectedItems { get; set; }

        public enum BoxType { Item, Charm, Socketable, Key }
        public Brush HighlightColor { get; set; }
        public Brush ErrorColor { get; set; }

        public readonly Point InventorySize = new Point(15, 6);
        public readonly Point StashSize = new Point(17, 18);
        public readonly int CharmWidth = 4;
        public readonly Point BoxSize = new Point(40, 40);

        public InventoryBoxHandler()
        {
            InventoryBoxes = new List<InventoryBox>();
            StashBoxes = new List<InventoryBox>();
            SelectedItems = new List<Border>();
            HighlightColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0d6628"));
            ErrorColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#941818"));
        }

        public Border GetImageOverlay(Item item, List<InventoryBox> boxes)
        {
            Border b = new Border
            {
                DataContext = item,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(boxes[0].Grid.Width * boxes[0].Position.X, boxes[0].Grid.Height * boxes[0].Position.Y, 0, 0),
                Width = (boxes[0].Grid.Width * item.Size.X),
                Height = (boxes[0].Grid.Height * item.Size.Y),
                Background = new LinearGradientBrush((Color)ColorConverter.ConvertFromString(item.Rarity.BackgroundColor2), (Color)ColorConverter.ConvertFromString(item.Rarity.BackgroundColor), new Point(1, 0.7), new Point(0.4, 0.3)),
                BorderThickness = new Thickness(1),
                BorderBrush = Util.Util.ColorFromString(item.Rarity.IngameID == 0 ? "#702F25" : item.Rarity.Color)
            };
            b.Child = new Image { Source = new BitmapImage(new Uri(item.Sprite)), IsHitTestVisible = false, Stretch = Stretch.Uniform, StretchDirection = StretchDirection.DownOnly, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(3 * item.Size.X, 3 * item.Size.Y, 3 * item.Size.X, 3 * item.Size.Y) };
            return b;
        }

        public Border GetBoxBorder(bool charm = false)
        {
            return new Border
            {
                Width = this.BoxSize.X,
                Height = this.BoxSize.Y,
                Background = new LinearGradientBrush((Color)ColorConverter.ConvertFromString(charm ? "#151821" : "#1b1c21"), (Color)ColorConverter.ConvertFromString(charm ? "#181c2b" : "#21232b"), new Point(1, 0.7), new Point(0.4, 0.3)),
                BorderThickness = new Thickness(1),
                BorderBrush = Util.Util.ColorFromString(charm ? "#a3895b" : "#636b70"),
                AllowDrop = true,
            };
        }

        public bool isValidItem(Item item, ItemHandler.InvType? invType = null)
        {
            invType = invType == null ? (ItemHandler.InvType)MainWindow.INSTANCE.controlSelectedInventory.SelectedIndex : invType;
            switch (invType)
            {
                case ItemHandler.InvType.Socketables:
                    return item.Slot.ID == 15;
                case ItemHandler.InvType.Potion:
                    return item.Slot.ID == 11;
                default:
                    return true;
            }
        }

        public void SelectItem(Border item)
        {
            if (this.SelectedItems.Contains(item))
            {
                this.SelectedItems.Remove(item);
                Item item2 = item.DataContext as Item;
                item.BorderBrush = Util.Util.ColorFromString(item2.Rarity.Color);
            }
            else
            {
                this.SelectedItems.Add(item);
                item.BorderBrush = Util.Util.ColorFromString("#4199bf");
            }
        }


        public void ClearSelectedItems()
        {
            this.SelectedItems.ForEach(o =>
            {
                Item item = o.DataContext as Item;
                o.BorderBrush = Util.Util.ColorFromString(item.Rarity.Color);
            });
            this.SelectedItems.Clear();
        }

        public List<InventoryBox> GetCellsBetweenPoints(Point start, Point end, List<InventoryBox> boxes)
        {
            Point left = start.X > end.X ? end : start;
            Point right = left == start ? end : start;
            Point top = start.Y > end.Y ? end : start;
            Point bottom = top == start ? end : start;

            return boxes.FindAll(o => o.Position.X >= left.X && o.Position.X <= right.X && o.Position.Y >= top.Y && o.Position.Y <= bottom.Y);
        }

        public List<InventoryBox> GetInventoryBoxesInMatrix(Point start, Point end, List<InventoryBox> boxes)
        {
            return boxes.FindAll(o => o.Position.X >= start.X && o.Position.X <= end.X && o.Position.Y >= start.Y && o.Position.Y <= end.Y);
        }

        public List<InventoryBox> GetInventoryBoxesInMatrix(Point start, Item item, List<InventoryBox> boxes)
        {
            Point end = new Point(start.X + (item.Size.X - 1), start.Y + (item.Size.Y - 1));
            return boxes.FindAll(o => o.Position.X >= start.X && o.Position.X <= end.X && o.Position.Y >= start.Y && o.Position.Y <= end.Y);
        }

        public InventoryBox FindSpace(Item item, List<InventoryBox> inventoryBoxes = null)
        {
            inventoryBoxes = inventoryBoxes == null ? MainWindow.INSTANCE.Stash != null ? this.StashBoxes : this.InventoryBoxes : inventoryBoxes;
            return inventoryBoxes.Find(o =>
            {
                if (o.Item != null) return false;
                List<InventoryBox> boxes = this.GetInventoryBoxesInMatrix(o.Position, item, inventoryBoxes);
                if (boxes.Count != item.Size.X * item.Size.Y) return false;
                if (boxes.Find(o => o.Item != null) != null) return false;

                return true;
            });
        }

        public InventoryBox FindCharmSpace(Item item)
        {
            return this.InventoryBoxes.FindAll(o => o.Type == BoxType.Charm).Find(o =>
            {
                if (o.Item != null) return false;
                List<InventoryBox> boxes = this.GetInventoryBoxesInMatrix(o.Position, item, this.InventoryBoxes);
                if (boxes.Count != item.Size.X * item.Size.Y) return false;
                if (boxes.Find(o => o.Item != null) != null) return false;

                return true;
            });
        }
    }
}
