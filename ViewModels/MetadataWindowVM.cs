using AssetManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace AssetManager.ViewModels
{
    public class MetadataWindowVM : ObservableObject
    {
        private AssetMetadata _selectedMetadata;

        public MainPageVM MainPageVM { get; }
        public OverviewPageVM OverViewPageVM { get; }

        public RelayCommand CloseWindowCommand { get; } 

        public AssetMetadata SelectedMetadata
        {
            get { return _selectedMetadata; }
            set
            {
                _selectedMetadata = value;
                OnPropertyChanged(nameof(SelectedMetadata));
            }
        }


        public MetadataWindowVM(MainPageVM mainPageVM, OverviewPageVM overviewVM)
        {
            MainPageVM = mainPageVM;
            OverViewPageVM = overviewVM;
            CloseWindowCommand = new RelayCommand(CloseWindow);
        }

        public MetadataWindowVM() { }
        public void SetMetadata(AssetMetadata metadata)
        {
            SelectedMetadata = metadata;
        }

        private void CloseWindow()
        {
            App.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
        }
    }
}
