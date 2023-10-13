using HSeditor.Classes;
using HSeditor.Classes.Filter.Item;
using HSeditor.Classes.iniDB;
using HSeditor.Classes.Items;
using HSeditor.Classes.Other;
using HSeditor.Classes.Util;
using HSeditor.SaveFiles;
using HSeditor.Windows;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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
        public UpdateHandler UpdateHandler { get; set; }
        public ConfigHandler ConfigHandler { get; private set; }
        public RuneHandler RuneHandler { get; private set; }
        public ClassHandler ClassHandler { get; private set; }
        public RarityHandler RarityHandler { get; private set; }
        public SaveFileHandler SaveFileHandler { get; private set; }
        public ItemHandler ItemHandler { get; private set; }
        public SetHandler SetHandler { get; private set; }
        public WeaponTypeHandler WeaponTypeHandler { get; private set; }
        public SlotHandler SlotHandler { get; private set; }
        public EquipmentSlotHandler EquipmentSlotHandler { get; private set; }
        public InventoryBoxHandler InventoryBoxHandler { get; private set; }
        public DescriptionHandler DescriptionHandler { get; private set; }



        public enum ListType { Equipment, Inventory, Mercenary, Stash }

        public EquipmentView EquipmentView { get; private set; }

        public Stash Stash { get; set; }
        public bool activeDrag = false;
        public bool stashOpenend = false;
        public ItemDrag previewDrag = null;


        public ItemHandler.InvType SelectedInv { get { return (ItemHandler.InvType)controlSelectedInventory.SelectedIndex; } }
        public ItemHandler.InvType? SelectedStash { get { return Stash == null ? null : (ItemHandler.InvType)(Stash.controlSelectedStash.SelectedIndex + this.ItemHandler.StashIndex); } }

        public double SizeFactor { get { return Util.GetScaleFactor(vbMain); } }

        public MainWindow()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(Exception);

            INSTANCE = this;

            var SplashScreen = new SplashScreen("Resources/SplashScreen.png");
            SplashScreen.Show(false);
            this.CheckDirectory();
            this.iniDB = new iniDB(Environment.CurrentDirectory + @"\ini.dll");
            this.UpdateHandler = new UpdateHandler();
            InitializeComponent();

            // Misc
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));
            ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(300));
            IsTabStopProperty.OverrideMetadata(typeof(Control), new FrameworkPropertyMetadata(false));

            // Classes
            this.RuneHandler = new RuneHandler();
            this.WeaponTypeHandler = new WeaponTypeHandler();
            this.SlotHandler = new SlotHandler();
            this.EquipmentSlotHandler = new EquipmentSlotHandler();
            this.ClassHandler = new ClassHandler();
            this.SetHandler = new SetHandler();
            this.RarityHandler = new RarityHandler();
            this.ConfigHandler = new ConfigHandler();
            this.RefreshSettings();
            this.ItemHandler = new ItemHandler();
            this.DescriptionHandler = new DescriptionHandler();
            this.InventoryBoxHandler = new InventoryBoxHandler();
            this.SaveFileHandler = new SaveFileHandler();
            this.popupGrid.Visibility = Visibility.Visible;
            SplashScreen.Close(TimeSpan.FromSeconds(1));
        }


        static void Exception(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            ThrowError(e);
        }

        //[Conditional("RELEASE")]
        static void ThrowError(Exception e)
        {
            ErrorBox error = new ErrorBox(e);
            error.ShowDialog();
        }

        public void CheckDirectory()
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege");

            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\save_folder"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\save_folder");

            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hs2saves"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hs2saves");

            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hseditor"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hseditor");

            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hseditor\builds"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hseditor\builds");

            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hseditor\saves"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hero_Siege\hseditor\saves");

        }

        private void listBox_Slots_Loaded(object sender, RoutedEventArgs e)
        {
            List<SaveFile> saves = this.SaveFileHandler.SaveFiles.FindAll(o => !o.NewChar);
            saves.ForEach(o => o.Color = saves.IndexOf(o) % 2 != 0 ? "#bababa" : "#F6F6F6");
            listBox_Slots.ItemsSource = saves;
            listBox_Slots.SelectedIndex = 0;
        }

        public void RefreshListboxes()
        {
            if (this.SaveFileHandler == null) return;
            List<SaveFile> saves = this.SaveFileHandler.SaveFiles.FindAll(o => !o.NewChar);
            saves.ForEach(o => o.Color = saves.IndexOf(o) % 2 != 0 ? "#bababa" : "#F6F6F6");
            listBox_Slots.ItemsSource = saves;
            this.listBox_Slots.Items.Refresh();
            UpdateEquippedItems();
            UpdateInventory();
            UpdateStash();
        }


        public void ShowPopup(UIElement element)
        {
            controlGrid.Children.Clear();
            controlGrid.Children.Add(element);
            popupGrid.Opacity = 0.5;
            popupGrid.IsHitTestVisible = true;
        }

        public void ClosePopup()
        {
            controlGrid.Children.Clear();
            popupGrid.Opacity = 0;
            popupGrid.IsHitTestVisible = false;
        }

        public void UpdateEquippedItems()
        {
            this.SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipment().ForEach(o =>
            {
                if (o.temp != null) o.ResetTempItem();
                o.SetItem(o.Item);
            });
            this.EquipmentView = new EquipmentView(this.SaveFileHandler.SelectedFile.Inventory.Equipment);
            gridEquipment.Children.Clear();
            gridEquipment.Children.Add(this.EquipmentView);
        }

        public void BuildInventory()
        {
            if (this.gridInvMain == null) return;
            this.InventoryBoxHandler.ClearSelectedItems();
            this.gridInvMain.Children.Clear();
            this.gridInvMainImages.Children.Clear();
            this.InventoryBoxHandler.InventoryBoxes.Clear();
            int height = (int)this.InventoryBoxHandler.InventorySize.Y;
            int width = (int)this.InventoryBoxHandler.InventorySize.X;
            int charmWidth = this.InventoryBoxHandler.CharmWidth;
            for (int i = 0; i < height; i++)
            {
                bool inv1 = (ItemHandler.InvType)controlSelectedInventory.SelectedIndex == ItemHandler.InvType.Main;
                for (int x = 0; x < width; x++)
                {
                    bool charm = x >= (width - charmWidth) && inv1;
                    Border grid = this.InventoryBoxHandler.GetBoxBorder(charm);

                    grid.DragEnter += invDragOver;
                    grid.Drop += invDragDrop;
                    grid.DragLeave += invDragLeave;
                    gridInvMain.Children.Add(grid);
                    InventoryBox inventoryBox = new InventoryBox(new Point(x, i), null, grid, x <= 8 ? InventoryBoxHandler.BoxType.Item : InventoryBoxHandler.BoxType.Charm);
                    grid.DataContext = inventoryBox;
                    InventoryBoxHandler.InventoryBoxes.Add(inventoryBox);
                }
            }
            this.UpdateInventory();
        }


        public void invSelect(object sender, EventArgs e)
        {
            Border border = (Border)sender;
            InventoryBox box = border.DataContext as InventoryBox;

            ItemDrag drag = new ItemDrag(null, null, DragHandler.Origin.InvSelect);
            drag.CurrentFields = new List<InventoryBox> { box };

            DataObject dataObject = new DataObject(typeof(ItemDrag), drag);
            DragDrop.DoDragDrop((Border)sender, dataObject, DragDropEffects.Move);
        }

        // Inv Images
        public void UpdateInventory()
        {
            if (this.SaveFileHandler == null || this.SaveFileHandler.SelectedFile == null) return;

            InventoryBoxHandler.ClearSelectedItems();
            gridInvMainImages.Children.Clear();
            InventoryBoxHandler.InventoryBoxes.ForEach(o => o.Item = null);


            foreach (Item item in this.SaveFileHandler.SelectedFile.Inventory.InventoryItems)
            {
                if (item.Inv != this.SelectedInv || (activeDrag && previewDrag != null && item == previewDrag.Item)) continue;
                Point end = new Point(item.InvPos.Value.X + (item.Size.X - 1), item.InvPos.Value.Y + (item.Size.Y - 1));
                List<InventoryBox> boxes = this.InventoryBoxHandler.GetInventoryBoxesInMatrix(item.InvPos.Value, end, InventoryBoxHandler.InventoryBoxes);
                if (boxes.Count == 0) continue;
                if (boxes.Find(o => o.Item != null) != null) continue;

                Border border = this.InventoryBoxHandler.GetImageOverlay(item, boxes);
                Image img = (Image)border.Child;
                border.Child = null;

                boxes.ForEach(o =>
                {
                    o.Item = item;
                    o.InvImage = border;
                });

                border.PreviewMouseLeftButtonDown += invStartDrag;
                border.GiveFeedback += Grid_GiveFeedback;
                border.ContextMenu = new ContextMenuInv(item, DragHandler.Origin.Inventory, null);
                border.MouseEnter += ShowTooltip;
                border.MouseLeave += HideTooltip;

                Grid grid = new Grid { };
                grid.Children.Add(img);
                if (item.Slot.ShowAmount && (int)item.SaveItem["amount"] > 1) grid.Children.Add(new Label { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Foreground = Util.ColorFromString("#FFFFFF"), FontSize = 10, FontWeight = FontWeights.Bold, Content = item.SaveItem["amount"], Margin = new Thickness(0, 0, 0, -3) });

                border.Child = grid;
                item.InvImage = border;
                item.Fields = boxes;

                gridInvMainImages.Children.Add(border);
            }
            labelInventoryCount.Content = this.SaveFileHandler.SelectedFile.Inventory.InventoryItems.Count;
        }

        public void UpdateStash()
        {
            if (!stashOpenend) return;
            if (this.Stash == null)
            {
                this.Stash = new Stash();
                Stash.Show();
                Stash.Update();
            }
            else
            {
                this.Stash.Update();
            }
        }

        private Thickness GetTooltipPos(Border border, UserControl toolTip)
        {
            toolTip.UpdateLayout();
            Point pos = border.TranslatePoint(new Point(0, 0), toolTipGrid);
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
            Image image = (Image)((Grid)border.Child as Grid).Children[0];

            Item item = border.DataContext as Item;
            if (item == null || image.Tag != null) return;

            UserControl toolTip;
            toolTip = new ItemTooltip(item);
            toolTipGrid.Children.Add(toolTip);
            toolTip.VerticalAlignment = VerticalAlignment.Top;
            toolTip.HorizontalAlignment = HorizontalAlignment.Left;
            toolTip.Margin = this.GetTooltipPos(border, toolTip);
            image.Tag = toolTip;
            toolTip.IsHitTestVisible = false;
        }

        private void HideTooltip(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            Image image = (Image)((Grid)border.Child as Grid).Children[0];
            toolTipGrid.Children.Remove((UserControl)image.Tag);
            image.Tag = null;
        }

        private Thickness ItemList_GetTooltipPos(Border border, ItemTooltip toolTip)
        {
            toolTip.UpdateLayout();
            Point pos = border.TranslatePoint(new Point(0, 0), toolTipGrid);
            double x = pos.X - toolTip.ActualWidth - 5;
            double y = pos.Y + (border.ActualHeight / 2) - (toolTip.ActualHeight / 2);

            if (y < 0) y = 0;
            else if ((y + toolTip.ActualHeight) > toolTipGrid.ActualHeight) y = toolTipGrid.ActualHeight - toolTip.ActualHeight;

            return new Thickness(x, y, 0, 0);
        }

        private void ItemList_ShowTooltip(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;

            Item item = border.DataContext as Item;
            if (item == null) return;

            ItemTooltip toolTip = new ItemTooltip(item);
            toolTipGrid.Children.Add(toolTip);
            toolTip.VerticalAlignment = VerticalAlignment.Top;
            toolTip.HorizontalAlignment = HorizontalAlignment.Left;
            toolTip.Margin = this.ItemList_GetTooltipPos(border, toolTip);
            border.Tag = toolTip;
            toolTip.IsHitTestVisible = false;
        }

        private void ItemList_HideTooltip(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            toolTipGrid.Children.Remove((ItemTooltip)border.Tag);
        }



        public void invStartDrag(object sender, MouseButtonEventArgs e)
        {
            Border border = (Border)sender;
            Item item = (Item)border.DataContext;
            if (item == null) return;

            if (e.ClickCount == 2)
            {
                if (item.Slot.Name == "Charm")
                {
                    int indexSelectedInv = controlSelectedInventory.SelectedIndex;
                    controlSelectedInventory.SelectedIndex = 2;
                    InventoryBox box = this.InventoryBoxHandler.FindCharmSpace(item);
                    if (box == null) { controlSelectedInventory.SelectedIndex = indexSelectedInv; return; }

                    item.Inv = ItemHandler.InvType.Main;
                    item.InvPos = box.Position;
                    this.UpdateInventory();
                }
                else
                {
                    EquipmentSlot slot = SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipmentSlotFromItem(item);
                    if (slot == null) return;

                    Item old = slot.Item;
                    SaveFileHandler.SelectedFile.Inventory.Equipment.EquipItem(item.DeepCopy(), slot.ID);
                    this.SaveFileHandler.SelectedFile.Inventory.RemoveItem(item);
                    if (old != null)
                    {
                        old.Inv = SelectedInv;
                        InventoryBoxHandler.InventoryBoxes.FindAll(o => o.Item == item).ForEach(o => o.Item = null);
                        InventoryBox box = InventoryBoxHandler.FindSpace(old, InventoryBoxHandler.InventoryBoxes);
                        if (box != null)
                        {
                            old.InvPos = box.Position;
                            SaveFileHandler.SelectedFile.Inventory.InventoryItems.Add(old);
                        }
                    }

                    this.UpdateEquippedItems();
                    this.UpdateInventory();
                    return;
                }
            }

            Point end = new Point(item.InvPos.Value.X + (item.Size.X - 1), item.InvPos.Value.Y + (item.Size.Y - 1));
            List<InventoryBox> boxes = InventoryBoxHandler.GetInventoryBoxesInMatrix(item.InvPos.Value, end, InventoryBoxHandler.InventoryBoxes);

            ItemDrag drag = new ItemDrag(null, item, DragHandler.Origin.Inventory);
            drag.CurrentFields = boxes;
            drag.sender = sender as Border;
            drag.oriPos = e.GetPosition(this);
            previewDrag = drag;
        }

        private void listBox_Slots_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SwitchSaveSlot();

            List<Grid> grids = new List<Grid> { mainGrid1, mainGrid3 };
            grids.ForEach(o => o.IsEnabled = listBox_Slots.SelectedItem != null);

            this.ContextMenuSlots.DataContext = this.listBox_Slots.SelectedItem;
        }

        private void SwitchSaveSlot()
        {
            if ((SaveFile)listBox_Slots.SelectedItem == null) return;
            this.SaveFileHandler.ReadSaveFile((SaveFile)listBox_Slots.SelectedItem);
            this.Title = new string($"HSeditor - Name: {this.SaveFileHandler.SelectedFile.HeroInfo.Name} | Class: {this.SaveFileHandler.SelectedFile.HeroInfo.Class.Name} | ID: {this.SaveFileHandler.SelectedFile.ID} | Version: {this.UpdateHandler.Version}");
            this.SaveFileHandler.Shop.Refresh();
            this.UpdateHeroInfo();
            RefreshListboxes();
        }

        private void comboBoxClasses_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxClasses.ItemsSource = this.ClassHandler.Classes_Filtered;
        }

        private void comboBoxClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((Class)comboBoxClasses.SelectedItem).ID == this.SaveFileHandler.SelectedFile.HeroInfo.Class.ID) return;
            this.SaveFileHandler.SelectedFile.HeroInfo.Class = this.ClassHandler.GetClassFromID(((Class)comboBoxClasses.SelectedItem).ID).DeepCopy();
            this.RefreshListboxes();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateHandler.CheckForUpdate();
            comboBoxItemFilterSlot.ItemsSource = this.SlotHandler.SlotsFiltered;
            comboBoxItemFilterWeaponType.ItemsSource = this.WeaponTypeHandler.WeaponTypesFiltered;
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
            List<Item> items = this.ItemHandler.GetFilteredList().OrderBy(o => o.Rarity.Name == "Runeword").ThenBy(o => o.Slot.ID).ThenBy(o => o.ID).ToList();
            this.listBox_Items.ItemsSource = items;
            this.listBox_Items.Items.Refresh();
            /*if (Util.ContainsFavorite(items))
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(listBox_Items.ItemsSource);
                view.GroupDescriptions.Add(new PropertyGroupDescription("Favorite"));
                view.SortDescriptions.Add(new SortDescription("Favorite", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("Slot.ID", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("WeaponType.ID", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("Runeword", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("ID", ListSortDirection.Ascending));
            }*/
            this.labelItemCount.Content = $"{this.listBox_Items.Items.Count}";
            labelItemCount.Foreground = listBox_Items.Items.Count == 0 ? Util.ColorFromString("#a32424") : Util.ColorFromString("#FF99AAB5");
            textBoxItemlistSearch.Foreground = listBox_Items.Items.Count == 0 ? Util.ColorFromString("#a32424") : Util.ColorFromString("#FFF6F6F6");
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
            int slotid = this.SaveFileHandler.SelectedFile == null ? -1 : this.SaveFileHandler.SelectedFile.ID;
            this.SaveFileHandler.Shop.Refresh();
            this.SaveFileHandler.GetSaveFiles();
            listBox_Slots.ItemsSource = this.SaveFileHandler.SaveFiles.FindAll(o => !o.NewChar);
            this.listBox_Slots.SelectedItem = slotid == -1 ? null : this.SaveFileHandler.SaveFiles.Find(o => o.ID == slotid);
            this.SwitchSaveSlot();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Refresh();
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

        public void UpdateHeroInfo()
        {
            this.gridSelectedSlot.DataContext = this.SaveFileHandler.SelectedFile;
            this.comboBoxClasses.SelectedItem = this.ClassHandler.GetClassFromName(this.SaveFileHandler.SelectedFile.HeroInfo.Class.Name);
            this.gridHeroInfo.DataContext = null;
            this.gridHeroInfo.DataContext = this.SaveFileHandler.SelectedFile.HeroInfo;
            this.textBoxHeroName.Text = this.SaveFileHandler.SelectedFile.HeroInfo.Name;
            this.textBoxGold.Text = this.SaveFileHandler.SelectedFile.HeroInfo.Hardcore ? this.SaveFileHandler.Shop.Gold_HC.ToString() : this.SaveFileHandler.Shop.Gold.ToString();
            this.textBoxRubies.Text = this.SaveFileHandler.Shop.Rubies.ToString();
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


        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.SaveFileHandler.SelectedFile.Save();
            this.Refresh();
            MessageBox mb = new MessageBox("Saving successful!", "Changes were successfully applied.", "OK");
            mb.ShowDialog();
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

            listBoxItemFilterRarity.Items.Refresh();
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

        private void MenuControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }


        public void RefreshSettings()
        {

        }



        private void ButtonApplySettings(object sender, RoutedEventArgs e)
        {

            this.Refresh();
            MessageBox mb = new MessageBox("Config saved!", "Your config was successfully saved.", "OK");
            mb.ShowDialog();
        }

        private void ButtonReloadSettings(object sender, RoutedEventArgs e)
        {
            this.ConfigHandler.LoadConfig();
            this.RefreshSettings();
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

        private void ContextMenu_Delete_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuSlots.IsOpen = false;
            try
            {
                SaveFile sf = (SaveFile)ContextMenuSlots.DataContext;
                sf.Delete();
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
                this.SaveFileHandler.Copy = (SaveFile)ContextMenuSlots.DataContext;
            }
            catch { }
        }

        private void ContextMenu_Paste_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuSlots.IsOpen = false;
            try
            {
                File.Copy(this.SaveFileHandler.Copy.Path, ((SaveFile)ContextMenuSlots.DataContext).Path);
                this.Refresh();
            }
            catch { }
        }


        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void listBox_Slots_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (listBox_Slots.SelectedItem == null) return;
            SaveFile sf = listBox_Slots.SelectedItem as SaveFile;
            if (sf.NewChar && this.SaveFileHandler.Copy == null)
                e.Handled = true;
        }

        private void controlSelectedInventory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            this.UpdateInventory();
            if (buttonAddToInventory == null) return;
            this.buttonAddToInventory.Content = controlSelectedInventory.SelectedIndex == 0 ? "Add to Inventory" : "Add to Stash";*/
            this.BuildInventory();
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TooltipHandler.SetToolTip((TextBlock)sender);
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Uri uriResult;
            bool result = Uri.TryCreate("https://discord.gg/DPeuk8Q", UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            Process.Start(new ProcessStartInfo(uriResult.AbsoluteUri) { UseShellExecute = true });
        }


        private void Window_Initialized(object sender, EventArgs e)
        {

        }

        // AddItem / Add Item
        private void listBox_Items_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Item item = (Item)((Border)sender).DataContext;
            if (item == null) return;

            if (e.ClickCount >= 2)
            {
                EquipmentSlot slot = SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipmentSlotFromItem(item);
                if (slot == null || !Keyboard.IsKeyDown(Key.LeftShift))
                {
                    // Add to inventory/stash
                    if (this.Stash == null || this.Stash.WindowState == WindowState.Minimized) this.SaveFileHandler.SelectedFile.Inventory.AddItem(item, true);
                    else this.SaveFileHandler.Shop.AddItemToStash(item, true);
                    this.UpdateInventory();
                    this.UpdateStash();
                }
                else
                {
                    // Equip
                    this.SaveFileHandler.SelectedFile.Inventory.Equipment.EquipItem(item);
                    this.UpdateEquippedItems();
                }
                return;
            }

            ItemDrag drag = new ItemDrag(null, item, DragHandler.Origin.ItemList);
            drag.oriPos = e.GetPosition(this);
            drag.sender = sender as Border;
            previewDrag = drag;
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        public static void SetCursor(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public Cursor customCursor;

        public void Grid_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                if (previewDrag == null) return;
                InventoryBox box = this.InventoryBoxHandler.FindSpace(previewDrag.Item);
                if (box != null)
                {
                    Point pos = box.Grid.PointToScreen(new Point(0, 0));
                    SetCursor((int)pos.X + 20, (int)pos.Y + 20);
                }
            }

            double sizeFactor = previewDrag.Type == DragHandler.Origin.Stash ? this.Stash.SizeFactor : this.SizeFactor;

            /*if (e.Effects == DragDropEffects.Scroll)
            {
                customCursor = null;
                e.UseDefaultCursors = false;
                Mouse.SetCursor(Cursors.Arrow);
                e.Handled = true;
                return;
            }*/

            if (customCursor == null)
            {
                object datacontext = (sender as FrameworkElement).DataContext;
                Item item = datacontext is EquipmentSlot ? ((EquipmentSlot)datacontext).temp : datacontext is Item ? (Item)datacontext : null;
                if (item == null) return;


                BitmapImage bmi = new BitmapImage(new Uri(item.Sprite));

                double width = bmi.Width > item.Size.X * this.InventoryBoxHandler.BoxSize.X ? item.Size.X * this.InventoryBoxHandler.BoxSize.X : bmi.Width;
                double height = bmi.Height > item.Size.Y * this.InventoryBoxHandler.BoxSize.Y ? item.Size.Y * this.InventoryBoxHandler.BoxSize.Y : bmi.Height;

                Border b = new Border { Width = width * sizeFactor, Height = height * sizeFactor, Background = null, BorderThickness = new Thickness(0), BorderBrush = Util.ColorFromString(item.Rarity.Color), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                b.Child = new Image { Source = bmi, Stretch = Stretch.Uniform };

                customCursor = CursorHelper.CreateCursor(b as UIElement);
            }

            if (customCursor != null)
            {
                e.UseDefaultCursors = false;
                Mouse.SetCursor(customCursor);
            }

            e.Handled = true;
        }


        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            List<SaveFile> saveFiles = (List<SaveFile>)this.listBox_Slots.ItemsSource;
            SaveFile sf = this.SaveFileHandler.SaveFiles.Find(o => o.NewChar);
            sf.HeroInfo = this.SaveFileHandler.GetHeroInfo("X");
            sf.SetProperties();
            this.SaveFileHandler.Save(sf);
            this.Refresh();
            listBox_Slots.SelectedItem = ((List<SaveFile>)listBox_Slots.ItemsSource).Find(o => o.ID == sf.ID);
            listBox_Slots.ScrollIntoView(listBox_Slots.SelectedItem);
        }

        private void listBox_Slots_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenuSlots.DataContext = ((Border)e.OriginalSource).DataContext;
            e.Handled = true;
        }

        private void gridInvMain_Loaded(object sender, RoutedEventArgs e)
        {
            if (gridInvMain.Children.Count != 0) return;
            this.BuildInventory();
        }

        public bool PlaceInv(Point start, Item item)
        {
            bool newItem = !SaveFileHandler.SelectedFile.Inventory.InventoryItems.Contains(item);
            item = newItem ? item.DeepCopy() : item;
            List<InventoryBox> newboxes = InventoryBoxHandler.GetInventoryBoxesInMatrix(start, item, InventoryBoxHandler.InventoryBoxes);
            if (newboxes.Count == 0 || newboxes.Count != (item.Size.X * item.Size.Y)) return false;
            InventoryBox occupied = newboxes.Find(o => o.Item != null && o.Item != item);
            //InventoryBoxHandler.BoxType type = newboxes[0].Type;
            if (occupied != null || !InventoryBoxHandler.isValidItem(item))
            {
                this.UpdateInventory();
                return false;
            }
            // if(newboxes.Find(o => o.Type != type) != null) return;
            item.InvPos = start;
            bool sameInv = item.Inv == this.SelectedInv;
            item.Inv = this.SelectedInv;


            if (newItem)
                this.SaveFileHandler.SelectedFile.Inventory.AddItem(item, false);

            return true;
        }

        public void invDragDrop(object sender, DragEventArgs e)
        {
            ItemDrag drag = e.Data.GetData(typeof(ItemDrag)) as ItemDrag;
            if (drag == null || drag.Item == null) return;

            Border border = (Border)sender;
            InventoryBox box = (InventoryBox)border.DataContext;

            Item item = drag.Item;
            bool success = false;
            if (drag.isValid)
            {
                success = this.PlaceInv(box.Position, item);
                if (success)
                {
                    switch (drag.Type)
                    {
                        case DragHandler.Origin.Equip:
                            drag.EquipmentSlot.SetItem(null);
                            drag.EquipmentSlot.temp = null;
                            this.UpdateEquippedItems();
                            break;
                        case DragHandler.Origin.Stash:
                            this.SaveFileHandler.Shop.Stash.Remove(item);
                            this.UpdateStash();
                            break;
                    }
                }



                if (item.InvImage != null)
                    item.InvImage.Visibility = Visibility.Visible;
            }
            //drag.CurrentFields.ForEach(o => o.Item = null);
            drag.HoverFields.ForEach(o => o.Grid.Background = o.DefaultBackground);
            ((EquipmentView)gridEquipment.Children[0]).SetAllowDrop(null);

            this.activeDrag = false;
            this.previewDrag = null;
            this.UpdateInventory();
        }

        public void invDragOver(object sender, DragEventArgs e)
        {
            ItemDrag drag = e.Data.GetData(typeof(ItemDrag)) as ItemDrag;
            if (drag.Item == null) return;

            //e.Effects = DragDropEffects.None;
            Border border = (Border)sender;
            Point start = ((InventoryBox)border.DataContext).Position;
            Point end = new Point(start.X + (drag.Item.Size.X - 1), start.Y + (drag.Item.Size.Y - 1));
            List<InventoryBox> boxes = InventoryBoxHandler.GetInventoryBoxesInMatrix(start, end, this.InventoryBoxHandler.InventoryBoxes);
            bool error = boxes.Find(o => o.Item != null && o.Item != drag.Item) != null || boxes.Count != (drag.Item.Size.X * drag.Item.Size.Y) || !InventoryBoxHandler.isValidItem(drag.Item);
            drag.isValid = !error;
            boxes.ForEach(o => o.Grid.Background = error ? InventoryBoxHandler.ErrorColor : InventoryBoxHandler.HighlightColor);
            drag.HoverFields = boxes;
            /* if (!error)
             {
                 Border b = this.InventoryBoxHandler.GetImageOverlay(drag.Item, boxes);
                 b.IsHitTestVisible = false;
                 drag.InvImagePreview = b;
                 gridInvMainImages.Children.Add(b);
                 e.Effects = DragDropEffects.Scroll;
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

        private void ShowQuickActions()
        {
            quickActions.Visibility = Visibility.Visible;
            bool set = previewDrag.Item.Set != null && previewDrag.Item.Set.ID != -1;
            bool stash = this.Stash != null && this.Stash.WindowState != WindowState.Minimized;
            bool equipment = this.SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipmentSlotFromItem(previewDrag.Item) != null;
            btnEquip.Opacity = equipment ? 1 : 0.5;
            btnEquip.AllowDrop = equipment;
            btnAddSet.Opacity = set ? 1 : 0.5;
            btnAddSet.AllowDrop = set;
            btnEquipSet.Opacity = set ? 1 : 0.5;
            btnEquipSet.AllowDrop = set;
            ((TextBlock)btnAddAllToInv.Child).Text = stash ? "Add all to Stash" : "Add all to Inventory";
            ((TextBlock)btnAddToInv.Child).Text = stash ? "Add to Stash" : "Add to Inventory";
            ((TextBlock)btnAddSet.Child).Text = stash ? "Add Set to Stash" : "Add Set to Inventory";
            ((TextBlock)btnFillInv.Child).Text = stash ? "Fill Stash" : "Fill Inventory";
            foreach (Border child in wpQuickActions.Children.OfType<Border>())
                child.BorderBrush = Util.ColorFromString("#FF483D85");
        }

        public void Window_MouseMove(object sender, MouseEventArgs e)
        {
            //Debug.WriteLine(e.GetPosition(MainGrid));
            // Reset inventory if no drag is in process
            if (e.LeftButton == MouseButtonState.Released)
            {
                customCursor = null;
                previewDrag = null;
                quickActions.Visibility = Visibility.Collapsed;
                if (activeDrag)
                {
                    ((EquipmentView)gridEquipment.Children[0]).SetAllowDrop(null);
                    this.activeDrag = false;
                    this.UpdateInventory();
                    this.UpdateEquippedItems();
                    this.UpdateStash();
                }
            }
            else if (previewDrag != null && !activeDrag)
            {
                Point curPos = e.GetPosition(this);
                var delta = curPos - previewDrag.oriPos;

                if (Math.Abs(delta.Length) < 5) return;

                this.InventoryBoxHandler.InventoryBoxes.Concat(this.InventoryBoxHandler.StashBoxes).ToList().FindAll(o => o.InvImage != null).ForEach(o => { o.InvImage.Opacity = 0.8; o.InvImage.IsHitTestVisible = false; });

                if (previewDrag.Item == null)
                {
                    this.previewDrag = null;
                    return;
                }

                if (previewDrag.CurrentFields != null)
                    previewDrag.CurrentFields.ForEach(o => o.Item = null);

                if (previewDrag.Item.InvImage != null)
                    previewDrag.Item.InvImage.Visibility = Visibility.Collapsed;

                if (previewDrag.EquipmentSlot != null)
                {
                    previewDrag.EquipmentSlot.SetTempItem(null);
                    this.ResetEquipmentSlot(previewDrag.sender, previewDrag.EquipmentSlot);
                }

                if (previewDrag.Type == DragHandler.Origin.ItemList)
                {
                    if (previewDrag.Item.Slot.ID != 15 && SelectedInv == ItemHandler.InvType.Socketables)
                        controlSelectedInventory.SelectedItem = InvMain;
                    else if (previewDrag.Item.Slot.ID != 12 && SelectedInv == ItemHandler.InvType.Potion)
                        controlSelectedInventory.SelectedItem = InvMain;

                    ShowQuickActions();
                }

                DataObject dataObject = new DataObject(typeof(ItemDrag), previewDrag);
                ((EquipmentView)gridEquipment.Children[0]).SetAllowDrop(previewDrag.Item);
                this.activeDrag = true;
                DragDrop.DoDragDrop(previewDrag.sender, dataObject, DragDropEffects.Scroll);
            }
        }

        public void equipmentSlotStartDrag(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            EquipmentSlot slot = (EquipmentSlot)border.DataContext;
            if (slot == null) return;

            ItemDrag drag = new ItemDrag(slot, slot.Item, DragHandler.Origin.Equip);
            drag.sender = sender as Border;
            drag.oriPos = e.GetPosition(this);
            previewDrag = drag;
        }

        public void ResetEquipmentSlot(Border border, EquipmentSlot slot)
        {
            if (slot == null) return;
            Image img = border.Child as Image;
            img.Source = new BitmapImage(new Uri(slot.Sprite));
            img.Opacity = slot.Item == null ? 0.6 : 1;
            border.Background = Util.ColorFromString(slot.Background);
            border.BorderBrush = Util.ColorFromString(slot.Border);
            border.ToolTip = slot.Item == null ? null : new ItemTooltip(slot.Item);
            border.DataContext = null;
            border.DataContext = slot;
        }

        public void equipmentSlotDragOver(object sender, DragEventArgs e)
        {
            Border border = sender as Border;
            border.BorderBrush = Util.ColorFromString("#a3895b");
        }

        public void equipmentSlotDragLeave(object sender, DragEventArgs e)
        {
            Border border = sender as Border;
            EquipmentSlot slot = border.DataContext as EquipmentSlot;
            if (slot == null) return;
            border.BorderBrush = Util.ColorFromString(slot.Border);
        }

        public void equipmentSlotDragDrop(object sender, DragEventArgs e)
        {
            Border border = sender as Border;
            EquipmentSlot newSlot = border.DataContext as EquipmentSlot;
            ItemDrag drag = e.Data.GetData(typeof(ItemDrag)) as ItemDrag;
            if (newSlot == null) return;

            switch (drag.Type)
            {
                case DragHandler.Origin.ItemList:
                    newSlot.SetItem(drag.Item.DeepCopy());
                    break;
                case DragHandler.Origin.Equip:
                    if (newSlot != drag.EquipmentSlot)
                    {
                        Item cur = newSlot.Item;
                        newSlot.SetItem(drag.Item.DeepCopy());
                        drag.EquipmentSlot.SetItem(cur == null ? null : cur.DeepCopy());
                    }
                    else
                        newSlot.ResetTempItem();

                    newSlot.temp = null;
                    drag.EquipmentSlot.temp = null;
                    break;
                case DragHandler.Origin.Inventory:
                    Item old = newSlot.Item;
                    newSlot.SetItem(drag.Item.DeepCopy());
                    this.SaveFileHandler.SelectedFile.Inventory.RemoveItem(drag.Item);
                    if (old != null) this.SaveFileHandler.SelectedFile.Inventory.AddItem(old);
                    this.UpdateInventory();
                    break;
                case DragHandler.Origin.Stash:
                    newSlot.SetItem(drag.Item.DeepCopy());
                    this.SaveFileHandler.Shop.Stash.Remove(drag.Item);
                    this.UpdateStash();
                    break;
            }
            this.UpdateEquippedItems();
            this.previewDrag = null;
            this.activeDrag = false;
        }

        private void TabItem_DragOver(object sender, DragEventArgs e)
        {
            ItemDrag drag = e.Data.GetData(typeof(ItemDrag)) as ItemDrag;
            if (drag == null) return;
            if (drag.Type == DragHandler.Origin.InvSelect || drag.Type == DragHandler.Origin.StashSelect) return;
            controlSelectedInventory.SelectedItem = sender as TabItem;
        }

        public void equipmentLoaded(object sender, RoutedEventArgs e)
        {
            Border border = sender as Border;
            EquipmentSlot equipmentSlot = border.DataContext as EquipmentSlot;
            if (equipmentSlot == null || equipmentSlot.Item == null) return;
            border.ContextMenu = new ContextMenuInv(equipmentSlot.Item, DragHandler.Origin.Equip, equipmentSlot);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (textBoxItemlistSearch.IsFocused) comboBoxItemFilterSlot.Focus();
        }

        public void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (previewDrag == null) return;

            // No drag was initiated. Proceeding with normal click

            switch (previewDrag.Type)
            {
                case DragHandler.Origin.Equip:
                    comboBoxItemFilterSlot.SelectedItem = SlotHandler.GetSlotFromUniqueName(previewDrag.EquipmentSlot.Slot.Name);
                    break;
                case DragHandler.Origin.Inventory:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        InventoryBox box = this.InventoryBoxHandler.FindSpace(previewDrag.Item, this.InventoryBoxHandler.StashBoxes);
                        if (box == null) return;
                        this.SaveFileHandler.SelectedFile.Inventory.RemoveItem(previewDrag.Item);
                        previewDrag.Item.Inv = this.SelectedStash;
                        previewDrag.Item.InvPos = box.Position;
                        this.SaveFileHandler.Shop.AddItemToStash(previewDrag.Item, false);
                        this.UpdateInventory();
                        this.UpdateStash();
                    }
                    break;
                case DragHandler.Origin.Stash:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        InventoryBox box = this.InventoryBoxHandler.FindSpace(previewDrag.Item, this.InventoryBoxHandler.InventoryBoxes);
                        if (box == null) return;
                        this.SaveFileHandler.Shop.Stash.Remove(previewDrag.Item);
                        previewDrag.Item.Inv = this.SelectedInv;
                        previewDrag.Item.InvPos = box.Position;
                        this.SaveFileHandler.SelectedFile.Inventory.AddItem(previewDrag.Item, false);
                        this.UpdateInventory();
                        this.UpdateStash();
                    }
                    break;
            }
            previewDrag = null;
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            Grid sp = sender as Grid;
            sp.Children.Add(((Item)sp.DataContext).SizeGrid);
        }

        private void buttonSort_Click(object sender, RoutedEventArgs e)
        {
            this.InventoryBoxHandler.InventoryBoxes.ForEach(o => o.Item = null);
            this.SaveFileHandler.SelectedFile.Inventory.InventoryItems.FindAll(o => o.Inv == this.SelectedInv).OrderByDescending(o => o.Size.X * o.Size.Y).ToList().ForEach(o =>
            {
                InventoryBox box = this.InventoryBoxHandler.FindSpace(o, this.InventoryBoxHandler.InventoryBoxes);
                o.InvPos = box.Position;
                this.InventoryBoxHandler.GetInventoryBoxesInMatrix(box.Position, o, this.InventoryBoxHandler.InventoryBoxes).ForEach(o2 => o2.Item = o);
            });
            this.UpdateInventory();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    this.Refresh();
                    break;
            }
        }



        private void buttonStash_Click(object sender, RoutedEventArgs e)
        {
            this.stashOpenend = true;
            if (this.Stash != null)
            {
                this.Stash.WindowState = this.Stash.WindowState == WindowState.Normal ? WindowState.Minimized : WindowState.Normal;
            }

            this.UpdateStash();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Debug.WriteLine(this.Width + " - " + this.Height);
        }


        private void vbMain_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Viewbox_Loaded(object sender, RoutedEventArgs e)
        {
            Viewbox vb = sender as Viewbox;
            Border border = vb.Child as Border;
            vb.Width = border.ActualWidth * MainWindow.INSTANCE.SizeFactor;
            vb.Height = border.ActualHeight * MainWindow.INSTANCE.SizeFactor;
        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            //Border border = sender as Border;
            //border.ContextMenu = new ContextMenuItemlist((Item)border.DataContext);
        }
        private readonly string highlightColor = "#766da8";
        private readonly string defaultcolor = "#FF483D85";
        private void Border_DragEnter(object sender, DragEventArgs e)
        {
            Border border = sender as Border;
            border.Tag = border.BorderBrush;
            border.BorderBrush = Util.ColorFromString(highlightColor);
        }

        private void Border_DragLeave(object sender, DragEventArgs e)
        {
            Border border = sender as Border;
            border.BorderBrush = Util.ColorFromString(defaultcolor);
        }

        private void btnAddToInv_Drop(object sender, DragEventArgs e)
        {
            ItemDrag drag = e.Data.GetData(typeof(ItemDrag)) as ItemDrag;
            if (drag == null || drag.Item == null) return;

            Item item = drag.Item.DeepCopy();
            bool stash = this.Stash != null && this.Stash.WindowState != WindowState.Minimized;

            this.activeDrag = false;
            this.previewDrag = null;
            if (stash)
            {
                this.SaveFileHandler.Shop.AddItemToStash(item, true);
                this.UpdateStash();
            }
            else
            {
                this.SaveFileHandler.SelectedFile.Inventory.AddItem(item, true);
                this.UpdateInventory();
            }
            ((EquipmentView)gridEquipment.Children[0]).SetAllowDrop(null);
        }

        private void btnEquip_Drop(object sender, DragEventArgs e)
        {
            previewDrag = null;
            activeDrag = false;
            ItemDrag drag = e.Data.GetData(typeof(ItemDrag)) as ItemDrag;
            if (drag == null || drag.Item == null) return;

            Item item = drag.Item.DeepCopy();
            EquipmentSlot slot = this.SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipmentSlotFromItem(item);
            if (slot == null) return;

            Item old = slot.Item;
            if (old != null)
            {
                old = old.DeepCopy();
                this.SaveFileHandler.SelectedFile.Inventory.AddItem(old, true);
                this.UpdateInventory();
            }
            slot.SetItem(item);
            this.UpdateEquippedItems();
        }

        private void btnFillInv_Drop(object sender, DragEventArgs e)
        {
            previewDrag = null;
            activeDrag = false;
            ItemDrag drag = e.Data.GetData(typeof(ItemDrag)) as ItemDrag;
            if (drag == null || drag.Item == null) return;

            bool stash = this.Stash != null && this.Stash.WindowState != WindowState.Minimized;
            Item item = drag.Item.DeepCopy();
            while (true)
            {
                InventoryBox start = this.InventoryBoxHandler.FindSpace(item, stash ? this.InventoryBoxHandler.StashBoxes : this.InventoryBoxHandler.InventoryBoxes);
                if (start == null) break;
                item.InvPos = start.Position;
                item.Inv = stash ? this.SelectedStash : this.SelectedInv;
                if (stash) this.SaveFileHandler.Shop.AddItemToStash(item, false);
                else this.SaveFileHandler.SelectedFile.Inventory.AddItem(item, false);
                List<InventoryBox> boxes = this.InventoryBoxHandler.GetInventoryBoxesInMatrix(start.Position, item, stash ? this.InventoryBoxHandler.StashBoxes : this.InventoryBoxHandler.InventoryBoxes);
                boxes.ForEach(o => o.Item = item);
            }
            if (stash) this.UpdateStash();
            else this.UpdateInventory();
            ((EquipmentView)gridEquipment.Children[0]).SetAllowDrop(null);
        }

        private void btnAddAllToInv_Drop(object sender, DragEventArgs e)
        {
            previewDrag = null;
            activeDrag = false;
            ItemDrag drag = e.Data.GetData(typeof(ItemDrag)) as ItemDrag;
            if (drag == null || drag.Item == null) return;

            bool stash = this.Stash != null && this.Stash.WindowState != WindowState.Minimized;
            List<Item> items = listBox_Items.ItemsSource as List<Item>;
            foreach (Item item in items)
            {
                if (items.IndexOf(item) < items.IndexOf(drag.Item)) continue;
                InventoryBox start = this.InventoryBoxHandler.FindSpace(item, stash ? this.InventoryBoxHandler.StashBoxes : this.InventoryBoxHandler.InventoryBoxes);
                if (start == null) continue;

                Item item2 = item.DeepCopy();
                item2.InvPos = start.Position;
                item2.Inv = stash ? this.SelectedStash : this.SelectedInv;
                if (stash) this.SaveFileHandler.Shop.AddItemToStash(item2, false);
                else this.SaveFileHandler.SelectedFile.Inventory.AddItem(item2, false);
                List<InventoryBox> boxes = this.InventoryBoxHandler.GetInventoryBoxesInMatrix(start.Position, item2, stash ? this.InventoryBoxHandler.StashBoxes : this.InventoryBoxHandler.InventoryBoxes);
                boxes.ForEach(o => o.Item = item2);
            }
            if (stash) this.UpdateStash();
            else this.UpdateInventory();
            ((EquipmentView)gridEquipment.Children[0]).SetAllowDrop(null);
        }

        private void btnAddSet_Drop(object sender, DragEventArgs e)
        {
            previewDrag = null;
            activeDrag = false;
            ItemDrag drag = e.Data.GetData(typeof(ItemDrag)) as ItemDrag;
            if (drag == null || drag.Item == null) return;

            bool stash = this.Stash != null && this.Stash.WindowState != WindowState.Minimized;
            List<Item> items = this.ItemHandler.AllItems.FindAll(o => o.Set != null && o.Set.ID == drag.Item.Set.ID);
            foreach (Item item in items)
            {
                InventoryBox start = this.InventoryBoxHandler.FindSpace(item, stash ? this.InventoryBoxHandler.StashBoxes : this.InventoryBoxHandler.InventoryBoxes);
                if (start == null) continue;

                Item item2 = item.DeepCopy();
                item2.InvPos = start.Position;
                item2.Inv = stash ? this.SelectedStash : this.SelectedInv;
                if (stash) this.SaveFileHandler.Shop.AddItemToStash(item2, false);
                else this.SaveFileHandler.SelectedFile.Inventory.AddItem(item2, false);
                List<InventoryBox> boxes = this.InventoryBoxHandler.GetInventoryBoxesInMatrix(start.Position, item2, stash ? this.InventoryBoxHandler.StashBoxes : this.InventoryBoxHandler.InventoryBoxes);
                boxes.ForEach(o => o.Item = item2);
            }
            if (stash) this.UpdateStash();
            else this.UpdateInventory();
            ((EquipmentView)gridEquipment.Children[0]).SetAllowDrop(null);
        }

        private void btnEquipSet_Drop(object sender, DragEventArgs e)
        {
            previewDrag = null;
            activeDrag = false;
            ItemDrag drag = e.Data.GetData(typeof(ItemDrag)) as ItemDrag;
            if (drag == null || drag.Item == null) return;

            List<Item> items = this.ItemHandler.AllItems.FindAll(o => o.Set != null && o.Set.ID == drag.Item.Set.ID);
            foreach (Item item in items)
            {
                Item item2 = item.DeepCopy();
                if (item.Slot.Name != "Charm")
                {
                    EquipmentSlot slot = this.SaveFileHandler.SelectedFile.Inventory.Equipment.GetEquipmentSlotFromItem(item);
                    if (slot == null) continue;
                    Item old = slot.Item;
                    if (old != null) this.SaveFileHandler.SelectedFile.Inventory.AddItem(old.DeepCopy(), true);
                    slot.SetItem(item2);
                }
                else
                {
                    InventoryBox start = this.InventoryBoxHandler.FindCharmSpace(item);
                    if (start == null) continue;
                    item2.InvPos = start.Position;
                    item2.Inv = this.SelectedInv;
                    this.SaveFileHandler.SelectedFile.Inventory.AddItem(item2, false);
                }
            }
            this.UpdateInventory();
            this.UpdateEquippedItems();
        }
    }
}
