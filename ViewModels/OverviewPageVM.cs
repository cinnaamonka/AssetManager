using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AssetManager.Models;
using System.Text.RegularExpressions;
using AssetManager.Services;
using AssetManager.Repositories;
using AssetManager.Views;
using System.IO;

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
            get
            {
                return _selectedAsset;
            }
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

        public RelayCommand OpenFullImageCommand { get; set; }
        public RelayCommand ClearSelectionCommand { get; }

        public RelayCommand OpenMetadataFileCommand { get; }


        public MainPageVM MainPageVM { get; }
        private AssetRepository _assetRepository;


        public OverviewPageVM(MainPageVM mainPageVM)
        {
            MainPageVM = mainPageVM;


            Assets = new List<Asset>();
            FilteredAssets = new List<Asset>();

            SearchCommand = new RelayCommand(ExecuteSearch);
            OpenHomePageCommand = new RelayCommand(OpenHomePage);
            OpenFullImageCommand = new RelayCommand(OpenFullImage);
            ClearSelectionCommand = new RelayCommand(ClearSelection);
            OpenMetadataFileCommand = new RelayCommand(OpenMetadataFile);

            _assetRepository = new AssetRepository();
            _metadataService = new MetadataService();
        }

        public OverviewPageVM() { }


        public Task GetMetadata()
        {
            foreach (var asset in Assets)
            {
                asset.Metadata = new AssetMetadata
                {
                    Name = asset.FileName,
                    FilePath = asset.FilePath,
                    FileType = asset.FileType,
                    Format = "Not defined",

                };

                if (asset.Metadata.FilePath != "")
                {
                    FileInfo fileInfo = new FileInfo(asset.Metadata.FilePath);
                    asset.Metadata.FileSize = Math.Round((fileInfo.Length / 1024.0) ,0);
                    asset.Metadata.Format = fileInfo.Extension;
                    asset.Metadata.DateCreated = fileInfo.CreationTimeUtc;
                    asset.Metadata.DateLastChanged = fileInfo.LastAccessTimeUtc;
                }
                else
                {
                    asset.Metadata.FileSize = 0;
                }


            }
            return Task.CompletedTask;
        }

    private void ExecuteSearch()
    {
        if (string.IsNullOrEmpty(SearchText))
        {
            var regex = new Regex("^" + Regex.Escape(SearchText), RegexOptions.IgnoreCase);
            var filtered = Assets.Where(a => regex.IsMatch(a.FileName)).ToList();
            FilteredAssets = new List<Asset>(filtered);

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

    private void ClearSelection()
    {
        SelectedAsset = null;
    }
    private void OpenMetadataFile()
    {
        MainPageVM?.HandleOpenPopUpWindow(SelectedAsset);
    }

    private void OpenHomePage()
    {
        MainPageVM?.OpenHomePage();
    }

    private void OpenFullImage()
    {
        var imageViewer = new ImageViewerWindow(SelectedAsset.FilePath)
        {
            Owner = App.Current.MainWindow
        };
        imageViewer.ShowDialog();
    }

    public async Task LoadAssetsFromUnityProject(string projectPath)
    {

        Assets = await _assetRepository.LoadAssetsFromUnityProjectAsync(projectPath);
        FilteredAssets = Assets;
        await GetMetadata();

    }


}
}
