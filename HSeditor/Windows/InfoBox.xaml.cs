using HSeditor.Classes.Util;
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
    /// Interaktionslogik für InfoBox.xaml
    /// </summary>
    public partial class InfoBox : UserControl
    {
        WindowState stashstate;
        public InfoBox(string Title, string Description)
        {
            InitializeComponent();
            labelTitle.Text = Title;
            textBlockDescription.Text = Description;
            if (MainWindow.INSTANCE.Stash != null)
            {
                this.stashstate = MainWindow.INSTANCE.Stash.WindowState;
                MainWindow.INSTANCE.Stash.WindowState = WindowState.Minimized;
            }
            MainWindow.INSTANCE.ShowPopup(this);
        }

        private void textBlockDescription_Loaded(object sender, RoutedEventArgs e)
        {
            TooltipHandler.SetToolTip((TextBlock)sender);
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.INSTANCE.Stash != null) MainWindow.INSTANCE.Stash.WindowState = stashstate;
            MainWindow.INSTANCE.ClosePopup();
        }
    }
}
