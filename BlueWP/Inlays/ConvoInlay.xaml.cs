using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Inlays
{
  public partial class ConvoInlay : UserControl, INotifyPropertyChanged
  {
    private App _app;
    private Pages.MainPage _mainPage;
    private DispatcherTimer _updateTimer = new DispatcherTimer();
    private ATProto.Lexicons.Chat.BSky.Convo.Defs.ConvoView _convoInfo;

    public ConvoInlay()
    {
      InitializeComponent();
      _app = (App)Application.Current;
      Loaded += ChatInlay_Loaded;
      DataContext = this;

      _updateTimer.Interval = TimeSpan.FromSeconds(10);
      _updateTimer.Tick += async (s, e) => { await Update(); };
    }

    public string ID { get; set; }
    public ObservableCollection<Message> Messages { get; private set; } = new ObservableCollection<Message>();
    public string MessageText { get; set; }
    public string ChatName { get; set; }

    private void ChatInlay_Loaded(object sender, RoutedEventArgs e)
    {
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
    }

    public void Flush()
    {
      ChatName = string.Empty;
      Messages.Clear();
      _updateTimer.Stop();
    }

    public async Task Refresh()
    {
      _mainPage?.StartLoading();
      await Update();

      var responseConvo = await _mainPage.Get<ATProto.Lexicons.Chat.BSky.Convo.GetConvoResponse>(new ATProto.Lexicons.Chat.BSky.Convo.GetConvo()
      {
        convoId = ID
      });
      if (responseConvo != null)
      {
        _convoInfo = responseConvo.convo;
        ChatName = string.Join(", ", _convoInfo.members.Select(m => m.DisplayName));
        OnPropertyChanged(nameof(ChatName));
      }

      _mainPage?.EndLoading();

      _updateTimer.Start();
    }

    public async Task Update()
    {
      if (string.IsNullOrEmpty(ID))
      {
        return;
      }

      var responseMessages = await _mainPage.Get<ATProto.Lexicons.Chat.BSky.Convo.GetMessagesResponse>(new ATProto.Lexicons.Chat.BSky.Convo.GetMessages()
      {
        convoId = ID
      });

      if (responseMessages != null)
      {
        AddNewMessages(responseMessages.messages);
      }
    }

    private void AddNewMessages(IEnumerable<object> messageBurst)
    {
      var newMessages = messageBurst.Select(s => s as ATProto.Lexicons.Chat.BSky.Convo.Defs.MessageView).Where(s => s != null);
      if (!newMessages.Any())
      {
        return;
      }

      foreach (var message in newMessages)
      {
        var insertionItem = Messages.FirstOrDefault(s => s.Timestamp > message?.sentAt);
        var idx = Messages.IndexOf(insertionItem);
        Messages.Insert(idx < 0 ? Messages.Count : idx, new Message()
        {
          ID = message?.id,
          SenderAvatarURL = _convoInfo.members.FirstOrDefault(s=>s.did == message?.sender?.did)?.avatar,
          SenderName = _convoInfo.members.FirstOrDefault(s => s.did == message?.sender?.did)?.DisplayName,
          Text = message?.text,
          Timestamp = message?.sentAt
        });
      }

      OnPropertyChanged(nameof(Messages));
      listView.UpdateLayout();
      listView.ScrollIntoView(Messages.LastOrDefault());
    }

    private async void Send_Click(object sender, RoutedEventArgs e)
    {
      var response = await _mainPage.Post<ATProto.Lexicons.Chat.BSky.Convo.SendMessageResponse>(new ATProto.Lexicons.Chat.BSky.Convo.SendMessage
      {
        convoId = ID,
        message = new ATProto.Lexicons.Chat.BSky.Convo.Defs.MessageInput()
        {
          text = MessageText,
          facets = await ATProto.Helpers.ParseTextForFacets(_app.Client, MessageText),
          embed = null
        }
      });

      if (response != null)
      {
        MessageText = string.Empty;
        AddNewMessages(new List<object>() { response });
      }
    }

    public class Message
    {
      public string ID { get; set; }
      public string SenderName { get; set; }
      public string SenderAvatarURL { get; set; }
      public string TimestampString => Timestamp.HasValue ? (Timestamp.Value.Date == DateTime.Now.Date ? Timestamp.Value.ToString("HH:mm") : Timestamp.Value.ToString("yyyy-MM-dd HH:mm")) : string.Empty;
      public DateTime? Timestamp { get; set; }
      public string Text { get; set; }
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
