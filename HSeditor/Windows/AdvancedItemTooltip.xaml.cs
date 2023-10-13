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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HSeditor.Windows
{
    /// <summary>
    /// Interaktionslogik für AdvancedItemTooltip.xaml
    /// </summary>
    public partial class AdvancedItemTooltip : UserControl
    {
        public AdvancedItemTooltip(Item item)
        {
            InitializeComponent();
            item.SaveItem = item.GetItemObject();
            this.DataContext = item;
        }
    }
}
