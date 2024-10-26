using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;
using AssetManager.Views;
using AssetManager.Models;

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

        public void HandleOpenMetadataPage(Asset asset)
        {
            if (CurrentPage.DataContext is OverviewPageVM overviewVM)
            {
                var selectedAsset = overviewVM.SelectedAsset;
                if (selectedAsset != null)
                {
                    CurrentPage = MetadataPage;
                    if (MetadataPage.DataContext is MetadataPageVM metadataVM)
                    {
                        metadataVM.SetMetadata(selectedAsset.Metadata); 
                        OnPropertyChanged(nameof(CurrentPage));
                    }
                }
            }
        }

        public void OpenOverViewPage()
        {
            CurrentPage = MainPage;
            OnPropertyChanged(nameof(CurrentPage));
        }
    }
}
