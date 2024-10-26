using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AssetManager.Models;
using System.Text.RegularExpressions;
using AssetManager.Services;
using System.IO;
using System.Windows.Forms;
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
        public RelayCommand OpenMetadataCommand { get; set; }
        public RelayCommand OpenHomePageCommand { get; set; }

        public MainPageVM MainPageVM { get; }

        private string _unityProjectPath;
        public string UnityProjectPath
        {
            get => _unityProjectPath;
            set => SetProperty(ref _unityProjectPath, value);
        }
        public OverviewPageVM(MainPageVM mainPageVM)
        {
            MainPageVM = mainPageVM;

            Assets = new List<Asset>
            {
                new Asset( "Tree Model", "/Assets/Models/Tree/tree.fbx", "3D Model"),
                new Asset( "Rock Texture", "/Assets/Textures/rock.png", "Texture"),
                new Asset( "Mountain Model", "/Assets/Models/Mountain/mountain.fbx", "3D Model")
            };

            foreach (var asset in Assets)
            {
                asset.MetadataRequested += (s, e) => OpenMetadataFile((Asset)s);
            }


            FilteredAssets = new List<Asset>(Assets);

            SearchCommand = new RelayCommand(ExecuteSearch);
            OpenHomePageCommand = new RelayCommand(OpenHomePage);

        }

        public OverviewPageVM() { }


        public async Task LoadAssetsAsync()
        {
            foreach (var asset in Assets)
            {
                var metadata = await _metadataService.LoadMetadataAsync(asset.FileName);

                if (metadata != null)
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

        private void OpenMetadataFile(Asset asset)
        {
            MainPageVM?.HandleOpenPopUpWindow(asset);
        }

        private void OpenHomePage()
        {
            MainPageVM?.OpenHomePage();
        }

        public void OpenFolderDialog()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    UnityProjectPath = folderDialog.SelectedPath;
                    LoadAssetsFromUnityProject(UnityProjectPath);
                }
            }
        }
        private void LoadAssetsFromUnityProject(string projectPath)
        {
            FilteredAssets.Clear();

            string assetsFolderPath = Path.Combine(projectPath, "Assets");

            if (Directory.Exists(assetsFolderPath))
            {
                string[] files = Directory.GetFiles(assetsFolderPath, "*.*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    if (!file.EndsWith(".meta"))
                    {
                        var asset = new Asset(
                            name: Path.GetFileName(file),
                            filePath: file,
                            fileType: Path.GetExtension(file));

                        FilteredAssets.Add(asset);
                    }
                }
            }
        }
    }
}
