using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueWP.ATProto;
using Windows.UI.Xaml;

namespace BlueWP.Controls.ProfileList
{
  public class ProfileListFollowers : ProfileListBase
  {
    public string ActorDID
    {
      get { return (string)GetValue(ActorDIDProperty); }
      set { SetValue(ActorDIDProperty, value); }
    }
    public static readonly DependencyProperty ActorDIDProperty = DependencyProperty.Register("ActorDID", typeof(string), typeof(ProfileListFollowers), new PropertyMetadata(string.Empty));

    public async override Task<List<ATProto.Lexicons.App.BSky.Actor.Defs.ProfileView>> GetProfileItems()
    {
      var response = await _mainPage.Get<ATProto.Lexicons.App.BSky.Graph.GetFollowers.Response>(new ATProto.Lexicons.App.BSky.Graph.GetFollowers()
      {
        actor = ActorDID,
      });
      return response?.followers;
    }
  }
}
