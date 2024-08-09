using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Inlays
{
  public partial class ConvoListInlay : UserControl, INotifyPropertyChanged
  {
    private App _app;
    private Pages.MainPage _mainPage;

    public ConvoListInlay()
    {
      InitializeComponent();
      _app = (App)Application.Current;
      Loaded += NotificationsInlay_Loaded;
      DataContext = this;
    }

    public List<Convo> Convos { get; set; }

    private void NotificationsInlay_Loaded(object sender, RoutedEventArgs e)
    {
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
    }

    public void Flush()
    {
    }

    public async Task Refresh()
    {
      _mainPage?.StartLoading();

      var response = await _mainPage.Get<ATProto.Lexicons.Chat.BSky.Convo.ListConvosResponse>(new ATProto.Lexicons.Chat.BSky.Convo.ListConvos()
      {
      });
      if (response != null)
      {
        Convos = response.convos.Select(s => new Convo() {
          ConvoView = s,
          CurrentUserDID = _app.Client.DID
        }).ToList();
        OnPropertyChanged(nameof(Convos));
      }

      _mainPage?.EndLoading();
    }

    public class Convo
    {
      public ATProto.Lexicons.Chat.BSky.Convo.Defs.ConvoView ConvoView { get; set; }
      public string CurrentUserDID { get; set; }

      public string ID => ConvoView.id;
      public string PartnerAvatarURL => ConvoView.members.Where(s=>s.did != CurrentUserDID).FirstOrDefault().avatar;
      public string PartnerNames => string.Join(", ", ConvoView.members.Where(s => s.did != CurrentUserDID).Select(s => s.DisplayName));
      public string LastMessage => (ConvoView.lastMessage as ATProto.Lexicons.Chat.BSky.Convo.Defs.MessageView)?.text ?? string.Empty;
      public bool IsRead => ConvoView.unreadCount == 0;
    }

    private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
    {
      var convo = e.ClickedItem as Convo;
      if (convo != null)
      {
        await _mainPage.SwitchToConvoInlay(convo.ID);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises this object's PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The property that has a new value.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
