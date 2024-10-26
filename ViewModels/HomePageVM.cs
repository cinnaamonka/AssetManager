using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;
using AssetManager.Views;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
    internal class HomePageVM : ObservableObject
    {
        public MainPageVM MainPageVM { get; }

        public HomePageVM()
        {
            int a = 0;
        }

        public HomePageVM(MainPageVM mainPageVM)
        {
            MainPageVM = mainPageVM;
        }
    }
}
