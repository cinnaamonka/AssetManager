using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;
using AssetManager.Views;
using AssetManager.Models;
using System.IO;

namespace AssetManager.ViewModels
{
    public class MainPageVM : ObservableObject
    {
        public OverviewPage MainPage { get; set; }
       // public MetadataPage MetadataPage { get; set; }
        public HomePage HomePage { get; }
        public Page CurrentPage { get; set; }

       

        public MainPageVM()
        {
            OverviewPageVM overViewPageVM = new(this);
            //MetadataWindowVM metadataPageVM = new(this, overViewPageVM);
            HomePageVM homePageVM = new(this,overViewPageVM);

            MainPage = new OverviewPage { DataContext = overViewPageVM };
            HomePage = new HomePage { DataContext = homePageVM };

            CurrentPage = MainPage;
        }

        public void HandleOpenMetadataPage(Asset asset)
        {
            //if (CurrentPage.DataContext is OverviewPageVM overviewVM)
            //{
            //    var selectedAsset = overviewVM.SelectedAsset;
            //    if (selectedAsset != null)
            //    {
            //        CurrentPage = MetadataPopUpWindow; 
            //        if (MetadataWindowVM.DataContext is MetadataWindowVM metadataVM)
            //        {
            //            metadataVM.SetMetadata(selectedAsset.Metadata); 
            //            OnPropertyChanged(nameof(CurrentPage));
            //        }
            //    }
            //}
        }

        public void HandleOpenPopUpWindow(Asset selectedAsset) 
        {
            if (selectedAsset != null)
            {
                var metadataPageVM = new MetadataWindowVM
                {
                    SelectedMetadata = selectedAsset.Metadata 
                };

                var metadataWindow = new MetadataWindow
                {
                    DataContext = metadataPageVM,
                };

                metadataWindow.ShowDialog();
            }
        }
        public void OpenOverViewPage()
        {
            CurrentPage = MainPage;
            OnPropertyChanged(nameof(CurrentPage));
        }

        public void OpenHomePage()
        {
            CurrentPage = HomePage;
            OnPropertyChanged(nameof(CurrentPage));
        }

      
    }
}
