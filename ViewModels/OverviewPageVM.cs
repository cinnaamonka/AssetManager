using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AssetManager.Models;
using System.Text.RegularExpressions;
using AssetManager.Services;
using AssetManager.Repositories;
using AssetManager.Views;
using System.IO;
using Assimp;
using Microsoft.EntityFrameworkCore;

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

        private MetadataRepository _metadataRepository;

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
            _metadataRepository = new MetadataRepository();


        }

        public OverviewPageVM() { }


        public async Task GetMetadata(Asset asset)
        {
            using (MainPageVM.AppDbContext)
            {
                var metadataFile = _metadataRepository.LoadMetadata(asset);

                MainPageVM.AppDbContext.MetadataFiles.Add(metadataFile);
                await MainPageVM.AppDbContext.SaveChangesAsync();

            }
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

        public async Task LoadAssetsFromUnityProject(string projectPath, int currentProjectId)
        {

            Assets = await _assetRepository.LoadAssetsFromUnityProjectAsync(projectPath,
                MainPageVM.AppDbContext, currentProjectId);
            FilteredAssets = Assets;

        }


    }
}
