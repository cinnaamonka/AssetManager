using AssetManager.Models;
using AssetManager.Views;

namespace AssetManager.ViewModels
{
    public class MetadataPageVM
    {
        public AssetMetadata SelectedMetadata { get; set; }

        public MainPageVM MainPageVM { get; }
        public OverviewPageVM OverViewPageVM { get; }

        public MetadataPageVM(MainPageVM mainPageVM, OverviewPageVM overviewVM)
        {
            MainPageVM = mainPageVM;
            OverViewPageVM = overviewVM;
        }

    }
}
