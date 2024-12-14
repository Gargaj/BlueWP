using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BlueWP.Controls.PostList
{
  public class PostListSearch : PostListBase
  {
    public string SearchTerm
    {
      get { return (string)GetValue(SearchTermProperty); }
      set { SetValue(SearchTermProperty, value); }
    }
    public static readonly DependencyProperty SearchTermProperty = DependencyProperty.Register("SearchTerm", typeof(string), typeof(PostListProfile), new PropertyMetadata(string.Empty));

    public SortCriteria Sort
    {
      get { return (SortCriteria)GetValue(SortProperty); }
      set { SetValue(SortProperty, value); }
    }
    public static readonly DependencyProperty SortProperty = DependencyProperty.Register("Sort", typeof(SortCriteria), typeof(PostListProfile), new PropertyMetadata(SortCriteria.Top));

    public async override Task<List<ATProto.IPost>> GetListItems()
    {
      var response = await _mainPage.Get<ATProto.Lexicons.App.BSky.Feed.SearchPosts.Response>(new ATProto.Lexicons.App.BSky.Feed.SearchPosts()
      {
        limit = 60,
        q = SearchTerm,
        sort = Sort == SortCriteria.Top ? "top" : "latest",
      });
      return response?.posts.ToList<ATProto.IPost>();
    }

    public enum SortCriteria
    {
      Top,
      Latest
    }
  }
}
