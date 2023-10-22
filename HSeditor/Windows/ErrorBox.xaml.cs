using HSeditor.Classes.Other;
using HSeditor.Classes.Util;
using Squirrel;
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
            Clipboard.SetText(error.ToString());
            tbError.Text = Util.GetFullMessage(error);
            SetVersion();
        }

        async void SetVersion()
        {
            tbTitle.Text = $"Unknown Error *[{MainWindow.INSTANCE.UpdateHandler.Version}]*";
            TooltipHandler.SetToolTip(tbTitle);
        }

        private void textBlockDescription_Initialized(object sender, EventArgs e)
        {
            TooltipHandler.SetToolTip(sender as TextBlock);
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StackPanel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            borderError.Visibility = borderError.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            showError.Text = borderError.Visibility == Visibility.Visible ? "Hide Error Message" : "Show Error Message";
        }
    }
}
