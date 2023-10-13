using System.Windows;

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
