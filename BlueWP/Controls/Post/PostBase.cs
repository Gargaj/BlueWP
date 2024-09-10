using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;

namespace BlueWP.Controls.Post
{
  public class PostBase : UserControl, INotifyPropertyChanged
  {
    protected App _app;
    protected Pages.MainPage _mainPage;
    protected Dictionary<Hyperlink, string> _atURIs = new Dictionary<Hyperlink, string>();
    public PostBase()
    {
      _app = (App)Application.Current;
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
      Loaded += PostBase_Loaded;
      LayoutUpdated += PostBase_LayoutUpdated;
    }

    protected void PostBase_Loaded(object sender, RoutedEventArgs e)
    {
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
    }

    public bool IsRepost => FeedViewPost?.IsRepost ?? false;
    public bool IsDeleted { get; set; } = false;
    public bool IsReply => FeedViewPost?.IsReply ?? false;
    public bool HasQuotedPost => PostData?.HasQuotedPost ?? false;
    public bool HasEmbedExternal => PostData?.HasEmbedExternal ?? false;
    public bool HasVideo => PostData?.HasVideo ?? false;

    public string PostAuthorAvatarURL => PostData?.PostAuthorAvatarURL;
    public string PostAuthorDisplayName => PostData?.PostAuthorDisplayName;
    public string PostAuthorHandle => PostData?.PostAuthorHandle;
    public string PostElapsedTime => PostData?.PostElapsedTime;
    public string PostText => PostData?.PostText;
    public string PostDateTime => PostView?.PostDateTime;

    public uint ReplyCount => PostView?.ReplyCount ?? 0;
    public uint RepostCount => PostView?.RepostCount ?? 0;
    public uint LikeCount => PostView?.LikeCount ?? 0;

    public bool PostReposted => PostView?.PostReposted ?? false;
    public bool PostLiked => PostView?.PostLiked ?? false;
    public bool PostMine => PostView.author.did == _app.Client.DID;

    public IEnumerable<ATProto.Lexicons.App.BSky.Embed.Images.ViewImage> PostImages => PostView?.PostImages;
    public ATProto.Lexicons.App.BSky.Embed.External.View PostEmbedExternal => PostView?.PostEmbedExternal;
    public ATProto.Lexicons.App.BSky.Embed.Record.ViewRecord QuotedPost => PostView?.QuotedPost;
    public ATProto.Lexicons.App.BSky.Embed.Video.View PostVideo => PostView?.PostVideo;

    public string PostReason => FeedViewPost?.PostReason;
    public string PostReplyTo => FeedViewPost?.PostReplyTo;
    public int VideoHeight
    {
      get
      {
        if (PostVideo == null)
        {
          return 100;
        }
        int height = (int)(ActualWidth * PostVideo.aspectRatio.height / PostVideo.aspectRatio.width);
        height = Math.Min(height, 400);
        return height;
      }
    }

    public ATProto.IPost PostData
    {
      get { return GetValue(PostDataProperty) as ATProto.IPost; }
      set { SetValue(PostDataProperty, value); }
    }
    public static readonly DependencyProperty PostDataProperty = DependencyProperty.Register("PostData", typeof(ATProto.IPost), typeof(PostBase), new PropertyMetadata(null, OnPostDataChanged));

    public ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost FeedViewPost => PostData as ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost;
    public ATProto.Lexicons.App.BSky.Feed.Defs.PostView PostView
    {
      get
      {
        var feedViewPost = PostData as ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost;
        if (feedViewPost != null)
        {
          return feedViewPost?.post;
        }

        return PostData as ATProto.Lexicons.App.BSky.Feed.Defs.PostView;
      }
    }

    protected static void OnPostDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var post = d as PostBase;
      if (post != null)
      {
        post.OnPropertyChanged(nameof(IsRepost));
        post.OnPropertyChanged(nameof(IsReply));
        post.OnPropertyChanged(nameof(HasQuotedPost));
        post.OnPropertyChanged(nameof(HasEmbedExternal));
        post.OnPropertyChanged(nameof(PostText));

        post.OnPropertyChanged(nameof(HasVideo));

        post.OnPropertyChanged(nameof(PostAuthorAvatarURL));
        post.OnPropertyChanged(nameof(PostAuthorDisplayName));
        post.OnPropertyChanged(nameof(PostAuthorHandle));
        post.OnPropertyChanged(nameof(PostElapsedTime));
        post.OnPropertyChanged(nameof(PostText));
        post.OnPropertyChanged(nameof(PostDateTime));

        post.OnPropertyChanged(nameof(ReplyCount));
        post.OnPropertyChanged(nameof(RepostCount));
        post.OnPropertyChanged(nameof(LikeCount));

        post.OnPropertyChanged(nameof(PostReposted));
        post.OnPropertyChanged(nameof(PostLiked));

        post.OnPropertyChanged(nameof(PostImages));
        post.OnPropertyChanged(nameof(PostEmbedExternal));
        post.OnPropertyChanged(nameof(QuotedPost));

        post.OnPropertyChanged(nameof(PostReason));
        post.OnPropertyChanged(nameof(PostReplyTo));

        post.UpdateText();
      }
    }

