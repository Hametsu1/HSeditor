using HSeditor.Classes.Other;
using System;
using System.Windows.Controls;

namespace HSeditor.Windows
{
    /// <summary>
    /// Interaktionslogik für ItemTooltip.xaml
    /// </summary>
    public partial class ItemTooltip : UserControl
    {
        bool showStats = true;
        public ItemTooltip(Item item, bool showStats = true)
        {
            InitializeComponent();
            Item item2 = item.DeepCopy();
            this.showStats = showStats;
            item2.BindingProp1 = item.Name.ToUpper() + (item.Slot.ID == 16 && (int)item.SaveItem["amount"] > 1 ? $" [{item.SaveItem["amount"]}]" : "");
            item2.BindingProp2 = item.Rarity == null ? "Generic" : !item.Rarity.ShowInTooltip ? item.Slot.UniqueName : item.Rarity == null ? "Unknown Rarity" : item.Rarity.Name + " " + (item.Slot.ID == 3 ? item.WeaponType.TooltipName : item.Slot.UniqueName);
            item2.BindingProp3 = item.Slot.ShowRunes ? $"({item.Sockets.GetRuneString()})" : "";
            item2.BindingProp4 = "None";
            if (item2.SaveItem != null && item2.Slot.ShowAugment && item.SaveItem.ContainsKey("token") && item.SaveItem.ContainsKey("token_level"))
            {
                int id;
                bool valid = Int32.TryParse(Convert.ToString(item.SaveItem["token"]), out id);
                if (valid && id != 0)
                {
                    Augment augment = MainWindow.INSTANCE.AugmentHandler.GetAugmentFromID(id);

                    valid = Int32.TryParse(Convert.ToString(item.SaveItem["token_level"]), out id);
                    if (valid)
                        item2.BindingProp4 = $"Augment: {augment.Name} [{id}]";
                }
            }
            this.mainBorder.DataContext = item2;
        }

        private void StackPanel_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            (sender as StackPanel).Visibility = this.showStats ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }
    }
}
