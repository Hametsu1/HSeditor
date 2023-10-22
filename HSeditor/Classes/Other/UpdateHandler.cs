using Squirrel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace HSeditor.Classes.Other
{
    public class UpdateHandler
    {
        public string Version { get; private set; }

        public UpdateHandler()
        {
            this.Version = "0.0.0";
            this.CheckForUpdate();
        }

        [Conditional("RELEASE")]
        public async void CheckForUpdate()
        {
            try
            {
                using (var mgr = await UpdateManager.GitHubUpdateManager("https://github.com/Hametsu1/HSeditor"))
                {
                    UpdateInfo updateInfo = await mgr.CheckForUpdate();
                    this.Version = updateInfo.CurrentlyInstalledVersion == null ? "0.0.0" : updateInfo.CurrentlyInstalledVersion.ToString();

                    if (updateInfo.ReleasesToApply.Any())
                    {
                        UpdatePreview mb = new UpdatePreview(Version, updateInfo.FutureReleaseEntry.Version.ToString());
                        mb.ShowDialog();

                        if (!mb.Cancel)
                        {
                            DownloadProgress dp = new DownloadProgress();
                            dp.Show();
                            mgr.UpdateApp(dp.UpdateProgress);
                            UpdateManager.RestartApp("HSeditor.exe");
                        }
                    }
                }
            }
            catch
            {
                this.Version = "0.0.0";
            }
        }
    }
}


