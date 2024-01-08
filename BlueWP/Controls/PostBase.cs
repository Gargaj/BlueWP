using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Controls
{
  public class PostBase : UserControl, INotifyPropertyChanged
  {
    public PostBase()
    {
    }

    public bool IsRepost
    {
      get { return PostData?.IsRepost ?? false; }
    }

    public bool IsReply
    {
      get { return PostData?.IsReply ?? false; }
    }

    public bool HasQuotedPost
    {
      get { return PostData?.HasQuotedPost ?? false; }
    }

    public ATProto.IPost PostData
    {
      get { return GetValue(PostDataProperty) as ATProto.IPost; }
      set { SetValue(PostDataProperty, value); }
    }
    public static readonly DependencyProperty PostDataProperty = DependencyProperty.Register("PostData", typeof(ATProto.IPost), typeof(PostBase), new PropertyMetadata(null, OnPostDataChanged));

    protected static void OnPostDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var post = d as PostBase;
      if (post != null)
      {
        post.OnPropertyChanged(nameof(IsRepost));
        post.OnPropertyChanged(nameof(IsReply));
        post.OnPropertyChanged(nameof(HasQuotedPost));
      }
    }

    public async void OpenExternalURL_Click(object sender, RoutedEventArgs e)
    {
      var button = e.OriginalSource as Button;
      if (button == null)
      {
        return;
      }
      var dataContext = button.DataContext as PostBase;
      if (dataContext == null)
      {
        return;
      }
/*
      if (!string.IsNullOrEmpty(dataContext.PostData.PostEmbedExternalURL))
      {
        await Windows.System.Launcher.LaunchUriAsync(new Uri(dataContext.PostData.PostEmbedExternalURL));
      }
*/
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
