using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AssetManager.Models;
using System.Text.RegularExpressions;


namespace AssetManager.ViewModels
{
    public class AssetViewModel : ObservableObject
    {
        private List<Asset> _assets;
        public List<Asset> Assets
        {
            get { return _assets; }
            set
            {
                _assets = value;
                OnPropertyChanged(nameof(Assets));
            }
        }

        private List<Asset> _filteredAssets;
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

        public RelayCommand SearchCommand { get; set; }

        public AssetViewModel()
        {
            Assets = new List<Asset>
            {
                new Asset( "Tree Model", "/Assets/Models/Tree/tree.fbx", "3D Model"),
                new Asset( "Rock Texture", "/Assets/Textures/rock.png", "Texture"),
                new Asset( "Mountain Model", "/Assets/Models/Mountain/mountain.fbx", "3D Model")
            };


            FilteredAssets = new List<Asset>(Assets);

            SearchCommand = new RelayCommand(ExecuteSearch);
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
