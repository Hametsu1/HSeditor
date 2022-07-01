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
using static HSeditor.MainWindow;

namespace HSeditor
{
    /// <summary>
    /// Interaktionslogik für ForgePrompt.xaml
    /// </summary>
    public partial class ForgePrompt : Window
    {
        public bool Quality { get; set; }
        public bool UpgradeLevel { get; set; }
        public bool ItemLevel { get; set; }
        public bool Roll { get; set; }
        public bool Ability { get; set; }
        public bool Runes { get; set; }

        public bool Stash { get; set; }
        public bool Inventory { get; set; }
        public bool Mercenary { get; set; }
        public bool Equipment { get; set; }

        public bool Cancel { get; set; }
        public ForgePrompt(bool Quality, bool UpgradeLevel, bool ItemLevel, bool Roll, bool Ability, bool Runes, ListType List)
        {
            this.Owner = MainWindow.INSTANCE;
            InitializeComponent();
            this.Stash = false;
            this.Inventory = false;
            this.Equipment = false;
            this.Mercenary = false;
            this.Cancel = false;
            this.Quality = Quality;
            this.UpgradeLevel = UpgradeLevel;
            this.ItemLevel = ItemLevel;
            this.Roll = Roll;
            this.Ability = Ability;
            this.Runes = Runes;

            this.checkBoxItemQuality.DataContext = this;
            this.checkBoxItemUpgradelevel.DataContext = this;
            this.checkBoxItemIlvl.DataContext = this;
            this.checkBoxItemRoll.DataContext = this;
            this.checkBoxItemAbility.DataContext = this;
            this.checkBoxItemRunes.DataContext = this;
            this.checkBoxItemMercenary.DataContext = this;
            this.checkBoxItemStash.DataContext = this;
            this.checkBoxItemEquipment.DataContext = this;
            this.checkBoxItemInventory.DataContext = this;
            MainWindow.INSTANCE.popupGrid.Opacity = 0.5;

            switch (List)
            {
                case ListType.Equipment:
                    this.Equipment = true;
                    break;
                case ListType.Inventory:
                    this.Inventory = true;
                    break;
                case ListType.Mercenary:
                    this.Mercenary = true;
                    break;
                case ListType.Stash:
                    this.Stash = true;
                    break;
            }
        }

        void Exit()
        {
            MainWindow.INSTANCE.popupGrid.Opacity = 0;
            this.Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Cancel = true;
            this.Exit();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            this.Exit();
        }
    }
}
