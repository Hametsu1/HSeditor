using HSeditor.Classes.Util;
using System.Windows;
using System.Windows.Input;

namespace HSeditor
{
    /// <summary>
    /// Interaktionslogik für MessageBox.xaml
    /// </summary>
    public partial class MessageBox : Window
    {
        public string TextBox = "";
        public bool Cancel = true;
        public MessageBox(string Title, string Description, string ButtonContentOK, bool CancelButton = false, string ButtonContentCancel = "", bool locked = false)
        {
            this.Owner = MainWindow.INSTANCE;
            InitializeComponent();
            this.labelTitle.Text = Title;
            TooltipHandler.SetHyperlink(textBlockDescription, Description);
            this.buttonOK.Content = ButtonContentOK;
            this.buttonCancel.Content = ButtonContentCancel;
            MainWindow.INSTANCE.popupGrid.Opacity = 0.5;
            if (Description == "") gridDescription.Visibility = Visibility.Collapsed;
            if (CancelButton) buttonCancel.Visibility = Visibility.Visible;
            if (locked) buttonOK.Visibility = Visibility.Hidden;
        }

        private void Exit()
        {
            MainWindow.INSTANCE.popupGrid.Opacity = 0;
            this.Close();
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
    }
}
