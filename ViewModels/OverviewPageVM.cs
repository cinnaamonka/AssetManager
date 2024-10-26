using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AssetManager.Models;
using System.Text.RegularExpressions;
using AssetManager.Services;
using System.Windows.Controls;
using System.Windows.Input;

namespace AssetManager.ViewModels
{
    public class OverviewPageVM : ObservableObject
    {
        private List<Asset> _assets;
        private List<Asset> _filteredAssets;
        private Asset _selectedAsset;

        public List<Asset> Assets
        {
            get { return _assets; }
            set
            {
                _assets = value;
                OnPropertyChanged(nameof(Assets));
            }
        }

        public Asset SelectedAsset
        {
            get { return _selectedAsset; }
            set
            {
                _selectedAsset = value;
                OnPropertyChanged(nameof(SelectedAsset));
               // MainPageVM.HandleOpenDetailsPage();
            }
        } 

        public List<Asset> FilteredAssets
        {
            get { return _filteredAssets; }
            set
            {
                _filteredAssets = value;
                OnPropertyChanged(nameof(FilteredAssets));
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ExecuteSearch();
            }
        }

        private MetadataService _metadataService;

        public RelayCommand SearchCommand { get; set; }

        public MainPageVM MainPageVM { get; }

        public OverviewPageVM(MainPageVM mainPageVM)
        {
            MainPageVM = mainPageVM;

            Assets = new List<Asset>
            {
                new Asset( "Tree Model", "/Assets/Models/Tree/tree.fbx", "3D Model"),
                new Asset( "Rock Texture", "/Assets/Textures/rock.png", "Texture"),
                new Asset( "Mountain Model", "/Assets/Models/Mountain/mountain.fbx", "3D Model")
            };


            FilteredAssets = new List<Asset>(Assets);

            SearchCommand = new RelayCommand(ExecuteSearch);
        }

        public OverviewPageVM() { }
      

        public async Task LoadAssetsAsync()
        {
            foreach(var asset in Assets)
            {
                var metadata = await _metadataService.LoadMetadataAsync(asset.FileName);

                if(metadata != null)
                {
                    asset.Metadata = metadata;
                }

                else
                {
                    asset.Metadata = new AssetMetadata
                    {
                        Name = asset.FileName,
                        FilePath = asset.FilePath,
                        FileType = asset.FileType,
                        FileSize = 0,
                        Format = "Not defined"
                    };

                    await _metadataService.SaveMetadataAsync(asset.Metadata);
                }
            }
        }

        private void ExecuteSearch()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                FilteredAssets = new List<Asset>(Assets);
            }
            else
            {
                try
                {
                    var regex = new Regex("^" + Regex.Escape(SearchText), RegexOptions.IgnoreCase);
                    var filtered = Assets.Where(a => regex.IsMatch(a.FileName)).ToList();
                    FilteredAssets = new List<Asset>(filtered);
                }
                catch (RegexParseException)
                {
                    // Handle invalid regex input if necessary
                }
            }
            OnPropertyChanged(nameof(FilteredAssets));
        }

      
    }
}
