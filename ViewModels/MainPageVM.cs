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
        public MetadataWindow MetadataWindow { get; set; }
        public HomePage HomePage { get; }
        public Page CurrentPage { get; set; }

        private AppDbContext _appDbContext;
        public AppDbContext AppDbContext { get; set; }

        private MetadataWindowVM metadataWindowVM { get; }

        public MainPageVM()
        {
            AppDbContext = new AppDbContext();

            OverviewPageVM overViewPageVM = new(this);
       
            HomePageVM homePageVM = new(this,overViewPageVM);
            metadataWindowVM = new(this, overViewPageVM);
            MainPage = new OverviewPage { DataContext = overViewPageVM };
            HomePage = new HomePage { DataContext = homePageVM };

            CurrentPage = HomePage;

           
        }


        public void HandleOpenPopUpWindow(Asset selectedAsset) 
        {
            if (selectedAsset != null)
            {
              
                metadataWindowVM.SelectedMetadata = selectedAsset.Metadata;


                MetadataWindow = new MetadataWindow
                {
                    DataContext = metadataWindowVM,
                };

                MetadataWindow.ShowDialog(); 
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
