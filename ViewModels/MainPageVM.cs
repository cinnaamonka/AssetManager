using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;
using AssetManager.Views; 

namespace AssetManager.ViewModels
{
    public class MainPageVM : ObservableObject
    {
        public OverviewPage MainPage { get; set; } 
        public MetadataPage MetadataPage { get; set; } 
        public Page CurrentPage { get; set; }

        public MainPageVM()
        {
            OverviewPageVM overViewPageVM = new(this);
            MetadataPageVM metadataPageVM = new(this, overViewPageVM);  

            MainPage = new OverviewPage { DataContext = overViewPageVM };
            MetadataPage = new MetadataPage { DataContext = metadataPageVM };

            CurrentPage = MainPage;
        }

        public void HandleOpenDetailsPage()
        {
            //if (CurrentPage.DataContext is OverviewPageVM overviewVM)
            //{
            //    var selectedSuperhero = overviewVM.SelectedSuperhero;
            //    if (selectedSuperhero != null)
            //    {
            //        CurrentPage = StatsPage;
            //        if (StatsPage.DataContext is MetadataPageVM detailsVM)
            //        {
            //            detailsVM.SetSuperhero(selectedSuperhero);
            //            OnPropertyChanged(nameof(CurrentPage));
            //        }
            //    }
            //}
        }

        public void OpenOverViewPage()
        {
            CurrentPage = MainPage;
            OnPropertyChanged(nameof(CurrentPage));
        }
    }
}
