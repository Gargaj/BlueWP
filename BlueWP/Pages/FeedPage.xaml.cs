using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Pages
{
    public partial class FeedPage : Page, INotifyPropertyChanged
    {
        private App _app;
        private List<ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost> _feedItems = new List<ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost>();
        public FeedPage()
        {
            this.InitializeComponent();
            _app = (App)Windows.UI.Xaml.Application.Current;
            DataContext = this;

            RefreshFeed();
        }

        private async void RefreshFeed()
        {
            var response = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Feed.GetTimelineResponse>(new ATProto.Lexicons.App.BSky.Feed.GetTimeline()
            {
                limit = 60
            });

            _feedItems = response?.feed;
            OnPropertyChanged(nameof(FeedItems));
        }

        private void Logout_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _app.Logout();
        }

        public List<ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost> FeedItems { get { return _feedItems; } }

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
