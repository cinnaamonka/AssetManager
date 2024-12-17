using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using AssetManager.Repositories;
using System.Windows;
using AssetManager.Models;
using Assimp;
using Perforce.P4;

namespace AssetManager.ViewModels
{
    public class PerforceWindowVM : ObservableObject
    {
        public RelayCommand ConnectCommand { get; set; }

        public Action CloseWindowAction { get; set; }

        private Project _selectedProject;

        public MainPageVM MainPageVM { get; }


        AppDbContext _context;

        public PerforceWindowVM(MainPageVM mainpageVM)
        {
            MainPageVM = mainpageVM;

            ConnectCommand = new RelayCommand(ConnectToPerforce);
            _perforceConfiguration = new PerforceConfig();
        
        }
        public PerforceWindowVM() { }
        private PerforceConfig _perforceConfiguration { get; set; }
        public PerforceConfig PerforceConfiguration
        {
            get { return _perforceConfiguration; }
            set
            {
                _perforceConfiguration = value;

            }
        }

        public AppDbContext AppDbContext
        {
            get { return _context; }
            set
            {
                _context = value;
            }
        }

        public Project SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;
            }
        }

        public void SyncWorkspace()
        {
            try
            {

                MainPageVM.PerforceRepository.SyncWorkspace(_selectedProject.WorkspaceName, _selectedProject.Path);
                System.Windows.MessageBox.Show("Project successfully synced.", "Sync Successful", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error during sync: {ex.Message}",
                                       "Sync Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ConnectToPerforce()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(PerforceConfiguration.ServerUri) ||
                    string.IsNullOrWhiteSpace(PerforceConfiguration.Username) ||
                    string.IsNullOrWhiteSpace(PerforceConfiguration.WorkspaceName)
                    )
                {
                    System.Windows.MessageBox.Show("Please provide all required fields: Server URI, Username, and Password.",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);



                    return;
                }


                System.Windows.MessageBox.Show($"Saved Configuration:\nServer URI: {PerforceConfiguration.ServerUri}\nUsername:" +
                    $" {PerforceConfiguration.Username}\nWorkspace: {PerforceConfiguration.WorkspaceName}",
                    "Save Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                MainPageVM.PerforceRepository.ConnectToPerforce(PerforceConfiguration.ServerUri, PerforceConfiguration.Username, PerforceConfiguration.Password, PerforceConfiguration.WorkspaceName);
                MainPageVM.PerforceRepository.SyncWorkspace(PerforceConfiguration.WorkspaceName, _selectedProject.Path);


                var localProject = _context.Projects.Local.FirstOrDefault(p => p.Id == _selectedProject.Id);

                localProject.ServerUri = PerforceConfiguration.ServerUri;
                localProject.PerforceUser = PerforceConfiguration.Username;
                localProject.WorkspaceName = PerforceConfiguration.WorkspaceName;
                localProject.DepotPath = new LocalPath(_selectedProject.Path).ToString();
                localProject.PerforcePassword = PerforceConfiguration.Password;

                _context.Projects.Update(localProject);
                _context.SaveChanges();
                _selectedProject = localProject;

                OnPropertyChanged(nameof(_selectedProject));

                CloseWindowAction?.Invoke();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error during save: {ex.Message}",
                    "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
