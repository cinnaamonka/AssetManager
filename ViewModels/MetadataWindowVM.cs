using AssetManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AssetManager.ViewModels
{
    public class MetadataWindowVM : ObservableObject
    {
        private AssetMetadata _selectedMetadata;

        public MainPageVM MainPageVM { get; }
        public OverviewPageVM OverViewPageVM { get; }

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
        }

        public MetadataWindowVM() { }
        public void SetMetadata(AssetMetadata metadata)
        {
            SelectedMetadata = metadata;
        }

    }
}
