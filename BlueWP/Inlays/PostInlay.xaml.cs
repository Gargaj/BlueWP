using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

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

      ImageAttachments = new ObservableCollection<ImageAttachment>();
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
    public ObservableCollection<ImageAttachment> ImageAttachments { get; set; }

    private async void Attach_Click(object sender, RoutedEventArgs e)
    {
      var picker = new Windows.Storage.Pickers.FileOpenPicker
      {
        SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
      };
      picker.FileTypeFilter.Add(".gif");
      picker.FileTypeFilter.Add(".jpg");
      picker.FileTypeFilter.Add(".jpeg");
      picker.FileTypeFilter.Add(".png");

      var file = await picker.PickSingleFileAsync();
      if (file != null)
      {
        using (var fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
        {
          var bitmapImage = new BitmapImage();
          await bitmapImage.SetSourceAsync(fileStream);
          ImageAttachments.Add(new ImageAttachment() {
            File = file,
            BitmapImage = bitmapImage
          });
        }
      }
      OnPropertyChanged(nameof(ImageAttachments));
    }

    private async void Send_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        var post = new ATProto.Lexicons.App.BSky.Feed.Post()
        {
          text = PostText,
          createdAt = DateTime.Now
        };
        if (ImageAttachments != null && ImageAttachments.Count > 0)
        {
          foreach (var imageAttachment in ImageAttachments)
          {
            imageAttachment.IsLoading = true;
            imageAttachment.OnPropertyChanged("IsLoading");
          }
          var images = new ATProto.Lexicons.App.BSky.Embed.Images()
          {
            images = new List<ATProto.Lexicons.App.BSky.Embed.Images.Image>()
          };
          foreach (var imageAttachment in ImageAttachments)
          {
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(imageAttachment.File);
            byte[] byteData = new byte[buffer.Length];
            using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
            {
              dataReader.ReadBytes(byteData);
            }
            var blobResponse = await _app.Client.PostAsync<ATProto.Lexicons.COM.ATProto.Repo.UploadBlobResponse>(new ATProto.Lexicons.COM.ATProto.Repo.UploadBlob()
            {
              MimeType = imageAttachment.File.ContentType,
              PostData = byteData,
            });

            var image = new ATProto.Lexicons.App.BSky.Embed.Images.Image()
            {
              image = blobResponse.blob,
              alt = imageAttachment.AltText
            };
            images.images.Add(image);
            imageAttachment.IsLoading = false;
            imageAttachment.OnPropertyChanged("IsLoading");
          }
          post.embed = images;
        }
        var response = await _app.Client.PostAsync<ATProto.Lexicons.COM.ATProto.Repo.CreateRecordResponse>(new ATProto.Lexicons.COM.ATProto.Repo.CreateRecord()
        {
          repo = _app.Client.DID,
          collection = "app.bsky.feed.post",
          record = post
        });
        if (response != null)
        {
          ImageAttachments.Clear();
          OnPropertyChanged(nameof(ImageAttachments));
        }
      }
      catch (WebException ex)
      {
        var webResponse = ex.Response as HttpWebResponse;
        var dialog = new ContentDialog
        {
          Content = new TextBlock { Text = ex.Message, TextWrapping = TextWrapping.WrapWholeWords },
          Title = $"HTTP ERROR {(int)webResponse.StatusCode}",
          IsSecondaryButtonEnabled = false,
          PrimaryButtonText = "Ok"
        };
        await dialog.ShowAsync();
      }
    }

    private void RemoveImage_Click(object sender, RoutedEventArgs e)
    {
      var b = sender as Button;
      var i = b?.DataContext as ImageAttachment;
      ImageAttachments.Remove(i);
      OnPropertyChanged(nameof(ImageAttachments));
    }

    private async void EditAltText_Click(object sender, RoutedEventArgs e)
    {
      var b = sender as Button;
      var i = b?.DataContext as ImageAttachment;

      var textBox = new TextBox()
      {
        AcceptsReturn = false,
        Text = i.AltText,
        VerticalAlignment = VerticalAlignment.Bottom
      };
      var dialog = new ContentDialog()
      {
        Content = textBox,
        Title = "Edit ALT text",
        IsSecondaryButtonEnabled = true,
        PrimaryButtonText = "Ok",
        SecondaryButtonText = "Cancel"
      };
      if (await dialog.ShowAsync() == ContentDialogResult.Primary)
      {
        i.AltText = textBox.Text;
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

    public class ImageAttachment : INotifyPropertyChanged
    {
      public Windows.Storage.StorageFile File { get; set; }
      public BitmapImage BitmapImage { get; set; }
      public object Blob { get; set; }
      public string AltText { get; set; } = string.Empty;
      public bool IsLoading { get; set; } = false;

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
}
