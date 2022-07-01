using HSeditor.Classes;
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
    /// Interaktionslogik für SaveBuild.xaml
    /// </summary>
    public partial class SaveBuild : Window
    {
        public bool Cancel = true;
        public SaveBuild(Class Class)
        {
            this.Owner = MainWindow.INSTANCE;
            InitializeComponent();
            MainWindow.INSTANCE.popupGrid.Opacity = 0.5;
            textBoxBuildName.Text = Class.Name;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            Cancel = false;
            this.Exit();
        }

        void Exit()
        {
            MainWindow.INSTANCE.popupGrid.Opacity = 0;
            this.Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Exit();
        }
    }
}
