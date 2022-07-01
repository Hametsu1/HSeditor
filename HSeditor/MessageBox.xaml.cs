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
        public MessageBox(string Title, string Description, string ButtonContent, bool TextBox = false, string TextBoxText = "", bool CancelButton = false, bool locked = false)
        {
            this.Owner = MainWindow.INSTANCE;
            InitializeComponent();
            this.labelTitle.Text = Title;
            TooltipHandler.SetHyperlink(textBlockDescription, Description);
            this.buttonOK.Content = ButtonContent;
            MainWindow.INSTANCE.popupGrid.Opacity = 0.5;
            textBoxItemName.Text = TextBoxText;
            if (Description == "") gridDescription.Visibility = Visibility.Collapsed;
            if (TextBox) textBoxItemName.Visibility = Visibility.Visible;
            if (CancelButton) buttonCancel.Visibility = Visibility.Visible;
            this.textBoxItemName.Focus();
            this.textBoxItemName.Select(textBoxItemName.Text.Length, 0);
            if (locked) buttonOK.Visibility = Visibility.Hidden;
        }

        private void Exit()
        {
            MainWindow.INSTANCE.popupGrid.Opacity = 0;
            this.Close();
        }
        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxItemName.Visibility == Visibility.Visible && textBoxItemName.Text == "") return;
            this.TextBox = textBoxItemName.Text;
            Cancel = false;
            this.Exit();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.TextBox = null;
            this.Exit();
        }

        private void textBoxItemName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (this.textBoxItemName.Text == "") return;
                this.TextBox = textBoxItemName.Text;
                this.Exit();
            }
        }
    }
}
