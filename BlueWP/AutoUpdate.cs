using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP
{
  public class AutoUpdate
  {
    private class Asset
    {
      public string browser_download_url { get; set; }
    }
    private class Release
    {
      public string url { get; set; }
      public string tag_name { get; set; }
      public string name { get; set; }
      public string published_at { get; set; }
      public Asset[] assets { get; set; }
    }

    public async static Task CheckForUpdates(Type _app)
    {
      string url = "https://api.github.com/repos/Gargaj/BlueWP/releases";
      using (var hc = new System.Net.Http.HttpClient())
      {
        string contents = "";
        using (var requestMessage = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url))
        {
          requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:133.0) Gecko/20100101 Firefox/133.0");
          try
          {
            var response = await hc.SendAsync(requestMessage);
            contents = await response.Content.ReadAsStringAsync();
          }
          catch (Exception)
          {
          }
        }

        var releases = JsonConvert.DeserializeObject<Release[]>(contents);
        if (releases != null)
        {
          var release = releases?.OrderByDescending(s => s.published_at)?.FirstOrDefault();
          if (release != null)
          {
            string tag_name = release.tag_name;
            string name = release.name;
            Version ourVersion = _app.GetTypeInfo().Assembly.GetName().Version;
            Version latestVersion = new Version(tag_name.Substring(0, 1) == "v" ? tag_name.Substring(1) : tag_name);
            if (latestVersion.CompareTo(ourVersion) > 0)
            {
              await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, async () => {
                var dialog = new ContentDialog
                {
                  Content = new TextBlock { Text = $"A new version of BlueWP is available: {tag_name}\n\n{name}\n\nDo you want to download it?", TextWrapping = TextWrapping.WrapWholeWords },
                  Title = "BlueWP version check",
                  IsSecondaryButtonEnabled = true,
                  PrimaryButtonText = "Yes",
                  SecondaryButtonText = "No"
                };
                if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                {
                  var releaseURL = release.assets.Count() > 0 ? release.assets[0].browser_download_url : release.url;
                  await Windows.System.Launcher.LaunchUriAsync(new Uri(releaseURL));
                }
              });
            }
          }
        }
      }
    }

  }
}
