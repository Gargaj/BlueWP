using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BlueWP.Controls.PostList
{
  public class PostListProfile : PostListBase
  {
    public string ActorDID
    {
      get { return (string)GetValue(ActorDIDProperty); }
      set { SetValue(ActorDIDProperty, value); }
    }
    public static readonly DependencyProperty ActorDIDProperty = DependencyProperty.Register("ActorDID", typeof(string), typeof(PostListProfile), new PropertyMetadata(string.Empty));

    public async override Task<List<ATProto.IPost>> GetListItems()
    {
      var response = await _mainPage.Get<ATProto.Lexicons.App.BSky.Feed.GetAuthorFeed.Response>(new ATProto.Lexicons.App.BSky.Feed.GetAuthorFeed()
      {
        limit = 60,
        actor = ActorDID
      });
      return response?.feed.ToList<ATProto.IPost>();
    }
  }
}
