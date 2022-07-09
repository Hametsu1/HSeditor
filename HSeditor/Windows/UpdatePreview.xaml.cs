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
    public partial class UpdatePreview : Window
    {
        public bool Cancel = true;
        public UpdatePreview(string CurrentVersion, string LatestVersion)
        {
            this.Owner = MainWindow.INSTANCE;
            InitializeComponent();
            this.tbCurrent.Text = CurrentVersion;
            this.tbLatest.Text = LatestVersion;
            MainWindow.INSTANCE.popupGrid.Opacity = 0.5;
        }


        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            Cancel = false;
            this.Exit();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Exit();
        }

        void Exit()
        {
            MainWindow.INSTANCE.popupGrid.Opacity = 0;
            this.Close();
        }


    }
}
