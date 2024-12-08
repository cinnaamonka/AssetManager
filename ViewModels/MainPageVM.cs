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
               

            }
        }

        // Version control
        PerforceRepository _perforceRepository;
  
        public RelayCommand OpenVersionControlConfigurationCommand { get; set; }

        public MainPageVM()
        {
            AppDbContext = new AppDbContext();

            OverviewPageVM overViewPageVM = new(this);
       
            HomePageVM homePageVM = new(this,overViewPageVM);

         

            metadataWindowVM = new(this, overViewPageVM);
            MainPage = new OverviewPage { DataContext = overViewPageVM };
            HomePage = new HomePage { DataContext = homePageVM };

            CurrentPage = HomePage;

            OpenVersionControlConfigurationCommand = new RelayCommand(OpenPerforceConfiguration);
            _perforceRepository = new PerforceRepository();
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
            var configWindow = new PerforceWindow();
            if (configWindow.ShowDialog() == true)
            {
                PerforceWindowVM perforceWindowVM = new PerforceWindowVM(_perforceRepository); 

                var config = perforceWindowVM.PerforceConfiguration; 

                try
                {
                    _perforceRepository = new PerforceRepository(
                        config.ServerUri, config.Username, config.Password
                    );
                    System.Windows.MessageBox.Show("Connected to Perforce successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Failed to connect: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


    }
}
