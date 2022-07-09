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
    /// Interaktionslogik für DownloadProgress.xaml
    /// </summary>
    public partial class DownloadProgress : Window
    {
        public DownloadProgress()
        {
            this.Owner = MainWindow.INSTANCE;
            InitializeComponent();
            MainWindow.INSTANCE.popupGrid.Opacity = 0.5;
        }

        public void UpdateProgress(int i)
        {
            Application.Current.Dispatcher.Invoke(() => { this.DownloadProgressBar.Value = i; this.tbProgress.Text = $"{i}%"; });
        }
    }
}
