using HSeditor.Classes;
using HSeditor.Classes.Filter.Item;
using HSeditor.Classes.iniDB;
using HSeditor.Classes.Merc;
using HSeditor.Classes.Other;
using HSeditor.Classes.Util;
using HSeditor.SaveFiles;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HSeditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    /*
     * Fix online builds timeout
     * Add local builds
     * Add misc tab
     * 
     * 
     * 
     * */
    public partial class MainWindow : Window
    {
        public static MainWindow INSTANCE;
        public Random random = new Random();
        public iniDB iniDB { get; private set; }
        public UpdateHandler UpdateHandler { get; private set; }
        public ConfigHandler ConfigHandler { get; private set; }
        public AbilityHandler AbilityHandler { get; private set; }
        public RuneHandler RuneHandler { get; private set; }
        public ClassHandler ClassHandler { get; private set; }
        public RarityHandler RarityHandler { get; private set; }
        public SaveFileHandler SaveFileHandler { get; private set; }
        public ItemHandler ItemHandler { get; private set; }
        public SetHandler SetHandler { get; private set; }
        public UberHandler UberHandler { get; private set; }
        public RelicHandler RelicHandler { get; private set; }
        public WeaponTypeHandler WeaponTypeHandler { get; private set; }
        public SlotHandler SlotHandler { get; private set; }
        public BuildHandler BuildHandler { get; private set; }
        public TalentHandler TalentHandler { get; private set; }
        public StatHandler StatHandler { get; private set; }
        public MercenaryHandler MercenaryHandler { get; private set; }
        public enum ListType { Equipment, Inventory, Mercenary, Stash }
        public MainWindow()
        {
            INSTANCE = this;
            var SplashScreen = new SplashScreen("Resources/SplashScreen.png");
            SplashScreen.Show(false);
            this.CheckDirectory();
            this.iniDB = new iniDB(Environment.CurrentDirectory + @"\ini.dll");
            this.UpdateHandler = new UpdateHandler();
            this.StatHandler = new StatHandler();
            this.RuneHandler = new RuneHandler();
            InitializeComponent();
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));
            ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(300));
            IsTabStopProperty.OverrideMetadata(typeof(Control), new FrameworkPropertyMetadata(false));
            this.WeaponTypeHandler = new WeaponTypeHandler();
            this.SlotHandler = new SlotHandler();
            this.TalentHandler = new TalentHandler();
            this.ClassHandler = new ClassHandler();
            this.SetHandler = new SetHandler();
            this.AbilityHandler = new AbilityHandler();
            this.RarityHandler = new RarityHandler();
            this.ConfigHandler = new ConfigHandler();
            this.RefreshSettings();
            this.ItemHandler = new ItemHandler();
            this.StatHandler.ReadStats();
            this.RelicHandler = new RelicHandler();
            this.UberHandler = new UberHandler();
            this.MercenaryHandler = new MercenaryHandler();
            this.BuildHandler = new BuildHandler();
            this.SaveFileHandler = new SaveFileHandler();
            this.popupGrid.Visibility = Visibility.Visible;
            SplashScreen.Close(TimeSpan.FromSeconds(1));
        }

        public void CheckDirectory()
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege");

            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\save_folder"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\save_folder");

            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hseditor"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hseditor");

            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hseditor\builds"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hseditor\builds");
        }

        private void listBox_Slots_Loaded(object sender, RoutedEventArgs e)
        {
            listBox_Slots.ItemsSource = this.SaveFileHandler.SaveFiles;
            listBox_Slots.SelectedIndex = 0;
        }

        public void RefreshListboxes()
        {
            if (this.SaveFileHandler == null) return;
            this.listBox_Slots.ItemsSource = this.SaveFileHandler.SaveFiles;
            this.listBox_Slots.Items.Refresh();
            this.listBox_Inventory.ItemsSource = this.SaveFileHandler.SelectedFile.Inventory.InventoryItems;
            UpdateEquippedItems();
            UpdateInventory();
            this.listBoxRelicInventory.ItemsSource = this.SaveFileHandler.SelectedFile.Relics;
            this.UpdateForgeLists();
            this.listBox_Talents1.ItemsSource = this.SaveFileHandler.SelectedFile.HeroInfo.Class.Talents.GetTree1();
            this.listBox_Talents2.ItemsSource = this.SaveFileHandler.SelectedFile.HeroInfo.Class.Talents.GetTree2();
            this.listBox_HeroTalents.ItemsSource = this.SaveFileHandler.SelectedFile.HeroInfo.HeroTalents;
            this.labelTalent_Tree1.Content = this.SaveFileHandler.SelectedFile.HeroInfo.Class.Talents.Name_Tree1;
            this.labelTalent_Tree2.Content = this.SaveFileHandler.SelectedFile.HeroInfo.Class.Talents.Name_Tree2;
            this.listBoxActiveTalents.ItemsSource = this.SaveFileHandler.SelectedFile.HeroInfo.ActiveTalents;
            this.listBox_HeroTalents.ItemsSource = this.SaveFileHandler.SelectedFile.HeroInfo.HeroTalents;
            this.UpdateBuildsList();
            this.labelInventoryCount.Content = this.listBox_Inventory.Items.Count + "/100";
            this.labelInventoryCountForge.Content = this.listBox_InventoryForge.Items.Count + "/100";
            this.labelTalentPointsSpent.Text = this.SaveFileHandler.SelectedFile.HeroInfo.GetTalentPointsSpent() + "/" + this.SaveFileHandler.SelectedFile.HeroInfo.Level;

            this.listBoxMercTalents_Offensive.ItemsSource = this.SaveFileHandler.SelectedFile.Mercenaries.Talents.GetOffensiveTalents(this.SaveFileHandler.SelectedFile.Mercenaries.GetSelected());
            this.listBoxMercTalents_Defensive.ItemsSource = this.SaveFileHandler.SelectedFile.Mercenaries.Talents.GetDefensiveTalents();
            this.textBoxCompanionName.Text = this.SaveFileHandler.Shop.CompanionName;
            this.textBoxCompanionID.Text = this.SaveFileHandler.Shop.CompanionID.ToString();

            this.UpdateRelicFilter();
            this.UpdateUberFilter();
            this.UpdateRuneFilter();
        }

        public void UpdateBuildsList()
        {
            if (this.BuildHandler == null) return;
            this.listBox_Builds.ItemsSource = controlSelectedBuilds.SelectedIndex == 0 ? this.BuildHandler.Builds : this.BuildHandler.OfflineBuilds;
            this.listBox_Builds.Items.Refresh();
        }

        public void UpdateForgeLists()
        {
            this.listBox_InventoryForge.ItemsSource = this.SaveFileHandler.SelectedFile.Inventory.InventoryItems.Concat(this.SaveFileHandler.Shop.Stash);
            this.listBox_InventoryForge.Items.Refresh();
            ICollectionView cv = CollectionViewSource.GetDefaultView(listBox_InventoryForge.ItemsSource);
            cv.GroupDescriptions.Clear();
            cv.GroupDescriptions.Add(new PropertyGroupDescription("Stash"));
            this.listBox_EquippeditemsForge.ItemsSource = this.SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipmentList().Concat(this.SaveFileHandler.SelectedFile.Mercenaries.GetSelectedMerc().Equipment.GetEquipmentList());
            this.listBox_EquippeditemsForge.Items.Refresh();
            cv = CollectionViewSource.GetDefaultView(listBox_EquippeditemsForge.ItemsSource);
            cv.GroupDescriptions.Clear();
            cv.GroupDescriptions.Add(new PropertyGroupDescription("Mercenary"));
        }

        public void UpdateEquippedItems()
        {
            if (this.SaveFileHandler == null) return;
            if (controlSelectedEquipment.SelectedIndex == 0)
            {
                this.listBoxEquipment.ItemsSource = this.SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipment();
                this.listBox_Equippeditems.ItemsSource = this.SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipmentList();
            }
            else
            {
                this.listBoxEquipment.ItemsSource = this.SaveFileHandler.SelectedFile.Mercenaries.GetSelected() == 0 ? this.SaveFileHandler.SelectedFile.Mercenaries.Melee.Equipment.GetEquipment() : this.SaveFileHandler.SelectedFile.Mercenaries.Ranged.Equipment.GetEquipment();
                this.listBox_Equippeditems.ItemsSource = this.SaveFileHandler.SelectedFile.Mercenaries.GetSelected() == 0 ? this.SaveFileHandler.SelectedFile.Mercenaries.Melee.Equipment.GetEquipmentList() : this.SaveFileHandler.SelectedFile.Mercenaries.Ranged.Equipment.GetEquipmentList();
            }
            listBoxEquipment.Items.Refresh();
            listBox_Equippeditems.Items.Refresh();

        }

        public void UpdateInventory()
        {
            if (this.SaveFileHandler == null) return;
            this.listBox_Inventory.ItemsSource = controlSelectedInventory.SelectedIndex == 0 ? this.SaveFileHandler.SelectedFile.Inventory.InventoryItems : this.SaveFileHandler.Shop.Stash;
            this.listBox_Inventory.Items.Refresh();
            this.labelInventoryCount.Content = this.listBox_Inventory.Items.Count + "/100";
        }

        public void UpdateMercenaryInfo()
        {
            textBoxMercName.Text = this.SaveFileHandler.SelectedFile.Mercenaries.GetSelected() == 0 ? this.SaveFileHandler.Shop.MercenaryName_Melee : this.SaveFileHandler.Shop.MercenaryName_Ranged;
            if (this.SaveFileHandler.SelectedFile.Mercenaries.GetSelected() == 0)
                buttonMeleeMerc.IsChecked = true;
            else
                buttonRangedMerc.IsChecked = true;
            this.labelMercTalentPointsSpent.Text = this.SaveFileHandler.SelectedFile.Mercenaries.Talents.GetPointsSpent() + "/" + this.SaveFileHandler.SelectedFile.HeroInfo.Level;
            if (this.SaveFileHandler.SelectedFile.Mercenaries.GetSelected() == 1)
            {
                this.listBoxMercTalents_Defensive.ItemsSource = null;
                this.listBoxMercTalents_Defensive.Visibility = Visibility.Collapsed;
                this.listBoxMercTalents_Offensive.ItemsSource = this.SaveFileHandler.SelectedFile.Mercenaries.Talents.GetOffensiveTalents(1);
            }
            else
            {
                this.listBoxMercTalents_Defensive.ItemsSource = this.SaveFileHandler.SelectedFile.Mercenaries.Talents.GetDefensiveTalents();
                this.listBoxMercTalents_Defensive.Visibility = Visibility.Visible;
                this.listBoxMercTalents_Offensive.ItemsSource = this.SaveFileHandler.SelectedFile.Mercenaries.Talents.GetOffensiveTalents(0);
            }
            this.UpdateEquippedItems();
        }



        private void listBox_Slots_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SwitchSaveSlot();
            this.ContextMenuSlots.DataContext = (SaveFile)this.listBox_Slots.SelectedItem;
        }

        private void SwitchSaveSlot()
        {
            this.SaveFileHandler.ReadSaveFile((SaveFile)listBox_Slots.SelectedItem);
            this.Title = new string($"HSeditor - Name: {this.SaveFileHandler.SelectedFile.HeroInfo.Name} | Class: {this.SaveFileHandler.SelectedFile.HeroInfo.Class.Name} | ID: {this.SaveFileHandler.SelectedFile.ID} | Version: {this.UpdateHandler.Version}");
            this.UpdateHeroInfo();
            this.UpdateMercenaryInfo();
            this.ToggleHardcoreButton();
            this.gridForgeAttributes.DataContext = null;
            this.RefreshForgeAttributes(null);
            RefreshListboxes();
            if (this.SlotHandler.SelectedItemSlot != null) this.SelectEquipmentSlot(this.SlotHandler.SelectedItemSlot);
        }

        private void listBox_Items_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed) return;
            this.AddItemToInventory((Item)listBox_Items.SelectedItem, Keyboard.IsKeyDown(Key.LeftShift) ? 10 : 1);
        }

        private void comboBoxClasses_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxClasses.ItemsSource = this.ClassHandler.Classes_Filtered;
        }

        private void comboBoxClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((Class)comboBoxClasses.SelectedItem).ID == this.SaveFileHandler.SelectedFile.HeroInfo.Class.ID) return;
            this.SaveFileHandler.SelectedFile.HeroInfo.Class = this.ClassHandler.GetClassFromID(((Class)comboBoxClasses.SelectedItem).ID).DeepCopy();
            this.SaveFileHandler.SelectedFile.HeroInfo.ClearActiveTalents();
            this.RefreshListboxes();
            this.listBoxActiveTalents.Items.Refresh();
        }

        private void textBoxItemlistSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (((TextBox)sender).Text == "Search...")
                ((TextBox)sender).Text = "";
        }

        private void textBoxItemlistSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(((TextBox)sender).Text) || String.IsNullOrWhiteSpace(((TextBox)sender).Text))
                ((TextBox)sender).Text = "Search...";
        }

        public void ToggleHardcoreButton()
        {
            if (this.SaveFileHandler.SelectedFile.HeroInfo.Hardcore)
                this.buttonHardcoreYes.IsChecked = true;
            else
                this.buttonHardcoreNo.IsChecked = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateHandler.CheckForUpdate();
            comboBoxItemFilterSlot.ItemsSource = this.SlotHandler.SlotsFiltered;
            comboBoxItemFilterWeaponType.ItemsSource = this.WeaponTypeHandler.WeaponTypesFiltered;
        }

        public void UpdateRelicFilter()
        {
            if (comboBoxRelicFilterType == null || comboBoxRelicFilterType.SelectedItem == null || comboBoxRelicFilterStat.SelectedItem == null) return;
            this.RelicHandler.RelicFilter.Name = textBoxRelicListSearch.Text;
            this.RelicHandler.RelicFilter.Type = (RelicType)comboBoxRelicFilterType.SelectedItem;
            this.RelicHandler.RelicFilter.Stats = new List<Stat>();
            this.RelicHandler.RelicFilter.Stats.Add((Stat)comboBoxRelicFilterStat.SelectedItem);
            this.listBox_Relics.ItemsSource = this.RelicHandler.GetFilteredList();
            this.listBox_Relics.Items.Refresh();
            if (listBox_Relics.Items.Count > 0)
                this.listBox_Relics.ScrollIntoView(listBox_Relics.Items[0]);
            textBoxRelicListSearch.Foreground = listBox_Relics.Items.Count == 0 ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a32424")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF6F6F6"));
            if (this.SaveFileHandler == null || this.SaveFileHandler.SelectedFile == null) return;
            labelSelectedRelics.Content = $"{this.SaveFileHandler.SelectedFile.Relics.Count}/30";
            buttonSaveRelicTemplate.IsEnabled = this.SaveFileHandler.SelectedFile.Relics.Count > 0;
            this.buttonAddRelic.IsEnabled = this.SaveFileHandler.SelectedFile.Relics.Count == 30 ? false : true;
        }

        private void UpdateUberFilter()
        {
            if (this.comboBoxUberFilterType == null || this.comboBoxUberFilterType.SelectedItem == null || this.SaveFileHandler.SelectedFile == null) return;
            this.listBoxUberInventory.ItemsSource = this.SaveFileHandler.SelectedFile.Inventory.GetFilteredUbers(textBoxUberFilterSearch.Text, (UberType)comboBoxUberFilterType.SelectedItem);
            ICollectionView cv = CollectionViewSource.GetDefaultView(listBoxUberInventory.ItemsSource);
            cv.GroupDescriptions.Clear();
            cv.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
            textBoxUberFilterSearch.Foreground = listBoxUberInventory.Items.Count == 0 ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a32424")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF6F6F6"));
        }

        private void UpdateRuneFilter()
        {
            if (this.comboBoxRuneFilterType == null || this.comboBoxRuneFilterType.SelectedItem == null || this.SaveFileHandler.SelectedFile == null) return;
            this.listBoxRuneInventory.ItemsSource = this.SaveFileHandler.SelectedFile.Inventory.GetFilteredRunes(textBoxRuneFilterSearch.Text, (RuneType)comboBoxRuneFilterType.SelectedItem);
            ICollectionView cv = CollectionViewSource.GetDefaultView(listBoxRuneInventory.ItemsSource);
            cv.GroupDescriptions.Clear();
            cv.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
            textBoxRuneFilterSearch.Foreground = listBoxRuneInventory.Items.Count == 0 ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a32424")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF6F6F6"));
        }

        public void UpdateItemFilter()
        {
            if (comboBoxItemFilterSlot == null || comboBoxItemFilterWeaponType == null || comboBoxItemFilterSlot.SelectedItem == null ||
                comboBoxItemFilterWeaponType.SelectedItem == null) return;
            if (this.ItemHandler.Filter == null) this.ItemHandler.Filter = new ItemFilter();
            if (this.ConfigHandler.Favorites == null) this.ConfigHandler.LoadFavorites();
            this.ItemHandler.Filter.Name = textBoxItemlistSearch.Text;
            this.ItemHandler.Filter.Slot = (Slot)comboBoxItemFilterSlot.SelectedItem;
            this.ItemHandler.Filter.WeaponType = (WeaponType)comboBoxItemFilterWeaponType.SelectedItem;
            List<Item> items = this.ItemHandler.GetFilteredList().OrderBy(o => o.Slot.ID).ThenBy(o => o.WeaponType.ID).ThenBy(o => o.Runeword).ThenBy(o => o.ID).ToList();
            this.listBox_Items.ItemsSource = items;
            if (Util.ContainsFavorite(items))
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(listBox_Items.ItemsSource);
                view.GroupDescriptions.Add(new PropertyGroupDescription("Favorite"));
                view.SortDescriptions.Add(new SortDescription("Favorite", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("Slot.ID", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("WeaponType.ID", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("Runeword", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("ID", ListSortDirection.Ascending));
            }
            this.labelItemCount.Content = $"{this.listBox_Items.Items.Count}";
            if (listBox_Items.Items.Count > 0)
                this.listBox_Items.ScrollIntoView(listBox_Items.Items[0]);
            labelItemCount.Foreground = listBox_Items.Items.Count == 0 ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a32424")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF99AAB5"));
            textBoxItemlistSearch.Foreground = listBox_Items.Items.Count == 0 ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a32424")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF6F6F6"));
        }



        private void comboBoxItemFilterSlot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.comboBoxItemFilterWeaponType.IsEnabled = true;
            if (this.comboBoxItemFilterSlot.SelectedItem == null) return;
            if (((Slot)this.comboBoxItemFilterSlot.SelectedItem).ID != 3)
            {
                this.comboBoxItemFilterWeaponType.SelectedIndex = 0;
                if (((Slot)this.comboBoxItemFilterSlot.SelectedItem).ID != -1)
                    this.comboBoxItemFilterWeaponType.IsEnabled = false;
            }
            this.UpdateItemFilter();
        }

        private void comboBoxItemFilterWeaponType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateItemFilter();
            if (this.comboBoxItemFilterWeaponType.SelectedIndex != 0 && this.comboBoxItemFilterWeaponType.SelectedIndex != -1 && ((Slot)this.comboBoxItemFilterSlot.SelectedItem).ID != 3) comboBoxItemFilterSlot.SelectedItem = this.SlotHandler.GetSlotFromName("Weapon");
        }

        private void textBoxItemlistSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.UpdateItemFilter();
        }

        private void listBox_Inventory_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
                ScrollBar.LineDownCommand.Execute(null, e.OriginalSource as IInputElement);
            if (e.Delta > 0)
                ScrollBar.LineUpCommand.Execute(null, e.OriginalSource as IInputElement);
            e.Handled = true;
        }

        private void SelectEquipmentSlot(Slot slot)
        {

            if (slot == null)
            {
                this.SlotHandler.SelectedItemSlot = null;
                this.comboBoxItemFilterSlot.SelectedIndex = 0;
                this.HighlightEquipmentSlot(slot);
                return;
            }
            this.SlotHandler.SelectedItemSlot = slot;
            this.comboBoxItemFilterSlot.SelectedItem = slot;
            this.listBox_Equippeditems.SelectedItem = controlSelectedEquipment.SelectedIndex == 0 ? this.SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipmentSlotFromSlot(slot) : this.SaveFileHandler.SelectedFile.Mercenaries.GetSelectedMerc().Equipment.GetEquipmentSlotFromSlot(slot);

            this.HighlightEquipmentSlot(slot);
        }

        private void listBox_Equippeditems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.listBox_Equippeditems.SelectedItem == null) return;
            this.SelectEquipmentSlot(((EquipmentSlot)listBox_Equippeditems.SelectedItem).Slot);
            this.listBox_Equippeditems.ScrollIntoView(listBox_Equippeditems.SelectedItem);
        }

        private void AddItemToInventory(Item Item, int amount = 1, bool refresh = true)
        {
            if (Item == null) return;
            for (int i = 0; i < amount; i++)
            {
                Item item = Item.DeepCopy();
                if (!item.Favorite)
                    item.RollID = item.RollID == -1 ? random.Next(0, 10001) : item.RollID;

                if (controlSelectedInventory.SelectedIndex == 0)
                    this.SaveFileHandler.SelectedFile.Inventory.AddItem(item, false);
                else
                    this.SaveFileHandler.Shop.AddItemToStash(item);
            }
            labelInventoryCount.Content = $"{this.listBox_Inventory.Items.Count}/100";

            if (!refresh) return;
            this.RefreshListboxes();
            listBox_Inventory.ScrollIntoView(listBox_Inventory.Items[listBox_Inventory.Items.Count - 1]);
        }

        private void HighlightEquipmentSlot(Slot Slot)
        {
            foreach (EquipmentSlot slot in listBoxEquipment.Items)
            {
                slot.Status = "#FF1B1D21";
                if (Slot != null && slot.Slot == Slot)
                    slot.Status = "#474b52";
            }
            listBoxEquipment.Items.Refresh();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Border border = sender as Border;
                if (((EquipmentSlot)border.DataContext).Slot == this.SlotHandler.SelectedItemSlot)
                {
                    this.SlotHandler.SelectedItemSlot = null;
                    this.HighlightEquipmentSlot(null);
                    listBox_Equippeditems.UnselectAll();
                    this.comboBoxItemFilterSlot.SelectedIndex = 0;
                    return;
                }
                this.SelectEquipmentSlot(((EquipmentSlot)border.DataContext).Slot);
            }
            catch { }
        }

        private void buttonEquipItem_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_Items.SelectedItem == null) return;
            Item item = ((Item)listBox_Items.SelectedItem).DeepCopy(); ;
            if (this.ConfigHandler.Config.NewItem.RollID == -1 && !item.Favorite) item.RollID = random.Next(0, 10001);
            this.EquipItem(item, true);
            //Slot slot = this.SlotHandler.SelectedItemSlot == null ? null : this.SlotHandler.SelectedItemSlot;
            //Item Item = controlSelectedEquipment.SelectedIndex == 0 ? this.SaveFileHandler.SelectedFile.Inventory.Equipment.GetItemFromSlot(slot) : this.SaveFileHandler.SelectedFile.Mercenaries.GetSelectedMerc().Equipment.GetItemFromSlot(slot);
            //if (controlSelectedEquipment.SelectedIndex == 0)
            //    this.SaveFileHandler.SelectedFile.Inventory.Equipment.EquipItem(item, this.SlotHandler.SelectedItemSlot == null ? null : this.SlotHandler.SelectedItemSlot.ID);
            //else
            //{
            //    if (slot == null && !Util.MercenaryComp(item.Slot.ID) || slot != null && !Util.MercenaryComp(slot.ID)) return;
            //    this.SaveFileHandler.SelectedFile.Mercenaries.
            //}
            //if (Item != null)
            //    this.AddItemToInventory(Item, 1, true);
            ////this.SelectEquipmentSlot(this.SlotHandler.SelectedItemSlot == null ? item.Slot : this.SlotHandler.SelectedItemSlot);
        }

        private void listBox_Relics_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.UpdateRelicFilter();
        }

        private void comboBoxRelicFilterType_Loaded(object sender, RoutedEventArgs e)
        {
            this.comboBoxRelicFilterType.ItemsSource = this.RelicHandler.RelicTypes;
        }

        private void textBoxRelicListSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.UpdateRelicFilter();
        }

        private void comboBoxRelicFilterSearchBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateRelicFilter();
        }

        private void listBox_Relics_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.SaveFileHandler.SelectedFile.AddRelic((Relic)listBox_Relics.SelectedItem);
        }

        private void buttonAddRelic_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBox_Relics.SelectedItem == null) return;
            this.SaveFileHandler.SelectedFile.AddRelic((Relic)listBox_Relics.SelectedItem);
        }

        private void buttonClearRelics_Click(object sender, RoutedEventArgs e)
        {
            this.SaveFileHandler.SelectedFile.Relics.Clear();
            this.UpdateRelicFilter();
        }

        private void Border_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Border border = sender as Border;
                this.SaveFileHandler.SelectedFile.RemoveRelic((Relic)border.DataContext);
            }
            catch { }
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                Border border = sender as Border;
                border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#474b52"));
            }
            catch { }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                Border border = sender as Border;
                EquipmentSlot eq = (EquipmentSlot)border.DataContext;
                if (this.SlotHandler.SelectedItemSlot != eq.Slot)
                    border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1B1D21"));
            }
            catch { }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.AddItemToInventory((Item)listBox_Items.SelectedItem, Keyboard.IsKeyDown(Key.LeftShift) ? 10 : 1);
        }

        private void listBox_Inventory_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.listBox_Inventory.SelectedItem == null) return;
            Item Item = (Item)listBox_Inventory.SelectedItem;
            ListBox lb = (ListBox)sender;
            switch (e.Key)
            {
                case Key.Enter:
                    this.EquipItem(Item);
                    break;
                case Key.Delete:
                    if (Item.Stash) this.SaveFileHandler.Shop.Stash.Remove(Item); else this.SaveFileHandler.SelectedFile.Inventory.RemoveItem(Item);
                    break;
                case Key.Down:
                    if (lb.SelectedIndex == lb.Items.Count - 1) { e.Handled = true; return; }
                    lb.SelectedIndex += 1;
                    lb.ScrollIntoView(lb.SelectedItem);
                    e.Handled = true;
                    break;
                case Key.Up:
                    if (lb.SelectedIndex == 0) { e.Handled = true; return; }
                    lb.SelectedIndex -= 1;
                    lb.ScrollIntoView(lb.SelectedItem);
                    e.Handled = true;
                    break;
                default:
                    e.Handled = true;
                    return;
            }
        }

        private void listBox_Relics_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            switch (e.Key)
            {
                case Key.Enter:
                    if (this.listBox_Relics.SelectedItem != null)
                    {
                        int id = listBox_Relics.SelectedIndex;
                        this.SaveFileHandler.SelectedFile.AddRelic((Relic)listBox_Relics.SelectedItem);
                        if (listBox_Relics.Items.Count > id)
                            listBox_Relics.SelectedIndex = id;
                        if (listBox_Relics.Items.Count == id && id != 0)
                            listBox_Relics.SelectedIndex = id - 1;
                        listBox_Relics.ScrollIntoView(listBox_Relics.SelectedItem);
                    }
                    break;
                case Key.Down:
                    if (lb.SelectedIndex == lb.Items.Count - 1) { e.Handled = true; return; }
                    lb.SelectedIndex += 1;
                    lb.ScrollIntoView(lb.SelectedItem);
                    e.Handled = true;
                    break;
                case Key.Up:
                    if (lb.SelectedIndex == 0) { e.Handled = true; return; }
                    lb.SelectedIndex -= 1;
                    lb.ScrollIntoView(lb.SelectedItem);
                    e.Handled = true;
                    break;
                default:
                    e.Handled = true;
                    return;
            }
        }

        private async void Border_MouseEnter_1(object sender, MouseEventArgs e)
        {
            try
            {
                if (Mouse.RightButton == MouseButtonState.Pressed)
                {
                    DateTime timeButtonWasClicked = DateTime.Now;
                    TimeSpan ts = DateTime.Now - timeButtonWasClicked;
                    if (ts.Milliseconds < 120) await Task.Delay(TimeSpan.FromMilliseconds(120).Subtract(ts));
                    if (Mouse.RightButton == MouseButtonState.Pressed && ((Border)sender).IsMouseOver)
                        this.SaveFileHandler.SelectedFile.RemoveRelic((Relic)((Border)sender).DataContext);
                    return;
                }

                Border border = sender as Border;
                Relic relic = (Relic)border.DataContext;
                border.BorderBrush = relic.Type.Name == "Active" ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#73602e")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2e265c"));
            }
            catch { }
        }

        private void Border_MouseLeave_1(object sender, MouseEventArgs e)
        {
            try
            {
                Border border = sender as Border;
                border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(((Relic)border.DataContext).Border));
            }
            catch { }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                int x = Int32.Parse(e.Text);
                int y = Int32.Parse(((TextBox)sender).Text + e.Text);
            }
            catch
            {
                e.Handled = true;
            }
        }





        public void Refresh()
        {
            if (this.SaveFileHandler == null) return;
            int slotid = this.SaveFileHandler.SelectedFile.ID;
            this.SaveFileHandler.Shop.Refresh();
            this.SaveFileHandler.GetSaveFiles();
            this.listBox_Slots.SelectedItem = this.SaveFileHandler.SaveFiles[slotid];
            this.SwitchSaveSlot();
        }


        private void textBoxUberAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == "")
                tb.Text = "1";

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Refresh();
        }

        private void listBox_InventoryForge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox_InventoryForge.SelectedItem == null) return;

            RefreshForgeAttributes((Item)listBox_InventoryForge.SelectedItem);
            listBox_EquippeditemsForge.UnselectAll();
        }

        private void RefreshForgeAttributes(Item item)
        {
            textBoxForgeQuality.IsEnabled = item == null ? false : true;
            textBoxForgeUpgradeLevel.IsEnabled = item == null ? false : true;
            textBoxForgeIlvl.IsEnabled = item == null ? false : true;
            textBoxForgeRollID.IsEnabled = item == null ? false : true;
            buttonForgeRandomRoll.IsEnabled = item == null ? false : true;
            textBoxForgeAbilityLevel.IsEnabled = item == null ? false : true;
            comboBoxForgeAbilities.IsEnabled = item == null ? false : true;

            comboBoxForgeRune1.IsEnabled = item == null || item.Rarity.Name == "Runeword" || (item.Stats != null && item.Stats.SocketsMax < 1) ? false : true;
            comboBoxForgeRune2.IsEnabled = item == null || item.Rarity.Name == "Runeword" || (item.Stats != null && item.Stats.SocketsMax < 2) ? false : true;
            comboBoxForgeRune3.IsEnabled = item == null || item.Rarity.Name == "Runeword" || (item.Stats != null && item.Stats.SocketsMax < 3) ? false : true;
            comboBoxForgeRune4.IsEnabled = item == null || item.Rarity.Name == "Runeword" || (item.Stats != null && item.Stats.SocketsMax < 4) ? false : true;
            comboBoxForgeRune5.IsEnabled = item == null || item.Rarity.Name == "Runeword" || (item.Stats != null && item.Stats.SocketsMax < 5) ? false : true;
            comboBoxForgeRune6.IsEnabled = item == null || item.Rarity.Name == "Runeword" || (item.Stats != null && item.Stats.SocketsMax < 6) ? false : true;
            comboBoxForgeRuneAll.IsEnabled = item == null || item.Rarity.Name == "Runeword" || (item.Stats != null && item.Stats.SocketsMax == 0) ? false : true;


            if (item == null) return;
            buttonForgeSave.IsEnabled = true;
            buttonForgeSaveAll.IsEnabled = true;
            this.gridForgeAttributes.DataContext = item.DeepCopy();
            this.comboBoxForgeRuneAll.SelectedIndex = -1;
        }

        private void listBox_EquippeditemsForge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox_EquippeditemsForge.SelectedItem == null) return;

            RefreshForgeAttributes((EquipmentSlot)listBox_EquippeditemsForge.SelectedItem == null ? null : ((EquipmentSlot)listBox_EquippeditemsForge.SelectedItem).Item);
            listBox_InventoryForge.UnselectAll();
        }

        private void comboBoxForgeAbilities_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxForgeAbilities.ItemsSource = this.AbilityHandler.Abilities;
        }

        private void comboBoxForgeRune1_Loaded(object sender, RoutedEventArgs e)
        {
            ((ComboBox)sender).ItemsSource = this.RuneHandler.Runes;
        }

        private void comboBoxForgeRuneAll_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxForgeRuneAll.SelectedIndex == -1) return;
            Item item = (Item)gridForgeAttributes.DataContext;
            comboBoxForgeRune1.SelectedItem = item.Stats != null && item.Stats.SocketsMax < 1 ? comboBoxForgeRune1.SelectedItem : comboBoxForgeRuneAll.SelectedItem;
            comboBoxForgeRune2.SelectedItem = item.Stats != null && item.Stats.SocketsMax < 2 ? comboBoxForgeRune2.SelectedItem : comboBoxForgeRuneAll.SelectedItem;
            comboBoxForgeRune3.SelectedItem = item.Stats != null && item.Stats.SocketsMax < 3 ? comboBoxForgeRune3.SelectedItem : comboBoxForgeRuneAll.SelectedItem;
            comboBoxForgeRune4.SelectedItem = item.Stats != null && item.Stats.SocketsMax < 4 ? comboBoxForgeRune4.SelectedItem : comboBoxForgeRuneAll.SelectedItem;
            comboBoxForgeRune5.SelectedItem = item.Stats != null && item.Stats.SocketsMax < 5 ? comboBoxForgeRune5.SelectedItem : comboBoxForgeRuneAll.SelectedItem;
            comboBoxForgeRune6.SelectedItem = item.Stats != null && item.Stats.SocketsMax < 6 ? comboBoxForgeRune6.SelectedItem : comboBoxForgeRuneAll.SelectedItem;
        }

        private void buttonForgeSave_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_EquippeditemsForge.SelectedItem == null && listBox_InventoryForge.SelectedItem == null) return;
            Item item = listBox_EquippeditemsForge.SelectedItem == null ? (Item)listBox_InventoryForge.SelectedItem : ((EquipmentSlot)listBox_EquippeditemsForge.SelectedItem).Item;
            Item item2 = (Item)gridForgeAttributes.DataContext;
            item.Forge(item2.ItemLevel, item2.Quality, item2.UpgradeLevel, item2.RollID, item2.Ability, item2.AbilityLevel, item2.Sockets.GetRuneList());
            this.RefreshListboxes();
        }

        private void buttonForgeRandomRoll_Click(object sender, RoutedEventArgs e)
        {
            textBoxForgeRollID.Text = new Random().Next(0, 10001).ToString();
            textBoxForgeRollID.Focus();
            Keyboard.ClearFocus();
        }

        private void listBox_Builds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox_Builds.SelectedItem == null) return;
            gridBuildInfo.DataContext = (Build)listBox_Builds.SelectedItem;
            gridBuildInfo.Visibility = Visibility.Visible;
            this.listBoxEquipment_Builds.ItemsSource = ((Build)listBox_Builds.SelectedItem).SaveFile.Inventory.Equipment.GetEquipment();
            if (((Build)listBox_Builds.SelectedItem).SaveFile.Mercenaries != null)
            {
                this.listBoxEquipmentMerc_Builds.ItemsSource = ((Build)listBox_Builds.SelectedItem).SaveFile.Mercenaries.GetSelectedMerc().Equipment.GetEquipment();
                this.listBoxMercTalentsOff.ItemsSource = ((Build)listBox_Builds.SelectedItem).SaveFile.Mercenaries.Talents.GetOffensiveList(((Build)listBox_Builds.SelectedItem).SaveFile.Mercenaries.GetSelected());
                this.listBoxMercTalentsDef.ItemsSource = ((Build)listBox_Builds.SelectedItem).SaveFile.Mercenaries.Talents.GetDefensiveList(((Build)listBox_Builds.SelectedItem).SaveFile.Mercenaries.GetSelected());
            }
            else
            {
                this.listBoxEquipmentMerc_Builds.ItemsSource = null;
                this.listBoxMercTalentsOff.ItemsSource = null;
                this.listBoxMercTalentsDef.ItemsSource = null;

            }
            this.listBoxRelicInventory_Builds.ItemsSource = ((Build)listBox_Builds.SelectedItem).SaveFile.Relics;
            this.listBoxTalents_Builds_Tree1.ItemsSource = ((Build)listBox_Builds.SelectedItem).SaveFile.HeroInfo.Class.Talents.GetTree1(true);
            this.listBoxTalents_Builds_Tree2.ItemsSource = ((Build)listBox_Builds.SelectedItem).SaveFile.HeroInfo.Class.Talents.GetTree2(true);
            this.listBoxHeroTalents_Builds.ItemsSource = ((Build)listBox_Builds.SelectedItem).SaveFile.HeroInfo.GetFilteredHeroTalents();
            TooltipHandler.SetHyperlink(textBlockBuildDesc, ((Build)listBox_Builds.SelectedItem).Description);
        }

        private void labelEquipmentBuilds_MouseDown(object sender, MouseButtonEventArgs e)
        {
            gridEquipmentBuilds.Visibility = gridEquipmentBuilds.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            collapseIndicatorEquipment.Content = gridEquipmentBuilds.Visibility == Visibility.Visible ? "▲" : "▼";
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            gridRelicsBuilds.Visibility = gridRelicsBuilds.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            collapseIndicatorRelics.Content = gridRelicsBuilds.Visibility == Visibility.Visible ? "▲" : "▼";
        }

        private void Grid_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            gridTalentsBuilds.Visibility = gridTalentsBuilds.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            collapseIndicatorTalents.Content = gridTalentsBuilds.Visibility == Visibility.Visible ? "▲" : "▼";
        }

        private void listBox_Uberlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine();
        }

        private void Border_MouseEnter_2(object sender, MouseEventArgs e)
        {
            try
            {
                Border border = sender as Border;
                Talent talent = (Talent)border.DataContext;
                border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(talent.HighlightBorder));
            }
            catch { }
        }

        private void Border_MouseLeave_2(object sender, MouseEventArgs e)
        {
            try
            {
                Border border = sender as Border;
                Talent talent = (Talent)border.DataContext;
                border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(talent.Border));
            }
            catch { }
        }

        private void Border_MouseEnter_3(object sender, MouseEventArgs e)
        {
            try
            {
                Border border = sender as Border;
                HeroTalent herotalent = (HeroTalent)border.DataContext;
                if (!herotalent.Selected)
                    border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b39239"));
            }
            catch { }
        }

        private void Border_MouseLeave_3(object sender, MouseEventArgs e)
        {
            try
            {
                Border border = sender as Border;
                HeroTalent herotalent = (HeroTalent)border.DataContext;
                if (!herotalent.Selected)
                    border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6e6b6a"));
            }
            catch { }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Border border = sender as Border;
                Talent talent = (Talent)border.DataContext;
                Grid grid = (Grid)border.Parent;
                TextBlock tb = (TextBlock)grid.Children[2];
                if (talent.Points == 0 && talent.Type == "Active")
                    foreach (ActiveTalent Atalent in this.SaveFileHandler.SelectedFile.HeroInfo.ActiveTalents)
                        if (Atalent.Talent == null)
                        {
                            Atalent.Talent = talent;
                            listBoxActiveTalents.Items.Refresh();
                            break;
                        }

                tb.Text = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? $"{talent.Points + 5}" : $"{talent.Points + 1}";
                talent.Points = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? talent.Points + 5 : talent.Points + 1;
                this.labelTalentPointsSpent.Text = this.SaveFileHandler.SelectedFile.HeroInfo.GetTalentPointsSpent() + "/" + this.SaveFileHandler.SelectedFile.HeroInfo.Level;
            }
            catch { }
        }

        private void Border_MouseRightButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Border border = sender as Border;
                Talent talent = (Talent)border.DataContext;
                Grid grid = (Grid)border.Parent;
                TextBlock tb = (TextBlock)grid.Children[2];
                tb.Text = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? talent.Points - 5 < 0 ? "0" : $"{talent.Points - 5}" : talent.Points == 0 ? "0" : $"{talent.Points - 1}";
                talent.Points = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? talent.Points - 5 < 0 ? 0 : talent.Points - 5 : talent.Points == 0 ? 0 : talent.Points - 1;
                this.labelTalentPointsSpent.Text = this.SaveFileHandler.SelectedFile.HeroInfo.GetTalentPointsSpent() + "/" + this.SaveFileHandler.SelectedFile.HeroInfo.Level;
                if (talent.Points > 0 || talent.Type != "Active") return;

                foreach (ActiveTalent activeTalent in this.SaveFileHandler.SelectedFile.HeroInfo.ActiveTalents)
                    if (activeTalent.Talent == talent)
                    {
                        activeTalent.Talent = null;
                        this.listBoxActiveTalents.Items.Refresh();
                    }

            }
            catch { }
        }

        private void Border_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Border border = sender as Border;
                HeroTalent talent = (HeroTalent)border.DataContext;
                if (talent.Selected)
                {
                    talent.Selected = false;
                    this.listBox_HeroTalents.Items.Refresh();
                    return;
                }
                foreach (HeroTalent heroTalent in this.SaveFileHandler.SelectedFile.HeroInfo.HeroTalents)
                    if (heroTalent.ID == talent.ID)
                        heroTalent.Selected = false;
                talent.Selected = true;
                this.listBox_HeroTalents.Items.Refresh();
            }
            catch { }
        }

        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste ||
                    e.Command == ApplicationCommands.ContextMenu)
            {
                e.Handled = true;
            }
        }

        private void Grid_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            gridItemFilter.Visibility = gridItemFilter.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            collapseIndicatorItemFilter.Content = gridItemFilter.Visibility == Visibility.Visible ? "▲" : "▼";
            gridItemFilter.ScrollToTop();
        }

        private void comboBoxItemFilterRarity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateItemFilter();
        }


        private void comboBoxItemFilterDamageType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateItemFilter();
        }

        public void UpdateHeroInfo()
        {
            this.gridSelectedSlot.DataContext = this.SaveFileHandler.SelectedFile;
            this.comboBoxClasses.SelectedItem = this.ClassHandler.GetClassFromName(this.SaveFileHandler.SelectedFile.HeroInfo.Class.Name);
            this.gridHeroInfo.DataContext = null;
            this.gridHeroInfo.DataContext = this.SaveFileHandler.SelectedFile.HeroInfo;
            this.textBoxHeroName.Text = this.SaveFileHandler.SelectedFile.HeroInfo.Name;
            this.textBoxGold.Text = this.SaveFileHandler.SelectedFile.HeroInfo.Hardcore ? this.SaveFileHandler.Shop.Gold_HC.ToString() : this.SaveFileHandler.Shop.Gold.ToString();
            this.textBoxRubies.DataContext = this.SaveFileHandler.Shop;
            this.textBoxMining.Text = this.SaveFileHandler.SelectedFile.HeroInfo.Hardcore ? this.SaveFileHandler.Shop.MiningLevel_HC.ToString() : this.SaveFileHandler.Shop.MiningLevel.ToString();
            this.textBoxChaosTower.DataContext = this.SaveFileHandler.SelectedFile.HeroInfo;
        }

        private void buttonHardcoreNo_Checked(object sender, RoutedEventArgs e)
        {
            if (this.SaveFileHandler == null) return;
            if (buttonHardcoreNo.IsChecked == true)
                this.SaveFileHandler.SelectedFile.HeroInfo.Hardcore = false;
            else
                this.SaveFileHandler.SelectedFile.HeroInfo.Hardcore = true;
            this.UpdateHeroInfo();
        }

        private void textBoxWormhole_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.SaveFileHandler.SelectedFile.HeroInfo.WormholeLevel = Convert.ToInt32(textBoxWormhole.Text);
            }
            catch { }
        }

        private void textBoxMining_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (this.SaveFileHandler.SelectedFile.HeroInfo.Hardcore)
                    this.SaveFileHandler.Shop.MiningLevel_HC = Convert.ToInt32(textBoxMining.Text);
                else
                    this.SaveFileHandler.Shop.MiningLevel = Convert.ToInt32(textBoxMining.Text);
            }
            catch { }
        }

        private void textBoxChaosTower_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.SaveFileHandler.SelectedFile.HeroInfo.ChaosTower = Convert.ToInt32(textBoxChaosTower.Text);
            }
            catch { }

        }

        private void textBoxGold_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (this.SaveFileHandler.SelectedFile.HeroInfo.Hardcore)
                    this.SaveFileHandler.Shop.Gold_HC = Convert.ToInt32(textBoxGold.Text);
                else
                    this.SaveFileHandler.Shop.Gold = Convert.ToInt32(textBoxGold.Text);
            }
            catch { }
        }

        private void textBoxRubies_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.SaveFileHandler.Shop.Rubies = Convert.ToInt32(textBoxRubies.Text);
            }
            catch { }

        }

        private void comboBoxItemFilterTier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateItemFilter();
        }

        private void EquipItem(Item item, bool scroll = false)
        {
            Slot slot = this.SlotHandler.SelectedItemSlot == null ? item.Slot : this.SlotHandler.SelectedItemSlot;
            Item equipped = controlSelectedEquipment.SelectedIndex == 0 ? this.SaveFileHandler.SelectedFile.Inventory.Equipment.GetItemFromSlot(slot) : this.SaveFileHandler.SelectedFile.Mercenaries.GetSelectedMerc().Equipment.GetItemFromSlot(slot);

            if (controlSelectedEquipment.SelectedIndex == 0)
                this.SaveFileHandler.SelectedFile.Inventory.Equipment.EquipItem(item, slot.ID);
            else
            {
                if (this.SaveFileHandler.SelectedFile.Mercenaries.GetSelectedMerc().Equipment.GetEquipmentSlot(item.Slot.ID).Invisible) return;
                this.SaveFileHandler.SelectedFile.Mercenaries.GetSelectedMerc().Equipment.EquipItem(item, slot.ID);
            }

            int index = controlSelectedInventory.SelectedIndex == 0 ? this.SaveFileHandler.SelectedFile.Inventory.InventoryItems.IndexOf(item) : this.SaveFileHandler.Shop.Stash.IndexOf(item);
            if (controlSelectedInventory.SelectedIndex == 0)
                this.SaveFileHandler.SelectedFile.Inventory.RemoveItem(item);
            else
                this.SaveFileHandler.Shop.Stash.Remove(item);

            if (equipped == null) return;

            if (controlSelectedInventory.SelectedIndex == 0)
                this.SaveFileHandler.SelectedFile.Inventory.InventoryItems.Insert(index >= this.SaveFileHandler.SelectedFile.Inventory.InventoryItems.Count || index == -1 ? this.SaveFileHandler.SelectedFile.Inventory.InventoryItems.Count : index, equipped);
            else
                this.SaveFileHandler.Shop.Stash.Insert(index >= this.SaveFileHandler.Shop.Stash.Count || index == -1 ? this.SaveFileHandler.Shop.Stash.Count : index, equipped);

            listBox_Inventory.Items.Refresh();
            this.UpdateEquippedItems();
            this.UpdateForgeLists();
            if (scroll)
                this.listBox_Inventory.ScrollIntoView(listBox_Inventory.Items[listBox_Inventory.Items.Count - 1]);
        }

        private void listBox_Inventory_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBox_Inventory.SelectedItem == null || e.ChangedButton == MouseButton.Right) return;
            this.EquipItem((Item)listBox_Inventory.SelectedItem);
        }

        private void ContextMenu_EquipItem_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuItem.IsOpen = false;
            this.EquipItem((Item)listBox_Inventory.SelectedItem);
        }

        private void ContextMenu_DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuItem.IsOpen = false;
            if (controlSelectedInventory.SelectedIndex == 0)
                this.SaveFileHandler.SelectedFile.Inventory.RemoveItem((Item)listBox_Inventory.SelectedItem);
            else
                this.SaveFileHandler.Shop.Stash.Remove((Item)listBox_Inventory.SelectedItem);
            UpdateForgeLists();
        }

        private void ContextMenu_EditItem_Click(object sender, RoutedEventArgs e)
        {
            Item Item = (Item)listBox_Inventory.SelectedItem;
            foreach (TabItem item in MenuControl.Items)
                if (item.Header.Equals("Forge"))
                {
                    MenuControl.SelectedItem = item;
                    break;
                }
            listBox_InventoryForge.SelectedItem = Item;
            listBox_InventoryForge.ScrollIntoView(listBox_InventoryForge.SelectedItem);
            listBox_InventoryForge.Focus();
            ContextMenuItem.IsOpen = false;
        }

        private void listBox_Inventory_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (listBox_Inventory.SelectedItem == null)
                e.Handled = true;


        }

        private void ContextMenu_ClearItems_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuItem.IsOpen = false;
            if (controlSelectedInventory.SelectedIndex == 0)
                SaveFileHandler.SelectedFile.Inventory.InventoryItems.Clear();
            else
                SaveFileHandler.Shop.Stash.Clear();
            labelInventoryCount.Content = listBox_Inventory.Items.Count + "/100";
            labelInventoryCountForge.Content = listBox_Inventory.Items.Count + "/100";
            this.RefreshListboxes();
        }

        private void ContextMenu_UnEquipEquipment_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuEquipment.IsOpen = false;
            Item Item = ((EquipmentSlot)listBox_Equippeditems.SelectedItem).Item;

            if (controlSelectedInventory.SelectedIndex == 0)
                this.SaveFileHandler.SelectedFile.Inventory.AddItem(Item, false);
            else
                this.SaveFileHandler.Shop.AddItemToStash(Item);

            ((EquipmentSlot)listBox_Equippeditems.SelectedItem).SetItem(null);
            this.RefreshListboxes();
            listBox_Inventory.ScrollIntoView(listBox_Inventory.Items[listBox_Inventory.Items.Count - 1]);
        }

        private void listBox_Equippeditems_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (listBox_Equippeditems.SelectedItem == null)
                e.Handled = true;
        }

        private void ContextMenu_EditEquipment_Click(object sender, RoutedEventArgs e)
        {
            EquipmentSlot Slot = (EquipmentSlot)listBox_Equippeditems.SelectedItem;
            foreach (TabItem item in MenuControl.Items)
                if (item.Header.Equals("Forge"))
                {
                    MenuControl.SelectedItem = item;
                    break;
                }
            listBox_EquippeditemsForge.SelectedItem = Slot;
            listBox_EquippeditemsForge.ScrollIntoView(listBox_EquippeditemsForge.SelectedItem);
            listBox_EquippeditemsForge.Focus();
            ContextMenuEquipment.IsOpen = false;
        }

        private void ContextMenu_DeleteEquipment_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuEquipment.IsOpen = false;
            ((EquipmentSlot)listBox_Equippeditems.SelectedItem).SetItem(null);
            this.UpdateEquippedItems();
            this.UpdateForgeLists();
        }

        private void ContextMenu_ClearEquipment_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuEquipment.IsOpen = false;
            if (controlSelectedEquipment.SelectedIndex == 0)
                this.SaveFileHandler.SelectedFile.Inventory.Equipment.Clear();
            else
                this.SaveFileHandler.SelectedFile.Mercenaries.GetSelectedMerc().Equipment.Clear();
            this.RefreshListboxes();
        }

        private void listBox_Items_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (listBox_Items.SelectedItem == null)
                e.Handled = true;
        }

        private void ContextMenu_EquipItem_Click_1(object sender, RoutedEventArgs e)
        {
            ContextMenuItemlist.IsOpen = false;
            Item item = ((Item)listBox_Items.SelectedItem).DeepCopy();
            if (this.ConfigHandler.Config.NewItem.RollID == -1 && !item.Favorite) item.RollID = random.Next(0, 10001);
            this.EquipItem(item, true);
            //this.SaveFileHandler.SelectedFile.Inventory.Equipment.EquipItem(item, this.SlotHandler.SelectedItemSlot == null ? null : this.SlotHandler.SelectedItemSlot.ID);
            //this.SelectEquipmentSlot(this.SlotHandler.SelectedItemSlot == null ? item.Slot : this.SlotHandler.SelectedItemSlot);
        }

        private void ContextMenu_Add1ToInventory_Click(object sender, RoutedEventArgs e)
        {
            this.AddItemToInventory((Item)listBox_Items.SelectedItem);
            ContextMenuItemlist.IsOpen = false;
        }

        private void ContextMenu_Add20ToInventory_Click(object sender, RoutedEventArgs e)
        {
            this.AddItemToInventory((Item)listBox_Items.SelectedItem, 10);
            ContextMenuItemlist.IsOpen = false;
        }

        private void ContextMenu_AddAll_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuItemlist.IsOpen = false;
            try
            {
                if (listBox_Items.SelectedItem == null) return;

                foreach (Item Item in listBox_Items.Items)
                    if ((!((Item)listBox_Items.SelectedItem).Favorite && !Item.Favorite) || (((Item)listBox_Items.SelectedItem).Favorite && Item.Favorite))
                        this.AddItemToInventory(Item, 1, false);

                this.RefreshListboxes();
                listBox_Inventory.ScrollIntoView(SaveFileHandler.SelectedFile.Inventory.InventoryItems.Last());
            }
            catch { }
        }

        private void comboBoxRelicFilterStat_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxRelicFilterStat.ItemsSource = this.StatHandler.GetRelicStats();
            comboBoxRelicFilterStat.SelectedIndex = 0;
        }

        private void buttonSaveRelicTemplate_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = "Relic Templates (*.relic)|*.relic", InitialDirectory = Environment.GetEnvironmentVariable("LocalAppData") + @"\Hero_Siege\hseditor\relics\" };
            if (saveFileDialog.ShowDialog() == true)
                this.RelicHandler.SaveTemplate(saveFileDialog.FileName);
        }

        private void buttonLoadRelicTemplate_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Relic Templates (*.relic)|*.relic", InitialDirectory = Environment.GetEnvironmentVariable("LocalAppData") + @"\Hero_Siege\hseditor\relics\" };
            if (openFileDialog.ShowDialog() == true)
            {
                this.RelicHandler.LoadTemplate(openFileDialog.FileName);
                this.RefreshListboxes();
            }
        }



        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.SaveFileHandler.SelectedFile.Save();
            this.Refresh();
            MessageBox mb = new MessageBox("Saving successful!", "Changes were successfully applied.", "OK");
            mb.ShowDialog();

        }

        private void comboBoxRelicFilterStat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateRelicFilter();
        }

        private void buttonImportBuild_Click(object sender, RoutedEventArgs e)
        {
            this.SaveFileHandler.SelectedFile.ImportBuild((Build)listBox_Builds.SelectedItem);
            MessageBox message = new MessageBox("Import successful!", "The selected build was successfully imported.", "OK");
            message.ShowDialog();
        }



        private void buttonResetTalents_Click(object sender, RoutedEventArgs e)
        {
            foreach (Talent talent in this.SaveFileHandler.SelectedFile.HeroInfo.Class.Talents.TalentList)
                talent.Points = 0;
            foreach (ActiveTalent activeTalent in this.SaveFileHandler.SelectedFile.HeroInfo.ActiveTalents)
                activeTalent.Talent = null;
            this.listBoxActiveTalents.Items.Refresh();
            this.listBox_Talents1.Items.Refresh();
            this.listBox_Talents2.Items.Refresh();
            this.labelTalentPointsSpent.Text = this.SaveFileHandler.SelectedFile.HeroInfo.GetTalentPointsSpent() + "/" + this.SaveFileHandler.SelectedFile.HeroInfo.Level;
        }

        private void buttonItemFilterRarity_Click(object sender, RoutedEventArgs e)
        {
            var cm = ContextMenuService.GetContextMenu(sender as DependencyObject);
            if (cm == null)
            {
                return;
            }
            cm.Placement = PlacementMode.Bottom;
            cm.PlacementTarget = sender as UIElement;
            cm.IsOpen = true;
        }

        private void ItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            ItemsControl ic = sender as ItemsControl;
            ic.ItemsSource = this.RarityHandler.RaritiesFiltered;
        }

        private void gridItemFilter_Loaded(object sender, RoutedEventArgs e)
        {
            gridItemFilter.DataContext = this.ItemHandler.Filter;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.UpdateItemFilter();
        }

        private void buttonResetFilterAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (RarityFilter item in this.ItemHandler.Filter.RarityFilter)
                item.Selected = false;

            foreach (TierFilter item in this.ItemHandler.Filter.TierFilter)
                item.Selected = false;

            foreach (StatFilter item in this.ItemHandler.Filter.StatFilter)
                item.Selected = false;

            listBoxItemFilterRarity.Items.Refresh();
            listBoxItemFilterStat.Items.Refresh();
            listBoxItemFilterTier.Items.Refresh();
            this.UpdateItemFilter();
        }

        private void buttonResetFilterStats_Click(object sender, RoutedEventArgs e)
        {
            foreach (StatFilter item in this.ItemHandler.Filter.StatFilter)
                item.Selected = false;

            listBoxItemFilterStat.Items.Refresh();
            this.UpdateItemFilter();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (((TextBox)sender).Text == "")
                ((TextBox)sender).Text = "1";
        }

        private void textBoxHeroName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxHeroName.Text == "")
                textBoxHeroName.Text = this.ConfigHandler.Config.NewChar.Name;
        }

        private void textBoxUberFilterSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.UpdateUberFilter();
        }

        private void comboBoxUberFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateUberFilter();
        }

        private void comboBoxUberFilterType_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxUberFilterType.ItemsSource = this.UberHandler.UberTypes;
        }

        private void listBox_Builds_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void textBoxRuneFilterSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.UpdateRuneFilter();
        }

        private void comboBoxRuneFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateRuneFilter();
        }

        private void comboBoxRuneFilterType_Loaded(object sender, RoutedEventArgs e)
        {
            this.comboBoxRuneFilterType.ItemsSource = this.RuneHandler.RuneTypes;
        }

        private void MenuControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void Border_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TextBox tb = (TextBox)((Border)((StackPanel)((Border)sender).Child).Children[1]).Child;
                tb.Text = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? $"{Convert.ToInt32(tb.Text) + 1000}" : $"{Convert.ToInt32(tb.Text) + 100}";
                tb.Focus();
                Keyboard.ClearFocus();
            }
            catch { }
        }

        private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((Talent)((Polygon)sender).DataContext).Type != "Active" || this.SaveFileHandler.SelectedFile.HeroInfo.Class.Talents.GetTalentFromID(((Talent)((Polygon)sender).DataContext).ID).Points < 1) return;
            Talent talent = (Talent)((Polygon)sender).DataContext;
            DataObject dataObject = new DataObject();
            dataObject.SetData("ID", talent.ID);
            DragDrop.DoDragDrop((Polygon)sender, dataObject, DragDropEffects.Move);
        }

        private void Border_Drop(object sender, DragEventArgs e)
        {
            ((ActiveTalent)((Border)sender).DataContext).Talent = this.SaveFileHandler.SelectedFile.HeroInfo.Class.Talents.GetTalentFromID(Convert.ToInt32(e.Data.GetData("ID")));
            listBoxActiveTalents.Items.Refresh();
        }

        private void Border_DragOver(object sender, DragEventArgs e)
        {
            Image image = (Image)((Border)sender).Child;
            Talent talent = this.SaveFileHandler.SelectedFile.HeroInfo.Class.Talents.GetTalentFromID(Convert.ToInt32(e.Data.GetData("ID")));
            image.Source = new BitmapImage(new Uri(talent.Sprite));
            image.Opacity = 0.4;
        }

        private void Border_DragLeave(object sender, DragEventArgs e)
        {
            Image image = (Image)((Border)sender).Child;
            ActiveTalent talent = (ActiveTalent)((Border)sender).DataContext;
            image.Source = talent.Talent == null ? null : new BitmapImage(new Uri(talent.Talent.Sprite));
            image.Opacity = 1;
        }

        private void Border_MouseRightButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            ActiveTalent talent = (ActiveTalent)((Border)sender).DataContext;
            talent.Talent = null;
            this.listBoxActiveTalents.Items.Refresh();
        }

        private void Border_MouseRightButtonDown_3(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (TextBox)((Border)((StackPanel)((Border)sender).Child).Children[1]).Child;
            tb.Text = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? Convert.ToInt32(tb.Text) - 1000 < 0 ? "0" : $"{Convert.ToInt32(tb.Text) - 1000}" : Convert.ToInt32(tb.Text) - 100 < 0 ? "0" : $"{Convert.ToInt32(tb.Text) - 100}";
            tb.Focus();
            Keyboard.ClearFocus();
        }

        private void TextBlock_Loaded(object sender, EventArgs e)
        {
            TooltipHandler.SetToolTip((TextBlock)sender);
        }

        private void buttonSetUbers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (Uber uber in listBoxUberInventory.Items)
                    uber.Amount = uber.Amount + Int64.Parse(textBoxUberAmount.Text) <= Int32.MaxValue ? Int32.Parse(textBoxUberAmount.Text) : Int32.MaxValue;
                listBoxUberInventory.Items.Refresh();
            }
            catch { }
        }

        private void buttonAddUbers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (Uber uber in listBoxUberInventory.Items)
                    uber.Amount += uber.Amount + Int64.Parse(textBoxUberAmount.Text) <= Int32.MaxValue ? Int32.Parse(textBoxUberAmount.Text) : Int32.MaxValue - uber.Amount;
                listBoxUberInventory.Items.Refresh();
            }
            catch { }
        }

        private void buttonSetRunes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (Rune Rune in listBoxRuneInventory.Items)
                    Rune.Amount = Rune.Amount + Int64.Parse(textBoxRuneAmount.Text) <= Int32.MaxValue ? Int32.Parse(textBoxRuneAmount.Text) : Int32.MaxValue;
                listBoxRuneInventory.Items.Refresh();
            }
            catch { }
        }

        private void buttonAddRunes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (Rune Rune in listBoxRuneInventory.Items)
                    Rune.Amount += Rune.Amount + Int64.Parse(textBoxRuneAmount.Text) <= Int32.MaxValue ? Int32.Parse(textBoxRuneAmount.Text) : Int32.MaxValue - Rune.Amount;
                listBoxRuneInventory.Items.Refresh();
            }
            catch { }
        }

        private void Border_MouseEnter_4(object sender, MouseEventArgs e)
        {
            foreach (Talent talent in this.SaveFileHandler.SelectedFile.HeroInfo.Class.Talents.GetActiveTalents())
                if (talent.Points > 0)
                {
                    if (((ActiveTalent)((Border)sender).DataContext).Talent != null)
                    {
                        if (talent == ((ActiveTalent)((Border)sender).DataContext).Talent) talent.Border = "#e3b02d";
                    }
                    else
                        talent.Border = "#e3b02d";
                }
            listBox_Talents1.Items.Refresh();
            listBox_Talents2.Items.Refresh();
        }

        private void Border_MouseLeave_4(object sender, MouseEventArgs e)
        {
            foreach (Talent talent in this.SaveFileHandler.SelectedFile.HeroInfo.Class.Talents.GetActiveTalents())
                talent.Border = "#b39239";
            listBox_Talents1.Items.Refresh();
            listBox_Talents2.Items.Refresh();
        }

        private void listBox_Slots_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            if (lb.SelectedItem == null) return;
            switch (e.Key)
            {
                case Key.Down:
                    if (lb.SelectedIndex == lb.Items.Count - 1) { e.Handled = true; return; }
                    lb.SelectedIndex += 1;
                    lb.ScrollIntoView(lb.SelectedItem);
                    e.Handled = true;
                    break;
                case Key.Up:
                    if (lb.SelectedIndex == 0) { e.Handled = true; return; }
                    lb.SelectedIndex -= 1;
                    lb.ScrollIntoView(lb.SelectedItem);
                    e.Handled = true;
                    break;
                case Key.Enter:
                    this.AddItemToInventory((Item)listBox_Items.SelectedItem, Keyboard.IsKeyDown(Key.LeftShift) ? 10 : 1);
                    break;
                case Key.Delete:
                    if (!((Item)listBox_Items.SelectedItem).Favorite) return;
                    this.ConfigHandler.RemoveFavorite((Item)listBox_Items.SelectedItem);
                    break;
                default:
                    e.Handled = true;
                    return;
            }
        }

        private void TextBlock_Initialized(object sender, EventArgs e)
        {
            Stat stat = (Stat)((TextBlock)sender).DataContext;
            foreach (Stat Stat in this.ItemHandler.Filter.GetFilteredStats())
            {
                if (Stat.Name == stat.Name && Stat.Name != "Random Stat")
                {
                    ((TextBlock)sender).Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3784db"));
                    return;
                }
            };
        }

        private void checkBoxNewItemRollRandom_Checked(object sender, RoutedEventArgs e)
        {
            textBoxNewItemRollID.IsEnabled = checkBoxNewItemRollRandom.IsChecked == false;
        }

        public void RefreshSettings()
        {
            if (this.ConfigHandler == null) return;
            buttonSettingsHardcoreNo.IsChecked = true;
            buttonSettingsGenericRaritiesNo.IsChecked = true;
            gridSettings.DataContext = this.ConfigHandler.Config;
            checkBoxNewItemRollRandom.IsChecked = this.ConfigHandler.Config.NewItem.RollID == -1;
            textBoxNewItemRollID.Text = this.ConfigHandler.Config.NewItem.RollID == -1 ? "0" : this.ConfigHandler.Config.NewItem.RollID.ToString();
            this.Width = this.ConfigHandler.Config.Resolution.X;
            this.Height = this.ConfigHandler.Config.Resolution.Y;
            comboBoxResolution.SelectedIndex = this.ConfigHandler.Config.Resolution.X == 1280 ? 0 : this.ConfigHandler.Config.Resolution.X == 1200 ? 1 : 2;

            if (this.ItemHandler == null) return;
            foreach (Item item in this.ItemHandler.AllItems)
                item.Forge(this.ConfigHandler.Config.NewItem.ItemLevel, this.ConfigHandler.Config.NewItem.Quality, this.ConfigHandler.Config.NewItem.UpgradeLevel, this.ConfigHandler.Config.NewItem.RollID, this.ConfigHandler.Config.NewItem.Ability, this.ConfigHandler.Config.NewItem.AbilityLevel, this.ConfigHandler.Config.NewItem.Sockets.GetRuneList());
            foreach (Item item in this.ItemHandler.Generics)
                item.Forge(this.ConfigHandler.Config.NewItem.ItemLevel, this.ConfigHandler.Config.NewItem.Quality, this.ConfigHandler.Config.NewItem.UpgradeLevel, this.ConfigHandler.Config.NewItem.RollID, this.ConfigHandler.Config.NewItem.Ability, this.ConfigHandler.Config.NewItem.AbilityLevel, this.ConfigHandler.Config.NewItem.Sockets.GetRuneList());
            listBox_Items.Items.Refresh();

            foreach (RarityFilter rarity in this.ItemHandler.Filter.RarityFilter)
            {
                rarity.Enabled = rarity.Rarity.EditorID < 6 ? this.ConfigHandler.Config.ShowGenericRarities : true;
                UpdateItemFilter();
            }
            listBoxItemFilterRarity.Items.Refresh();
            this.Refresh();
        }

        private void buttonSettingsHardcoreNo_Checked(object sender, RoutedEventArgs e)
        {
            if (buttonSettingsHardcoreNo.IsChecked == true)
                this.SaveFileHandler.SelectedFile.HeroInfo.Hardcore = false;
            else
                this.SaveFileHandler.SelectedFile.HeroInfo.Hardcore = true;
            this.UpdateHeroInfo();
        }

        private void comboBoxSettingsClasses_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxSettingsClasses.ItemsSource = this.ClassHandler.Classes_Filtered;
        }

        private void comboBoxNewItemAbilities_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxNewItemAbilities.ItemsSource = this.AbilityHandler.Abilities;
        }

        private void ButtonApplySettings(object sender, RoutedEventArgs e)
        {
            this.ConfigHandler.Config.NewItem.RollID = this.checkBoxNewItemRollRandom.IsChecked == true ? -1 : Convert.ToInt32(this.textBoxNewItemRollID.Text);
            this.ConfigHandler.Config.Resolution = new Point(Convert.ToInt32(((ComboBoxItem)comboBoxResolution.SelectedItem).Content.ToString().Split('x')[0]), Convert.ToInt32(((ComboBoxItem)comboBoxResolution.SelectedItem).Content.ToString().Split('x')[1]));
            this.ConfigHandler.WriteConfig();
            this.ConfigHandler.LoadConfig();
            this.RefreshSettings();
            MessageBox mb = new MessageBox("Config saved!", "Your config was successfully saved.", "OK");
            mb.ShowDialog();
        }

        private void ButtonReloadSettings(object sender, RoutedEventArgs e)
        {
            this.ConfigHandler.LoadConfig();
            this.RefreshSettings();
        }

        private void listBox_Inventory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine();
        }

        private void ButtonSetConfigToDefaults(object sender, RoutedEventArgs e)
        {
            this.ConfigHandler.WriteConfig(true);
            this.ConfigHandler.LoadConfig();
            this.RefreshSettings();
        }

        private void ClearItemSearchbar(object sender, MouseButtonEventArgs e)
        {
            textBoxItemlistSearch.Text = "";
            textBoxItemlistSearch.Focus();
        }

        private void ClearRelicSearchbar(object sender, MouseButtonEventArgs e)
        {
            textBoxRelicListSearch.Text = "";
            textBoxRelicListSearch.Focus();
        }

        private void ClearUberSearchbar(object sender, MouseButtonEventArgs e)
        {
            textBoxUberFilterSearch.Text = "";
            textBoxUberFilterSearch.Focus();
        }

        private void ClearRuneSearchbar(object sender, MouseButtonEventArgs e)
        {
            textBoxRuneFilterSearch.Text = "";
            textBoxRuneFilterSearch.Focus();
        }

        private void ContextMenu_ClearAllItems_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuItem.IsOpen = false;
            this.ClearAll();
            //if (controlSelectedInventory.SelectedIndex == 0)
            //    this.SaveFileHandler.SelectedFile.Inventory.InventoryItems.Clear();
            //else
            //    this.SaveFileHandler.Shop.Stash.Clear();

            //if (controlSelectedEquipment.SelectedIndex == 0)
            //    this.SaveFileHandler.SelectedFile.Inventory.Equipment.Clear();
            //else
            //    this.SaveFileHandler.SelectedFile.Mercenaries.GetSelectedMerc().Equipment.Clear();
            //this.RefreshListboxes();
        }

        public void ClearAll()
        {
            this.SaveFileHandler.SelectedFile.Inventory.InventoryItems = new System.Collections.ObjectModel.ObservableCollection<Item>();
            this.SaveFileHandler.Shop.Stash = new System.Collections.ObjectModel.ObservableCollection<Item>();
            this.SaveFileHandler.SelectedFile.Inventory.Equipment.Clear();
            this.SaveFileHandler.SelectedFile.Mercenaries.GetSelectedMerc().Equipment.Clear();
            this.RefreshListboxes();

        }
        private void ContextMenu_ClearAllItems_Click_1(object sender, RoutedEventArgs e)
        {
            ContextMenuEquipment.IsOpen = false;
            this.ClearAll();
            //if (controlSelectedInventory.SelectedIndex == 0)
            //    this.SaveFileHandler.SelectedFile.Inventory.InventoryItems = new System.Collections.ObjectModel.ObservableCollection<Item>();
            //else
            //    this.SaveFileHandler.Shop.Stash = new System.Collections.ObjectModel.ObservableCollection<Item>();

            //if (controlSelectedEquipment.SelectedIndex == 0)
            //    this.SaveFileHandler.SelectedFile.Inventory.Equipment.Clear();
            //else
            //    this.SaveFileHandler.SelectedFile.Mercenaries.GetSelectedMerc().Equipment.Clear();

            //this.RefreshListboxes();
        }

        private void buttonForgeSaveAll_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_EquippeditemsForge.SelectedItem == null && listBox_InventoryForge.SelectedItem == null) return;

            Item item = listBox_InventoryForge.SelectedItem == null ? ((EquipmentSlot)listBox_EquippeditemsForge.SelectedItem).Item : (Item)listBox_InventoryForge.SelectedItem;
            Item item2 = (Item)gridForgeAttributes.DataContext;
            ListType listType;
            listType = listBox_InventoryForge.SelectedItem == null ? ((EquipmentSlot)listBox_EquippeditemsForge.SelectedItem).Mercenary ? ListType.Mercenary : ListType.Equipment : item.Stash ? ListType.Stash : ListType.Inventory;

            ForgePrompt prompt = new ForgePrompt(item.Quality != item2.Quality, item.UpgradeLevel != item2.UpgradeLevel, item.ItemLevel != item2.ItemLevel, item.RollID != item2.RollID, item.Ability != item2.Ability, !item.Sockets.Compare(item2.Sockets.GetRuneList()), listType);
            prompt.ShowDialog();

            if (prompt.Cancel) return;

            List<Item> list = new List<Item>();
            if (prompt.Inventory) list.AddRange(this.SaveFileHandler.SelectedFile.Inventory.InventoryItems);
            if (prompt.Equipment) list.AddRange(this.SaveFileHandler.SelectedFile.Inventory.Equipment.GetItems());
            if (prompt.Mercenary) list.AddRange(this.SaveFileHandler.SelectedFile.Mercenaries.GetSelectedMerc().Equipment.GetItems());
            if (prompt.Stash) list.AddRange(this.SaveFileHandler.Shop.Stash);

            foreach (Item Item in list)
            {
                Item.Forge(prompt.ItemLevel ? item2.ItemLevel : Item.ItemLevel, prompt.Quality ? item2.Quality : Item.Quality, prompt.UpgradeLevel ? item2.UpgradeLevel : Item.UpgradeLevel, prompt.Roll ? item2.RollID : Item.RollID, prompt.Ability ? item2.Ability : Item.Ability, prompt.Ability ? item2.AbilityLevel : Item.AbilityLevel, prompt.Runes ? item2.Sockets.GetRuneList() : Item.Sockets.GetRuneList());
            }

            this.RefreshListboxes();
        }


        private void listBox_Equippeditems_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (listBox_Equippeditems.SelectedItem == null) return;
            ListBox lb = (ListBox)sender;
            switch (e.Key)
            {
                case Key.Delete:
                    ((EquipmentSlot)listBox_Equippeditems.SelectedItem).SetItem(null);
                    UpdateEquippedItems();
                    UpdateForgeLists();
                    break;
                case Key.Down:
                    if (lb.SelectedIndex == lb.Items.Count - 1) { e.Handled = true; return; }
                    lb.SelectedIndex += 1;
                    lb.ScrollIntoView(lb.SelectedItem);
                    e.Handled = true;
                    break;
                case Key.Up:
                    if (lb.SelectedIndex == 0) { e.Handled = true; return; }
                    lb.SelectedIndex -= 1;
                    lb.ScrollIntoView(lb.SelectedItem);
                    e.Handled = true;
                    break;
                default:
                    e.Handled = true;
                    return;
            }
        }

        private void ContextMenu_Delete_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuSlots.IsOpen = false;
            try
            {
                SaveFile sf = listBox_Slots.SelectedItem as SaveFile;
                File.Delete(sf.Path);
                this.Refresh();
            }
            catch { }
        }

        private void ContextMenu_Paste_Loaded(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Visibility = this.SaveFileHandler.Copy == null ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ContextMenu_Copy_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuSlots.IsOpen = false;
            try
            {
                if (listBox_Slots.SelectedItem == null) return;
                this.SaveFileHandler.Copy = (SaveFile)listBox_Slots.SelectedItem;
            }
            catch { }
        }

        private void ContextMenu_Paste_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuSlots.IsOpen = false;
            try
            {
                File.Copy(this.SaveFileHandler.Copy.Path, ((SaveFile)this.listBox_Slots.SelectedItem).Path);
                this.Refresh();
            }
            catch { }
        }

        private void ContextMenu_AddToFavorites_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuItemForge.IsOpen = false;
            try
            {
                this.ConfigHandler.AddFavorite((Item)listBox_InventoryForge.SelectedItem);
            }
            catch { }
        }

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void listBox_Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox_Items.SelectedItem == null) return;
            ContextMenuItemlist.DataContext = (Item)listBox_Items.SelectedItem;
        }

        private void ContextMenu_RemoveFavorite_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuItemlist.IsOpen = false;
            try
            {
                this.ConfigHandler.RemoveFavorite((Item)listBox_Items.SelectedItem);
            }
            catch { }
        }

        private void ContextMenu_AddToFavorites_Click_1(object sender, RoutedEventArgs e)
        {
            ContextMenuEquipmentForge.IsOpen = false;
            try
            {
                this.ConfigHandler.AddFavorite(((EquipmentSlot)listBox_EquippeditemsForge.SelectedItem).Item);
            }
            catch { }
        }

        private void listBox_Slots_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (listBox_Slots.SelectedItem == null) return;
            SaveFile sf = listBox_Slots.SelectedItem as SaveFile;
            if (sf.NewChar && this.SaveFileHandler.Copy == null)
                e.Handled = true;
        }

        private void ContextMenu_AddSet_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuItemlist.IsOpen = false;
            try
            {
                Item Item = listBox_Items.SelectedItem as Item;

                foreach (Item item in this.ItemHandler.AllItems)
                    if (item.Set == Item.Set && item.Rarity == Item.Rarity)
                        this.AddItemToInventory(item, 1, false);

                this.listBox_Inventory.Items.Refresh();
                listBox_Inventory.ScrollIntoView(SaveFileHandler.SelectedFile.Inventory.InventoryItems.Last());
            }
            catch { }
        }

        private void mainBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Border border = sender as Border;
                MercenaryTalent talent = (MercenaryTalent)border.DataContext;
                Grid grid = (Grid)border.Parent;
                TextBlock tb = (TextBlock)grid.Children[1];
                tb.Text = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? $"{talent.Points + 5}" : $"{talent.Points + 1}";
                talent.Points = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? talent.Points + 5 : talent.Points + 1;
                this.labelMercTalentPointsSpent.Text = this.SaveFileHandler.SelectedFile.Mercenaries.Talents.GetPointsSpent() + "/" + this.SaveFileHandler.SelectedFile.HeroInfo.Level;
            }
            catch { }
        }

        private void mainBorder_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Border border = sender as Border;
                MercenaryTalent talent = (MercenaryTalent)border.DataContext;
                Grid grid = (Grid)border.Parent;
                TextBlock tb = (TextBlock)grid.Children[1];
                tb.Text = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? talent.Points - 5 < 0 ? "0" : $"{talent.Points - 5}" : talent.Points == 0 ? "0" : $"{talent.Points - 1}";
                talent.Points = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? talent.Points - 5 < 0 ? 0 : talent.Points - 5 : talent.Points == 0 ? 0 : talent.Points - 1;
                this.labelMercTalentPointsSpent.Text = this.SaveFileHandler.SelectedFile.Mercenaries.Talents.GetPointsSpent() + "/" + this.SaveFileHandler.SelectedFile.HeroInfo.Level;
            }
            catch { }
        }

        private void textBoxMercName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (this.SaveFileHandler.SelectedFile.Mercenaries.GetSelected() == 0)
                    this.SaveFileHandler.Shop.MercenaryName_Melee = textBoxMercName.Text;
                else
                    this.SaveFileHandler.Shop.MercenaryName_Ranged = textBoxMercName.Text;
            }
            catch { }
        }

        private void buttonMeleeMerc_Checked(object sender, RoutedEventArgs e)
        {
            if (this.SaveFileHandler == null) return;
            this.SaveFileHandler.SelectedFile.Mercenaries.SetSelected(0);
            this.UpdateMercenaryInfo();
        }

        private void buttonRangedMerc_Checked(object sender, RoutedEventArgs e)
        {
            this.SaveFileHandler.SelectedFile.Mercenaries.SetSelected(1);
            this.UpdateMercenaryInfo();
        }

        private void buttonResetMercTalents_Click(object sender, RoutedEventArgs e)
        {
            foreach (MercenaryTalent talent in this.SaveFileHandler.SelectedFile.Mercenaries.Talents.AllTalents)
                talent.Points = 0;
            UpdateMercenaryInfo();
        }

        private void textBoxCompanionName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.SaveFileHandler.Shop.CompanionName = this.textBoxCompanionName.Text;
            }
            catch { }
        }

        private void textBoxCompanionID_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.SaveFileHandler.Shop.CompanionID = Convert.ToInt32(this.textBoxCompanionID.Text);
            }
            catch { }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SlotHandler == null) return;
            this.UpdateEquippedItems();
            if (controlSelectedEquipment.SelectedIndex == 1 && this.SlotHandler.SelectedItemSlot != null && this.SlotHandler.SelectedItemSlot.ID != 0 && this.SlotHandler.SelectedItemSlot.ID != 1 && this.SlotHandler.SelectedItemSlot.ID != 3 && this.SlotHandler.SelectedItemSlot.ID != 7)
                this.SelectEquipmentSlot(null);
            else
                this.SelectEquipmentSlot(this.SlotHandler.SelectedItemSlot);
        }

        private void controlSelectedInventory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateInventory();
            if (buttonAddToInventory == null) return;
            this.buttonAddToInventory.Content = controlSelectedInventory.SelectedIndex == 0 ? "Add to Inventory" : "Add to Stash";
        }

        private void ContextMenu_PinItem_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuItemlist.IsOpen = false;
            ItemDisplay itemDisplay = new ItemDisplay((Item)listBox_Items.SelectedItem);
            itemDisplay.Show();
        }

        private void ContextMenu_PinItem_Click_1(object sender, RoutedEventArgs e)
        {
            ContextMenuEquipment.IsOpen = false;
            ItemDisplay itemDisplay = new ItemDisplay(((EquipmentSlot)listBox_Equippeditems.SelectedItem).Item);
            itemDisplay.Show();
        }

        private void ContextMenu_PinItem_Click_2(object sender, RoutedEventArgs e)
        {
            ContextMenuItem.IsOpen = false;
            ItemDisplay itemDisplay = new ItemDisplay((Item)listBox_Inventory.SelectedItem);
            itemDisplay.Show();
        }

        private void controlSelectedBuilds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateBuildsList();
            if (buttonSaveBuild == null) return;
            buttonSaveBuild.Content = controlSelectedBuilds.SelectedIndex == 0 ? "Post Build" : "Save Build";
            buttonSaveBuild.IsEnabled = controlSelectedBuilds.SelectedIndex == 1;
        }

        private void TabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
            TabControl tc = (TabControl)sender;
            int index = tc.SelectedIndex;
            if (index == -1) return;
            tc.SelectedIndex = -1;

            if (index == 0)
            {
                if (this.SaveFileHandler.SelectedFile.Inventory.InventoryItems.Count == 0) return;
                this.listBox_InventoryForge.ScrollIntoView(this.SaveFileHandler.SelectedFile.Inventory.InventoryItems[0]);
            }
            else
            {
                if (this.SaveFileHandler.Shop.Stash.Count == 0) return;
                this.listBox_InventoryForge.ScrollIntoView(this.SaveFileHandler.Shop.Stash[0]);
            }
        }

        private void listBox_EquippeditemsForge_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (listBox_EquippeditemsForge.SelectedItem == null || e.OriginalSource.GetType() == typeof(ScrollViewer)) e.Handled = true;
        }

        private void listBox_InventoryForge_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (listBox_InventoryForge.SelectedItem == null) e.Handled = true;
        }

        private void listBox_EquippeditemsForge_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.listBox_InventoryForge.SelectedItem == null) return;
            Item Item = (Item)listBox_InventoryForge.SelectedItem;
            ListBox lb = (ListBox)sender;
            switch (e.Key)
            {
                case Key.Delete:
                    if (Item.Stash) this.SaveFileHandler.Shop.Stash.Remove(Item); else this.SaveFileHandler.SelectedFile.Inventory.RemoveItem(Item);
                    UpdateForgeLists();
                    break;
                case Key.Down:
                    if (lb.SelectedIndex == lb.Items.Count - 1) { e.Handled = true; return; }
                    lb.SelectedIndex += 1;
                    lb.ScrollIntoView(lb.SelectedItem);
                    e.Handled = true;
                    break;
                case Key.Up:
                    if (lb.SelectedIndex == 0) { e.Handled = true; return; }
                    lb.SelectedIndex -= 1;
                    lb.ScrollIntoView(lb.SelectedItem);
                    e.Handled = true;
                    break;
                default:
                    e.Handled = true;
                    return;
            }
        }

        private void ItemsControl_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        private void listBox_EquippeditemsForge_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ContextMenu_Remove_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuEquipmentForge.IsOpen = false;
            ((EquipmentSlot)listBox_EquippeditemsForge.SelectedItem).SetItem(null);
            this.UpdateEquippedItems();
            this.UpdateForgeLists();
        }

        private void ContextMenu_Clear_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuEquipmentForge.IsOpen = false;
            foreach (EquipmentSlot slot in listBox_EquippeditemsForge.Items)
                slot.SetItem(null);
            this.UpdateEquippedItems();
            this.UpdateForgeLists();
        }

        private void ContextMenu_ClearAll_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuEquipmentForge.IsOpen = false;
            ContextMenuItemForge.IsOpen = false;
            foreach (EquipmentSlot slot in listBox_EquippeditemsForge.Items)
                slot.SetItem(null);

            this.SaveFileHandler.SelectedFile.Inventory.InventoryItems.Clear();
            this.SaveFileHandler.Shop.Stash.Clear();

            this.UpdateEquippedItems();
            this.UpdateInventory();
            this.UpdateForgeLists();
        }

        private void ContextMenu_Remove_Click_1(object sender, RoutedEventArgs e)
        {
            ContextMenuItemForge.IsOpen = false;
            Item item = (Item)listBox_InventoryForge.SelectedItem;
            if (item.Stash) this.SaveFileHandler.Shop.Stash.Remove(item);
            else this.SaveFileHandler.SelectedFile.Inventory.RemoveItem(item);
            this.UpdateInventory();
            this.UpdateForgeLists();
        }

        private void ContextMenu_Clear_Click_1(object sender, RoutedEventArgs e)
        {
            ContextMenuItemForge.IsOpen = false;
            this.SaveFileHandler.SelectedFile.Inventory.InventoryItems.Clear();
            this.SaveFileHandler.Shop.Stash.Clear();
            this.UpdateInventory();
            this.UpdateForgeLists();
        }

        private void ContextMenu_PinItem_Click_3(object sender, RoutedEventArgs e)
        {
            ContextMenuEquipmentForge.IsOpen = false;
            ItemDisplay itemDisplay = new ItemDisplay(((EquipmentSlot)listBox_EquippeditemsForge.SelectedItem).Item);
            itemDisplay.Show();
        }

        private void ContextMenu_PinItem_Click_4(object sender, RoutedEventArgs e)
        {
            ContextMenuItemForge.IsOpen = false;
            ItemDisplay itemDisplay = new ItemDisplay((Item)listBox_InventoryForge.SelectedItem);
            itemDisplay.Show();
        }

        private void buttonSaveBuild_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.InitialDirectory = Environment.GetEnvironmentVariable("LocalAppData") + $@"\Hero_Siege\hseditor\builds";
            ofd.Filter = "Build-File | *.build";

            if (ofd.ShowDialog() == true)
            {
                this.BuildHandler.SaveBuild(ofd.FileName);
                this.BuildHandler.GetOfflineBuilds();
                this.UpdateBuildsList();
            }
        }

        private void Grid_MouseDown_3(object sender, MouseButtonEventArgs e)
        {
            gridMercenaryBuilds.Visibility = gridMercenaryBuilds.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            collapseIndicatorMercenary.Content = gridMercenaryBuilds.Visibility == Visibility.Visible ? "▲" : "▼";
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TooltipHandler.SetToolTip((TextBlock)sender);
        }

        private void ContextMenu_DuplicateItem_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuItem.IsOpen = false;
            Item item = (Item)listBox_Inventory.SelectedItem;
            Item duplicate = item.DeepCopy();
            if (item.Stash)
                this.SaveFileHandler.Shop.AddItemToStash(duplicate);
            else
                this.SaveFileHandler.SelectedFile.Inventory.InventoryItems.Insert(this.SaveFileHandler.SelectedFile.Inventory.InventoryItems.IndexOf(item) + 1, duplicate);

            this.listBox_Inventory.Items.Refresh();
            this.UpdateForgeLists();
            listBox_Inventory.ScrollIntoView(duplicate);

        }

        private void ContextMenu_Duplicate_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuItemForge.IsOpen = false;
            Item item = (Item)listBox_InventoryForge.SelectedItem;
            Item duplicate = item.DeepCopy();
            if (item.Stash)
                this.SaveFileHandler.Shop.AddItemToStash(duplicate);
            else
                this.SaveFileHandler.SelectedFile.Inventory.InventoryItems.Insert(this.SaveFileHandler.SelectedFile.Inventory.InventoryItems.IndexOf(item) + 1, duplicate);

            this.listBox_Inventory.Items.Refresh();
            this.UpdateForgeLists();
            this.listBox_InventoryForge.ScrollIntoView(duplicate);
        }

        private void listBox_EuippeditemsForge_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.listBox_EquippeditemsForge.SelectedItem == null) return;
            EquipmentSlot slot = (EquipmentSlot)listBox_EquippeditemsForge.SelectedItem;
            ListBox lb = (ListBox)sender;
            switch (e.Key)
            {
                case Key.Delete:
                    slot.SetItem(null);
                    UpdateEquippedItems();
                    UpdateForgeLists();
                    break;
                case Key.Down:
                    if (lb.SelectedIndex == lb.Items.Count - 1) { e.Handled = true; return; }
                    lb.SelectedIndex += 1;
                    lb.ScrollIntoView(lb.SelectedItem);
                    e.Handled = true;
                    break;
                case Key.Up:
                    if (lb.SelectedIndex == 0) { e.Handled = true; return; }
                    lb.SelectedIndex -= 1;
                    lb.ScrollIntoView(lb.SelectedItem);
                    e.Handled = true;
                    break;
                default:
                    e.Handled = true;
                    return;
            }
        }

        private void buttonRefreshBuilds_Click(object sender, RoutedEventArgs e)
        {
            if (controlSelectedBuilds.SelectedIndex == 0)
            {
                this.BuildHandler.GetBuilds();
                this.listBox_Builds.Items.Refresh();
                if (!this.BuildHandler.failedConnection) return;
                MessageBox mb = new MessageBox("Connection failed!", "Couldn't establish connection to the database. Please try again later.", "OK");
                mb.ShowDialog();
            }
            else
            {
                this.BuildHandler.GetOfflineBuilds();
                listBox_Builds.Items.Refresh();
            }
        }

        private void mainBorder_Loaded(object sender, RoutedEventArgs e)
        {
            List<Stat> flat = new List<Stat>();
            List<Stat> percent = new List<Stat>();

            List<Item> list = controlSelectedEquipment.SelectedIndex == 0 ? this.SaveFileHandler.SelectedFile.Inventory.Equipment.GetItems() : this.SaveFileHandler.SelectedFile.Mercenaries.GetSelectedMerc().Equipment.GetItems();
            List<Item> valid = new List<Item>();
            List<Item> upgradecosts = new List<Item>();
            foreach (Item Item in list)
            {
                Item item = Item.DeepCopy();
                if (item.Stats != null)
                {
                    valid.Add(item);
                    if (item.Stats.UpgradePrice != 0 && item.UpgradeLevel > 1)
                    {
                        double upgrade = item.Stats.UpgradePrice;
                        double cost = 0;
                        for (int i = 1; i <= item.UpgradeLevel - 1; i++)
                            cost += upgrade * i;
                        item.Stats.UpgradePrice = Convert.ToDouble(cost);
                        item.Stats.UpgradePriceString = cost >= 10000 ? (cost / 1000.0) > 999 ? (cost / 1000000.0).ToString() + "M" : (cost / 1000.0).ToString() + "K" : cost.ToString();
                        upgradecosts.Add(item);
                    }

                }
                if (item.Stats == null) continue;
                foreach (Stat stat in item.Stats.StatList)
                {
                    if (stat.DebugName.StartsWith("INV_TALENT_")) continue;
                    bool x = false;
                    if (stat.Type == "%")
                    {
                        foreach (Stat stat3 in percent)
                        {
                            if (stat3.Name == stat.Name)
                            {
                                x = true;
                                stat3.ChangeValue(stat3.Value + stat.Value);
                            }
                        }
                        if (!x)
                            percent.Add(stat);
                    }
                    else
                    {
                        foreach (Stat stat3 in flat)
                        {
                            if (stat3.Name == stat.Name)
                            {
                                x = true;
                                stat3.ChangeValue(stat3.Value + stat.Value);
                            }
                        }
                        if (!x)
                            flat.Add(stat);

                    }
                }
            }

            foreach (Stat stat in flat)
            {
                foreach (Stat stat2 in percent)
                {
                    if (stat2.Name == stat.Name)
                    {
                        stat.ChangeValue(Math.Round(stat.Value += stat.Value * (Convert.ToDouble(stat2.Value) / 100.0)));
                        break;
                    }

                }
                stat.ValueFormatted = stat.Value >= 10000 ? (stat.Value / 1000) > 999 ? Math.Round(stat.Value / 1000000.0, 2).ToString() + "M" : Math.Round(stat.Value / 1000.0, 2).ToString() + "K" : stat.Value.ToString();
                stat.ValueFormatted = stat.ValueFormatted.Replace(',', '.');
            }
            ((Border)sender).Visibility = valid.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            ((ItemsControl)((StackPanel)((Grid)((Border)sender).Child).Children[0]).Children[1]).ItemsSource = flat.OrderBy(o => o.Priority);
            ((Grid)((StackPanel)((Grid)((Border)sender).Child).Children[0]).Children[2]).Visibility = upgradecosts.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            ((ItemsControl)((StackPanel)((Grid)((Border)sender).Child).Children[0]).Children[3]).Visibility = upgradecosts.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            ((ItemsControl)((StackPanel)((Grid)((Border)sender).Child).Children[0]).Children[3]).ItemsSource = upgradecosts;
            double allupgrades = 0;
            upgradecosts.ForEach(o => { allupgrades += o.Stats.UpgradePrice; });
            ((TextBlock)((Grid)((StackPanel)((Grid)((Border)sender).Child).Children[0]).Children[2]).Children[1]).Text = allupgrades >= 10000 ? (allupgrades / 1000.0) > 999 ? Math.Round(allupgrades / 1000000.0, 2).ToString().Replace(',', '.') + "M" : Math.Round(allupgrades / 1000.0, 2).ToString().Replace(',', '.') + "K" : allupgrades.ToString();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Uri uriResult;
            bool result = Uri.TryCreate("https://discord.gg/DPeuk8Q", UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            Process.Start(new ProcessStartInfo(uriResult.AbsoluteUri) { UseShellExecute = true });
        }

        private void TabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button btn = (Button)sender;
            int index = Convert.ToInt32(btn.Tag);

            if (index == 0)
            {
                if (this.SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipmentList().Count == 0) return;
                this.listBox_EquippeditemsForge.ScrollIntoView(this.SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipmentList()[0]);
            }
            else
            {
                if (this.SaveFileHandler.SelectedFile.Mercenaries.GetSelectedMerc().Equipment.GetEquipmentList().Count == 0) return;
                this.listBox_EquippeditemsForge.ScrollIntoView(this.SaveFileHandler.SelectedFile.Mercenaries.GetSelectedMerc().Equipment.GetEquipmentList()[0]);
            }
        }

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button btn = (Button)sender;
            int index = Convert.ToInt32(btn.Tag);

            if (index == 0)
            {
                if (this.SaveFileHandler.SelectedFile.Inventory.InventoryItems.Count == 0) return;
                this.listBox_InventoryForge.ScrollIntoView(this.SaveFileHandler.SelectedFile.Inventory.InventoryItems[0]);
            }
            else
            {
                if (this.SaveFileHandler.Shop.Stash.Count == 0) return;
                this.listBox_InventoryForge.ScrollIntoView(this.SaveFileHandler.Shop.Stash[0]);
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {

        }
    }
}