    protected void PostBase_LayoutUpdated(object sender, object e)
    {
      OnPropertyChanged(nameof(VideoHeight));
    }

    protected virtual void UpdateText()
    {
    }

    protected void ViewProfile_Click(object sender, RoutedEventArgs e)
    {
      _mainPage.SwitchToProfileInlay(PostView?.author?.did);
    }

    protected async void ViewThread_Click(object sender, RoutedEventArgs e)
    {
      await _mainPage.SwitchToThreadViewInlay(PostView?.uri);
    }

    protected void Reply_Click(object sender, RoutedEventArgs e)
    {
      _mainPage.Reply(PostView);
    }

    protected void RepostMenu_Click(object sender, RoutedEventArgs e)
    {
      var element = sender as FrameworkElement;
      if (element != null)
      {
        FlyoutBase.ShowAttachedFlyout(element);
      }
    }

    protected void MiscMenu_Click(object sender, RoutedEventArgs e)
    {
      var element = sender as FrameworkElement;
      if (element != null)
      {
        FlyoutBase.ShowAttachedFlyout(element);
      }
    }

    protected async void Repost_Click(object sender, RoutedEventArgs e)
    {
      if (!PostReposted)
      {
        // add repost
        var response = await _mainPage.Post<ATProto.Lexicons.COM.ATProto.Repo.CreateRecordResponse>(new ATProto.Lexicons.COM.ATProto.Repo.CreateRecord()
        {
          repo = _app.Client.DID,
          collection = "app.bsky.feed.repost",
          record = new ATProto.Lexicons.App.BSky.Feed.Repost()
          {
            createdAt = DateTime.Now,
            subject = new ATProto.Lexicons.COM.ATProto.Repo.StrongRef()
            {
              cid = PostView.cid,
              uri = PostView.uri,
            }
          }
        });
        PostView.repostCount++;
        OnPropertyChanged(nameof(RepostCount));
        PostView.viewer.repost = response.uri;
        OnPropertyChanged(nameof(PostReposted));
      }
      else
      {
        // remove repost
        string repo = string.Empty;
        string collection = string.Empty;
        string rkey = string.Empty;
        if (ATProto.Helpers.ParseATURI(PostView?.viewer?.repost, ref repo, ref collection, ref rkey))
        {
          var response = await _mainPage.Post<ATProto.Lexicons.COM.ATProto.Repo.DeleteRecordResponse>(new ATProto.Lexicons.COM.ATProto.Repo.DeleteRecord()
          {
            repo = repo,
            collection = collection,
            rkey = rkey
          });
          PostView.repostCount--;
          OnPropertyChanged(nameof(RepostCount));
          PostView.viewer.repost = null;
          OnPropertyChanged(nameof(PostReposted));
        }
      }
    }

    protected async void Like_Click(object sender, RoutedEventArgs e)
    {
      if (!PostLiked)
      {
        // add like
        var response = await _mainPage.Post<ATProto.Lexicons.COM.ATProto.Repo.CreateRecordResponse>(new ATProto.Lexicons.COM.ATProto.Repo.CreateRecord()
        {
          repo = _app.Client.DID,
          collection = "app.bsky.feed.like",
          record = new ATProto.Lexicons.App.BSky.Feed.Like()
          {
            createdAt = DateTime.Now,
            subject = new ATProto.Lexicons.COM.ATProto.Repo.StrongRef()
            {
              cid = PostView.cid,
              uri = PostView.uri,
            }
          }
        });
        PostView.likeCount++;
        OnPropertyChanged(nameof(LikeCount));
        PostView.viewer.like = response.uri;
        OnPropertyChanged(nameof(PostLiked));
      }
      else
      {
        // remove like
        string repo = string.Empty;
        string collection = string.Empty;
        string rkey = string.Empty;
        if (ATProto.Helpers.ParseATURI(PostView?.viewer?.like, ref repo, ref collection, ref rkey))
        {
          var response = await _mainPage.Post<ATProto.Lexicons.COM.ATProto.Repo.DeleteRecordResponse>(new ATProto.Lexicons.COM.ATProto.Repo.DeleteRecord()
          {
            repo = repo,
            collection = collection,
            rkey = rkey
          });
          PostView.likeCount--;
          OnPropertyChanged(nameof(LikeCount));
          PostView.viewer.like = null;
          OnPropertyChanged(nameof(PostLiked));
        }
      }
    }

