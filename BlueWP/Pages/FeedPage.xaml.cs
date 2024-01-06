using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Pages
{
    public partial class FeedPage : Page, INotifyPropertyChanged
    {
        private App _app;
        private List<ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost> _feedItems = new List<ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost>();
        private bool _isLoading = false;
        private bool _hasError = false;
        private string _errorText = string.Empty;

        public FeedPage()
        {
            this.InitializeComponent();
            _app = (App)Windows.UI.Xaml.Application.Current;
            DataContext = this;

            RefreshFeed();
        }

        private async void RefreshFeed()
        {
            IsLoading = true;

            try
            {
                var response = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Feed.GetTimelineResponse>(new ATProto.Lexicons.App.BSky.Feed.GetTimeline()
                {
                    limit = 60
                });
                _feedItems = response?.feed;
                OnPropertyChanged(nameof(FeedItems));
            }
            catch (WebException ex)
            {
                HasError = true;
                var webResponse = ex.Response as HttpWebResponse;
                var error = ex.Response != null ? new StreamReader(ex.Response.GetResponseStream()).ReadToEnd() : ex.ToString();
                ErrorText = $"HTTP ERROR {(int)webResponse.StatusCode}\n\n{error}";
            }

            IsLoading = false;
        }

        private void Logout_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _app.Logout();
        }

        public List<ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost> FeedItems { get { return _feedItems; } }
        public bool IsLoading { get { return _isLoading; } set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); } }
        public bool HasError { get { return _hasError; } set { _hasError = value; OnPropertyChanged(nameof(HasError)); } }
        public string ErrorText { get { return _errorText; } set { _errorText = value; OnPropertyChanged(nameof(ErrorText)); } }

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
