using System;
using System.ComponentModel;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Inlays
{
  public partial class PostInlay : UserControl, INotifyPropertyChanged
  {
    private App _app;
    public string _postText;
    public PostInlay()
    {
      InitializeComponent();
      _app = (App)Application.Current;
      DataContext = this;
    }

    public string PostText
    {
      get => _postText?.Replace("\r","\n");
      set
      {
        _postText = value;
        OnPropertyChanged("PostText");
        OnPropertyChanged("PostLengthText");
      }
    }
    public string PostLengthText => $"{PostLengthInGraphemes} / {MaxLengthInGraphemes}";
    public int PostLengthInGraphemes { get { return new StringInfo(PostText ?? string.Empty).LengthInTextElements; } }
    public int MaxLengthInGraphemes { get { return 300; } }

    private void Send_Click(object sender, RoutedEventArgs e)
    {
      var response = _app.Client.PostAsync<ATProto.Lexicons.COM.ATProto.Repo.CreateRecordResponse>(new ATProto.Lexicons.COM.ATProto.Repo.CreateRecord() {
        repo = _app.Client.DID,
        collection = "app.bsky.feed.post",
        record = new ATProto.Lexicons.App.BSky.Feed.Post()
        {
          text = PostText,
          createdAt = DateTime.Now
        }
      });
      if (response != null)
      {
        
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises this object's PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The property that has a new value.</param>
    public virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
