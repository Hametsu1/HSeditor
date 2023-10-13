using System.Windows;

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
