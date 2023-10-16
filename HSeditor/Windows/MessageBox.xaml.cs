using HSeditor.Classes.Util;
using System.Windows;
using System.Windows.Controls;

namespace HSeditor
{
    /// <summary>
    /// Interaktionslogik für MessageBox.xaml
    /// </summary>
    public partial class MessageBox : Window
    {
        public string TextBox = "";
        public bool Cancel = true;
        public MessageBox(string Title, string Description, string ButtonContentOK, bool CancelButton = false, string ButtonContentCancel = "", bool locked = false, string Tag = "")
        {
            this.Owner = MainWindow.INSTANCE;
            InitializeComponent();
            this.labelTitle.Text = Title;
            textBlockDescription.Tag = Tag;
            if (Description.Contains("http") || Description.Contains("https"))
                TooltipHandler.SetHyperlink(textBlockDescription, Description);
            else
                textBlockDescription.Text = Description;
            this.buttonOK.Content = ButtonContentOK;
            this.buttonCancel.Content = ButtonContentCancel;
            MainWindow.INSTANCE.popupGrid.Opacity = 0.5;
            if (Description == "") gridDescription.Visibility = Visibility.Collapsed;
            if (CancelButton) buttonCancel.Visibility = Visibility.Visible;
            if (locked) buttonOK.Visibility = Visibility.Hidden;
            TooltipHandler.SetToolTip(textBlockDescription);
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

        private void gridDescription_Initialized(object sender, System.EventArgs e)
        {

        }


    }
}
