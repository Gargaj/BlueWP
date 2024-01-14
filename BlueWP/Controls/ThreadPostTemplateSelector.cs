using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Controls
{
  public class ThreadPostTemplateSelector : DataTemplateSelector
  {
    public string SelectedPostURI { get; set; }
    public DataTemplate SelectedPost { get; set; }
    public DataTemplate NormalPost { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
      var post = item as ATProto.Lexicons.App.BSky.Feed.Defs.PostView;
      return post?.uri == SelectedPostURI ? SelectedPost : NormalPost;
    }
  }
}
