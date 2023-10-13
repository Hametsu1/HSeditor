using HSeditor.Classes.Util;
using System;
using System.Windows;
using System.Windows.Controls;

namespace HSeditor.Windows
{
    /// <summary>
    /// Interaktionslogik für ErrorBox.xaml
    /// </summary>
    public partial class ErrorBox : Window
    {
        public ErrorBox(Exception error)
        {
            InitializeComponent();
            Clipboard.SetText(Util.GetFullMessage(error));
        }

        private void textBlockDescription_Initialized(object sender, EventArgs e)
        {
            TooltipHandler.SetToolTip(sender as TextBlock);
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
