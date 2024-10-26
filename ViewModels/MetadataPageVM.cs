using AssetManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AssetManager.ViewModels
{
    public class MetadataPageVM : ObservableObject
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


        public MetadataPageVM(MainPageVM mainPageVM, OverviewPageVM overviewVM)
        {
            MainPageVM = mainPageVM;
            OverViewPageVM = overviewVM;
        }

        public MetadataPageVM() { }
        public void SetMetadata(AssetMetadata metadata)
        {
            SelectedMetadata = metadata;
        }

    }
}
