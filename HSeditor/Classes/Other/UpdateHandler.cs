using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Squirrel;

namespace HSeditor.Classes.Other
{
    public class UpdateHandler
    {
        public string Version { get; private set; }
        UpdateManager mgr;

        public UpdateHandler()
        {
            this.Init();
        }

        async void Init()
        {
            this.mgr = await UpdateManager.GitHubUpdateManager(@"https://github.com/Hametsu1/HSeditor");
            Console.WriteLine();
        }

        public async void CheckForUpdate()
        {
            UpdateInfo updateInfo = await mgr.CheckForUpdate();

            try
            {
                this.Version = updateInfo.CurrentlyInstalledVersion.Version.ToString();
            }
            catch { this.Version = "0.0.0"; }
            MainWindow.INSTANCE.Title = new string($"HSeditor - Name: {MainWindow.INSTANCE.SaveFileHandler.SelectedFile.HeroInfo.Name} | Class: {MainWindow.INSTANCE.SaveFileHandler.SelectedFile.HeroInfo.Class.Name} | ID: {MainWindow.INSTANCE.SaveFileHandler.SelectedFile.ID} | Version: {this.Version}");

            if (updateInfo.ReleasesToApply.Any())
            {
                MessageBox mb = new MessageBox($"Update available! ({updateInfo.FutureReleaseEntry.Version})", "Do you want to install it now?", "Yes", false, "", true);
                mb.ShowDialog();

                if (!mb.Cancel)
                {
                    mb = new MessageBox($"Update is being installed!", "This may take a couple of seconds.", "OK", false, "", false, true);
                    mb.Show();
                    await mgr.UpdateApp();
                    mb.Close();
                    MessageBox mb2 = new MessageBox("Update installed!", "Update was successfully installed..", "OK");
                    mb2.ShowDialog();
                    UpdateManager.RestartApp("HSeditor.exe");
                }
            }

        }
    }
}
