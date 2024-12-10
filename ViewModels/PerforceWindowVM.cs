﻿using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using AssetManager.Repositories;
using System.Windows;
using AssetManager.Models;
using Assimp;

namespace AssetManager.ViewModels
{
    public class PerforceWindowVM : ObservableObject
    {
        public RelayCommand ConnectCommand { get; set; }

        public Action CloseWindowAction { get; set; }

        private PerforceRepository _perforceRepository; 

        public PerforceWindowVM()
        {
            ConnectCommand = new RelayCommand(ConnectToPerforce);
            _perforceConfiguration = new PerforceConfig();
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
                if (string.IsNullOrWhiteSpace(PerforceConfiguration.ServerUri) ||
                    string.IsNullOrWhiteSpace(PerforceConfiguration.Username) ||
                    string.IsNullOrWhiteSpace(PerforceConfiguration.WorkspaceName)
                    )
                {
                    System.Windows.MessageBox.Show("Please provide all required fields: Server URI, Username, and Password.",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                 

                    return;
                }

                _perforceRepository = new PerforceRepository(PerforceConfiguration.ServerUri, PerforceConfiguration.Username,
                     PerforceConfiguration.Password);

                System.Windows.MessageBox.Show($"Saved Configuration:\nServer URI: {PerforceConfiguration.ServerUri}\nUsername:" +
                    $" {PerforceConfiguration.Username}\nWorkspace: {PerforceConfiguration.WorkspaceName}",
                    "Save Successful", MessageBoxButton.OK, MessageBoxImage.Information);


                
                _perforceRepository.SyncWorkspace(PerforceConfiguration.WorkspaceName);
                

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
