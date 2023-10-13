using Squirrel;
using System.Diagnostics;
using System.Linq;

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
            try
            {
                this.mgr = await UpdateManager.GitHubUpdateManager(@"https://github.com/Hametsu1/HSeditor");
            }
            catch
            {

            }

        }

        [Conditional("RELEASE")]
        public async void CheckForUpdate()
        {
            try
            {
                UpdateInfo updateInfo = await mgr.CheckForUpdate();

                this.Version = updateInfo.CurrentlyInstalledVersion == null ? "0.0.0" : updateInfo.CurrentlyInstalledVersion.Version.ToString();
                MainWindow.INSTANCE.Title = new string($"HSeditor - Name: {MainWindow.INSTANCE.SaveFileHandler.SelectedFile.HeroInfo.Name} | Class: {MainWindow.INSTANCE.SaveFileHandler.SelectedFile.HeroInfo.Class.Name} | ID: {MainWindow.INSTANCE.SaveFileHandler.SelectedFile.ID} | Version: {this.Version}");

                if (updateInfo.ReleasesToApply.Any())
                {
                    updateInfo.FutureReleaseEntry.GetReleaseNotes(updateInfo.FutureReleaseEntry.PackageName);
                    UpdatePreview mb = new UpdatePreview(this.Version, updateInfo.FutureReleaseEntry.Version.ToString());
                    mb.ShowDialog();

                    if (!mb.Cancel)
                    {
                        DownloadProgress dp = new DownloadProgress();
                        dp.Show();
                        await mgr.UpdateApp(dp.UpdateProgress);
                        UpdateManager.RestartApp("HSeditor.exe");
                    }
                }
            }
            catch { }
        }
    }
}
