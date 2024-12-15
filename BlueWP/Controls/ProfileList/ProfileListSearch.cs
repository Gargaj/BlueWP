using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueWP.ATProto;
using Windows.UI.Xaml;

namespace BlueWP.Controls.ProfileList
{
  public class ProfileListSearch : ProfileListBase
  {
    public string SearchTerm
    {
      get { return (string)GetValue(SearchTermProperty); }
      set { SetValue(SearchTermProperty, value); }
    }
    public static readonly DependencyProperty SearchTermProperty = DependencyProperty.Register("SearchTerm", typeof(string), typeof(ProfileListSearch), new PropertyMetadata(string.Empty));

    public async override Task<List<ATProto.Lexicons.App.BSky.Actor.Defs.ProfileView>> GetProfileItems()
    {
      var response = await _mainPage.Get<ATProto.Lexicons.App.BSky.Actor.SearchActors.Response>(new ATProto.Lexicons.App.BSky.Actor.SearchActors()
      {
        limit = 60,
        q = SearchTerm,
      });
      return response?.actors;
    }
  }
}
