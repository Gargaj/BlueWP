using System;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Controls
{
  public sealed partial class Post : UserControl
  {
    public Post()
    {
      this.InitializeComponent();
    }

    private async void OpenExternalURL_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
      var button = e.OriginalSource as Button;
      if (button == null)
      {
        return;
      }
      var dataContext = button.DataContext as ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost;
      if (dataContext == null)
      {
        return;
      }
      await Windows.System.Launcher.LaunchUriAsync(new Uri(dataContext.PostEmbedExternalURL));
    }
  }
}
