using System.Windows.Controls;

namespace HSeditor.Windows
{
    /// <summary>
    /// Interaktionslogik für ItemTooltip.xaml
    /// </summary>
    public partial class ItemTooltip : UserControl
    {
        public ItemTooltip(Item item)
        {
            InitializeComponent();
            Item item2 = item.DeepCopy();
            item2.BindingProp1 = item.Name.ToUpper() + (item.Slot.ID == 16 && (int)item.SaveItem["amount"] > 1 ? $" [{item.SaveItem["amount"]}]" : "");
            item2.BindingProp2 = !item.Rarity.ShowInTooltip ? item.Slot.UniqueName : item.Rarity == null ? "Unknown Rarity" : item.Rarity.Name + " " + (item.Slot.ID == 3 ? item.WeaponType.TooltipName : item.Slot.UniqueName);
            item2.BindingProp3 = item.Slot.ShowRunes ? $"({item.Sockets.GetRuneString()})" : "";
            this.mainBorder.DataContext = item2;
        }
    }
}
