using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Inlays
{
  public partial class SearchInlay : UserControl, INotifyPropertyChanged
  {
    private App _app;
    protected Pages.MainPage _mainPage;
    public SearchInlay()
    {
      InitializeComponent();
      _app = (App)Application.Current;
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
      Loaded += SearchInlay_Loaded;
      DataContext = this;
    }

    public string SearchText { get; set; }

    protected async void SearchInlay_Loaded(object sender, RoutedEventArgs e)
    {
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
    }

    private async void Search_Click(object sender, RoutedEventArgs e)
    {
      await Refresh();
    }

    private async void SearchTypes_PivotItemLoading(Pivot sender, PivotItemEventArgs args)
    {
      await Refresh();
    }

    private async Task Refresh()
    {
      if (string.IsNullOrEmpty(SearchText))
      {
        return;
      }

      var pivotItem = SearchTypes.SelectedItem as PivotItem;
      if (pivotItem == null)
      {
        return;
      }
      var postList = pivotItem.ContentTemplateRoot as Controls.PostList.PostListSearch;
      if (postList != null)
      {
        postList.SearchTerm = SearchText;
        await postList.Refresh();
        return;
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
