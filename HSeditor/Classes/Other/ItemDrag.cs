using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace HSeditor.Classes.Other
{
    public class ItemDrag
    {


        public EquipmentSlot? EquipmentSlot { get; private set; }
        public Item Item { get; private set; }
        public DragHandler.Origin Type { get; private set; }
        public List<InventoryBox> HoverFields { get; set; }
        public List<InventoryBox> CurrentFields { get; set; }
        public Border InvImage { get; set; }
        public Border InvImagePreview { get; set; }
        public bool isValid { get; set; }
        public Point oriPos { get; set; }
        public Border sender { get; set; }

        public ItemDrag(EquipmentSlot? equipmentSlot, Item item, DragHandler.Origin type)
        {
            EquipmentSlot = equipmentSlot;
            Item = item;
            Type = type;
            HoverFields = new List<InventoryBox>();
            CurrentFields = new List<InventoryBox>();
            isValid = true;
        }
    }

    public static class DragHandler
    {
        public enum Origin { Equip, ItemList, Inventory, InvSelect, Stash, StashSelect }
    }
}
