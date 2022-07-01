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

namespace HSeditor
{
    /// <summary>
    /// Interaktionslogik für AddFavorite.xaml
    /// </summary>
    public partial class AddFavorite : Window
    {
        public bool Cancel = false;
        public AddFavorite(string Name)
        {
            this.Owner = MainWindow.INSTANCE;
            InitializeComponent();
            MainWindow.INSTANCE.popupGrid.Opacity = 0.5;
            textBlockName.Text = Name;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Exit();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (textBlockName.Text == "") return;
            Cancel = false;
            this.Exit();
        }

        void Exit()
        {
            MainWindow.INSTANCE.popupGrid.Opacity = 0;
            this.Close();
        }
    }
}
