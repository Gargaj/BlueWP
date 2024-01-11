using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Controls
{
  public class NotificationDataTemplateSelector : DataTemplateSelector
  {
    public DataTemplate Like { get; set; }
    public DataTemplate Repost { get; set; }
    public DataTemplate Follow { get; set; }
    public DataTemplate Mention { get; set; }
    public DataTemplate Reply { get; set; }
    public DataTemplate Quote { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
      var notification = item as ATProto.Lexicons.App.BSky.Notification.Notification;
      switch (notification.reason)
      {
        case "like": return Like;
        case "repost": return Repost;
        case "follow": return Follow;
        case "mention": return Mention;
        case "reply": return Reply;
        case "quote": return Quote;
        default: return null;
      }
    }
  }
}