    protected void Quote_Click(object sender, RoutedEventArgs e)
    {
      _mainPage.Quote(PostView);
    }

    protected void MiscCopy_Click(object sender, RoutedEventArgs e)
    {
      string repo = string.Empty;
      string collection = string.Empty;
      string rkey = string.Empty;
      if (ATProto.Helpers.ParseATURI(PostView.uri, ref repo, ref collection, ref rkey))
      {
        var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
        dataPackage.RequestedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
        dataPackage.SetText($"https://bsky.app/profile/{PostView?.author?.handle}/post/{rkey}");
        Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
      }
    }

    protected async void MiscDelete_Click(object sender, RoutedEventArgs e)
    {
      string repo = string.Empty;
      string collection = string.Empty;
      string rkey = string.Empty;
      if (ATProto.Helpers.ParseATURI(PostView.uri, ref repo, ref collection, ref rkey))
      {
        var response = await _mainPage.Post<ATProto.Lexicons.COM.ATProto.Repo.DeleteRecordResponse>(new ATProto.Lexicons.COM.ATProto.Repo.DeleteRecord()
        {
          repo = repo,
          collection = collection,
          rkey = rkey
        });
        IsDeleted = true;
        OnPropertyChanged(nameof(IsDeleted));
      }
    }

    protected List<Inline> GenerateInlines()
    {
      _atURIs.Clear();
      var result = new List<Inline>();
      var text = PostText;
      var facets = (PostView?.record as ATProto.Lexicons.App.BSky.Feed.Post)?.facets;
      if (facets == null || facets.Count == 0)
      {
        result.Add(new Run() { Text = text ?? string.Empty });
        return result;
      }

      var bytes = System.Text.Encoding.UTF8.GetBytes(text);
      var indices = facets.Select(s => s.index).OrderBy(s=>s.byteStart);
      var current = 0U;
      foreach (var index in indices)
      {
        if (current < index.byteStart)
        {
          var textFragment = System.Text.Encoding.UTF8.GetString(bytes, (int)current, (int)(index.byteStart - current));
          result.Add(new Run() { Text = textFragment });
        }

        var facet = facets.FirstOrDefault(s => s.index.byteStart == index.byteStart);
        if (facet == null || facet.features == null || facet.features.Count == 0)
        {
          continue;
        }

        var linkText = System.Text.Encoding.UTF8.GetString(bytes, (int)index.byteStart, (int)(index.byteEnd - index.byteStart));
        if (facet.features[0] as ATProto.Lexicons.App.BSky.RichText.Facet.Link != null)
        {
          var link = facet.features[0] as ATProto.Lexicons.App.BSky.RichText.Facet.Link;
          var hyperlink = new Hyperlink();
          hyperlink.NavigateUri = new Uri(link.uri);
          hyperlink.Inlines.Add(new Run() { Text = linkText });
          result.Add(hyperlink);
        }
        else if (facet.features[0] as ATProto.Lexicons.App.BSky.RichText.Facet.Mention != null)
        {
          var mention = facet.features[0] as ATProto.Lexicons.App.BSky.RichText.Facet.Mention;
          var hyperlink = new Hyperlink();
          _atURIs.Add(hyperlink, mention.did);
          hyperlink.Inlines.Add(new Run() { Text = linkText });
          hyperlink.Click += Hyperlink_Click;
          result.Add(hyperlink);
        }
        else
        {
          result.Add(new Run() { Text = linkText });
        }

        current = index.byteEnd;
      }
      if (current < (uint)bytes.Length)
      {
        var textFragment = System.Text.Encoding.UTF8.GetString(bytes, (int)current, bytes.Length - (int)current);
        result.Add(new Run() { Text = textFragment });
      }

      return result;
    }

    protected void Hyperlink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
    {
      if (!_atURIs.ContainsKey(sender))
      {
        return;
      }
      _mainPage.SwitchToProfileInlay(_atURIs[sender]);
    }

    protected void MediaPlayerElement_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
      if (!HasVideo)
      {
        return;
      }

      var mp = sender as MediaPlayerElement;
      if (mp == null)
      {
        return;
      }
      var video = PostView.embed as ATProto.Lexicons.App.BSky.Embed.Video.View;
      if (video == null)
      {
        return;
      }
      var mediaPlayer = new Windows.Media.Playback.MediaPlayer();
      mp.SetMediaPlayer(mediaPlayer);

      mediaPlayer.Source = Windows.Media.Core.MediaSource.CreateFromUri(new Uri(video.playlist));
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
