using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;
using AssetManager.Views;
using AssetManager.Models;
using System.IO;
using AssetManager.Repositories;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace AssetManager.ViewModels
{
    public class MainPageVM : ObservableObject
    {
        public OverviewPage MainPage { get; set; }
        public MetadataWindow MetadataWindow { get; set; }
        public PerforceWindow PerforceWindow { get; set; }
        public HomePage HomePage { get; }

        public Page CurrentPage { get; set; }



        public AppDbContext AppDbContext { get; set; }

        private MetadataWindowVM metadataWindowVM { get; }
        private PerforceWindowVM _perforceWindowVM { get; set; }

        private Project _selectedProject;
        public Project SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                OnPropertyChanged(nameof(SelectedProject));

                if(_perforceWindowVM != null)
                {
                    _perforceWindowVM.SelectedProject = _selectedProject;
                }
            }
        }




        public MainPageVM()
        {
            AppDbContext = new AppDbContext();

            _perforceWindowVM = new PerforceWindowVM();

            OverviewPageVM overViewPageVM = new(this);

            HomePageVM homePageVM = new(this, overViewPageVM);

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
        public void OpenPerforceConfiguration()
        {
            PerforceWindow = new PerforceWindow
            {
                DataContext = _perforceWindowVM,

            };
            _perforceWindowVM.CloseWindowAction = () => PerforceWindow.Close();

            PerforceWindow.ShowDialog();

        }


    }
}
