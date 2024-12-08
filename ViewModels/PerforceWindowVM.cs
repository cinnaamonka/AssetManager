using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using AssetManager.Repositories;
using System.Windows;
using AssetManager.Models;
using Assimp;

namespace AssetManager.ViewModels
{
    public class PerforceWindowVM : ObservableObject
    {
        private string _serverUri;
        public string ServerUri
        {
            get => _serverUri;
            set => SetProperty(ref _serverUri, value);
        }

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _workspaceName;
        public string WorkspaceName
        {
            get => _workspaceName;
            set => SetProperty(ref _workspaceName, value);
        }

        public RelayCommand ConnectCommand { get; }

        private PerforceRepository _perforceRepository;

        public Action CloseWindowAction { get; set; }



        public PerforceWindowVM(PerforceRepository perforceRepository)
        {
            ConnectCommand = new RelayCommand(ConnectToPerforce);
            _perforceConfiguration = new PerforceConfig();

            _perforceRepository = perforceRepository;

        }

        private PerforceConfig _perforceConfiguration { get; set; } 
        public PerforceConfig PerforceConfiguration 
        {
            get { return _perforceConfiguration; }
            set
            {
                _perforceConfiguration = value;
                
            }
        }

        private void ConnectToPerforce()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ServerUri) ||
                    string.IsNullOrWhiteSpace(Username) ||
                    string.IsNullOrWhiteSpace(WorkspaceName))
                {
                    System.Windows.MessageBox.Show("Please provide all required fields: Server URI, Username, and Password.",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

              
                System.Windows.MessageBox.Show($"Saved Configuration:\nServer URI: {ServerUri}\nUsername: {Username}\nWorkspace: {WorkspaceName}",
                    "Save Successful", MessageBoxButton.OK, MessageBoxImage.Information);

              
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
