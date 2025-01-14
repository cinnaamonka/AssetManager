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

        private PerforceRepository _perforceRepository;
        public PerforceRepository PerforceRepository
        {
            get { return _perforceRepository; }
            set
            {
                _perforceRepository = value;
            }
        }

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
                    _perforceWindowVM.AppDbContext = AppDbContext;
                }
            }
        }


        public MainPageVM()
        {
            AppDbContext = new AppDbContext();

          

            OverviewPageVM overViewPageVM = new(this);

            HomePageVM homePageVM = new(this, overViewPageVM);

            metadataWindowVM = new(this, overViewPageVM);
            _perforceWindowVM = new (this);

            MainPage = new OverviewPage { DataContext = overViewPageVM };
            HomePage = new HomePage { DataContext = homePageVM };

            CurrentPage = HomePage;

            _perforceRepository = new PerforceRepository();
        }

        public void HandleOpenPopUpWindow(Asset selectedAsset)
        {
            if (selectedAsset != null)
            {

                metadataWindowVM.SelectedMetadata = AppDbContext.Assets.FirstOrDefault(asset => asset.FileName == selectedAsset.FileName)?.Metadata;

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
        public bool OpenPerforceConfiguration()
        {
            PerforceWindow = new PerforceWindow
            {
                DataContext = _perforceWindowVM,

            };
            _perforceWindowVM.CloseWindowAction = () => PerforceWindow.Close();
           
             PerforceWindow.ShowDialog();

            return SelectedProject.IsPerforceEnabled;
      
        }

        public void SyncProject()
        {
            try
            {
                _perforceRepository.ConnectToPerforce(SelectedProject.ServerUri, SelectedProject.PerforceUser, SelectedProject.PerforcePassword,
                    SelectedProject.WorkspaceName);
                _perforceWindowVM.SyncWorkspace();

                var assets = AppDbContext.Assets;

                Parallel.ForEach(assets, asset =>
                {
                    var changeDetails = _perforceRepository.GetLastChangeDetails(asset.FilePath);

                    asset.Metadata.Author = changeDetails.LastChangeMadeByUser;
                    asset.Metadata.DateLastChanged = changeDetails.LastChanged;

                    lock (AppDbContext)
                    {
                        AppDbContext.Assets.Update(asset);
                      
                    }
                });

            
               
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

    }
}
