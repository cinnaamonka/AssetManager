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

        public RelayCommand OpenOverviewPageCommand { get; set; }

        public HomePageVM()
        {
            
        }

        public HomePageVM(MainPageVM mainPageVM)
        {
            MainPageVM = mainPageVM;
            OpenOverviewPageCommand = new RelayCommand(OpenOverviewPage); 
        }

        private void OpenOverviewPage()
        {
            MainPageVM.OpenOverViewPage();
        }
    }
}
