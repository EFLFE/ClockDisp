using System;
using System.Threading;
using System.Threading.Tasks;
using Octokit;

namespace ClockDisp
{
    internal sealed class UpdaterData
    {
        public string Name;
        public string Url;
        public Version Ver;
        public string AssetName;
        public string AsserDownloadUrl;

        public UpdaterData(string name, string url, Version ver, string assetName, string asserDownloadUrl)
        {
            Name = name;
            Url = url;
            Ver = ver;
            AssetName = assetName;
            AsserDownloadUrl = asserDownloadUrl;
        }
    }

    internal static class Updater
    {
        public static event Action<UpdaterData> OnDetectedNewVersion;

        public static UpdaterData LastData;

        private static GitHubClient client;
        private static int timer;

        static Updater()
        {
            client = new GitHubClient(new ProductHeaderValue("EFLFE's_Updater"));
            ThreadPool.QueueUserWorkItem(Pool);
        }

        public static void ResetTimer()
        {
            timer = 0;
        }

        private static void Pool(object nil)
        {
            Thread.Sleep(2000);

            while (true)
            {
                try
                {
                    Task<Release> taskRelease = client.Repository.Release.GetLatest("EFLFE", "ClockDisp");
                    taskRelease.Wait();
                    Release release = taskRelease.Result;

                    if (release.Assets.Count > 0)
                    {
                        ReleaseAsset asset = release.Assets[0];

                        LastData = new UpdaterData(
                            release.Name,
                            release.Url,
                            new Version(release.TagName),
                            asset.Name,
                            asset.BrowserDownloadUrl);

                        OnDetectedNewVersion(LastData);
                        return;
                    }
                }
                catch { }

                timer = 1000 * 30;
                while (timer > 0)
                {
                    Thread.Sleep(1);
                    timer--;
                }
            }
        }

    }
}
