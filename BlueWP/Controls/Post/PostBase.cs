using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace BlueWP.Controls.Post
{
  public class PostBase : UserControl, INotifyPropertyChanged
  {
    protected App _app;
    protected Pages.MainPage _mainPage;
    public PostBase()
    {
      _app = (App)Application.Current;
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
      Loaded += PostBase_Loaded;
    }

    protected void PostBase_Loaded(object sender, RoutedEventArgs e)
    {
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
    }

    public bool IsRepost => FeedViewPost?.IsRepost ?? false;
    public bool IsReply => FeedViewPost?.IsReply ?? false;
    public bool HasQuotedPost => PostData?.HasQuotedPost ?? false;
    public bool HasEmbedExternal => PostData?.HasEmbedExternal ?? false;

    public string PostAuthorAvatarURL => PostView?.PostAuthorAvatarURL;
    public string PostAuthorDisplayName => PostView?.PostAuthorDisplayName;
    public string PostAuthorHandle => PostView?.PostAuthorHandle;
    public string PostElapsedTime => PostView?.PostElapsedTime;
    public string PostText => PostView?.PostText;
    public string PostDateTime => PostView?.PostDateTime;

    public uint ReplyCount => PostView?.ReplyCount ?? 0;
    public uint RepostCount => PostView?.RepostCount ?? 0;
    public uint LikeCount => PostView?.LikeCount ?? 0;

    public bool PostReposted => PostView?.PostReposted ?? false;
    public bool PostLiked => PostView?.PostLiked ?? false;

    public IEnumerable<ATProto.Lexicons.App.BSky.Embed.Images.ViewImage> PostImages => PostView?.PostImages;
    public ATProto.Lexicons.App.BSky.Embed.External.View PostEmbedExternal => PostView?.PostEmbedExternal;
    public ATProto.Lexicons.App.BSky.Embed.Record.ViewRecord QuotedPost => PostView?.QuotedPost;

    public string PostReason => FeedViewPost?.PostReason;
    public string PostReplyTo => FeedViewPost?.PostReplyTo;

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
      }
    }

    protected void ViewProfile_Click(object sender, RoutedEventArgs e)
    {
      var postData = PostData as ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost;
      _mainPage.SwitchToProfileInlay(postData?.post?.author?.did);
    }

    protected void ViewThread_Click(object sender, RoutedEventArgs e)
    {
      var feedViewPost = PostData as ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost;
      if (feedViewPost != null)
      {
        _mainPage.SwitchToThreadViewInlay(feedViewPost?.post?.uri);
        return;
      }
      var postView = PostData as ATProto.Lexicons.App.BSky.Feed.Defs.PostView;
      if (postView != null)
      {
        _mainPage.SwitchToThreadViewInlay(postView?.uri);
        return;
      }
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

    protected async void Repost_Click(object sender, RoutedEventArgs e)
    {
      if (!PostReposted)
      {
        // add repost
        var response = await _app.Client.PostAsync<ATProto.Lexicons.COM.ATProto.Repo.CreateRecordResponse>(new ATProto.Lexicons.COM.ATProto.Repo.CreateRecord()
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
          var response = await _app.Client.PostAsync<ATProto.Lexicons.COM.ATProto.Repo.DeleteRecordResponse>(new ATProto.Lexicons.COM.ATProto.Repo.DeleteRecord()
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
        var response = await _app.Client.PostAsync<ATProto.Lexicons.COM.ATProto.Repo.CreateRecordResponse>(new ATProto.Lexicons.COM.ATProto.Repo.CreateRecord()
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
          var response = await _app.Client.PostAsync<ATProto.Lexicons.COM.ATProto.Repo.DeleteRecordResponse>(new ATProto.Lexicons.COM.ATProto.Repo.DeleteRecord()
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
