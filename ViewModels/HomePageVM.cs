using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;
using AssetManager.Views;
using AssetManager.Models;
using CommunityToolkit.Mvvm.Input;

namespace AssetManager.ViewModels
{
    internal class HomePageVM : ObservableObject
    {
        public MainPageVM MainPageVM { get; }
        public OverviewPageVM OverViewPageVM { get; }

        public RelayCommand OpenOverviewPageCommand { get; set; }
        public RelayCommand BrowseProjectFiles { get; set; }


        public HomePageVM()
        {
            
        }

        public HomePageVM(MainPageVM mainPageVM,OverviewPageVM OverviewPageViewModel)
        {
            MainPageVM = mainPageVM;
            OverViewPageVM = OverviewPageViewModel; 
            OpenOverviewPageCommand = new RelayCommand(OpenOverviewPage);
            BrowseProjectFiles = new RelayCommand(BrowseFiles);
        }

        private void OpenOverviewPage()
        {
            MainPageVM.OpenOverViewPage();
        }
        private void BrowseFiles()
        {
            OverViewPageVM.OpenFolderDialog();
        }
    }
}
