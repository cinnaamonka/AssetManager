using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AssetManager.Models;
using System.Text.RegularExpressions;
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
        private List<Tag> _tags;


        public List<Asset> Assets
        {
            get { return _assets; }
            set
            {
                _assets = value;
                OnPropertyChanged(nameof(Assets));
            }
        }

        public List<Tag> Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                OnPropertyChanged(nameof(Tags));
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

        public RelayCommand SearchCommand { get; set; }
        public RelayCommand OpenMetadataCommand { get; set; }
        public RelayCommand OpenHomePageCommand { get; set; }

        public RelayCommand OpenFullImageCommand { get; set; }
        public RelayCommand ClearSelectionCommand { get; }

        public RelayCommand OpenMetadataFileCommand { get; }
        public RelayCommand RemoveAllTagsCommand { get; set; }
        public RelayCommand AddTagCommand { get; set; }

        public MainPageVM MainPageVM { get; }

        private AssetRepository _assetRepository;
        private TagRepository _tagRepository;

        private string _newTagName;

        public string NewTagName
        {
            get { return _newTagName; }
            set { _newTagName = value; }
        }

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
            RemoveAllTagsCommand = new RelayCommand(RemoveAllTags);
            AddTagCommand = new RelayCommand(AddTag);

            _assetRepository = new AssetRepository();
            _tagRepository = new TagRepository(MainPageVM.AppDbContext);

            LoadAllTags();
        }

        public OverviewPageVM() { }

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

        public async void LoadAllTags()
        {
            Tags = await _tagRepository.GetAllTagsAsync();
        }

        public async void RemoveAllTags()
        {
            await _tagRepository.RemoveAllTagsAsync();
            Tags = new List<Tag>();
            OnPropertyChanged(nameof(Tags));
        }
        public async void LoadAssetTags(int assetId)
        {
            var tags = await _tagRepository.GetAssetTagsAsync(assetId);

            Tags.Clear();

            foreach (var tag in tags)
            {
                Tags.Add(tag);
            }
        }

        public async void AddTag(int assetId, string tagName)
        {
            await _tagRepository.AddAssetTagAsync(assetId, tagName);
            LoadAssetTags(assetId);
        }

        public async void RemoveTag(int assetId, string tagName)
        {
            await _tagRepository.RemoveTagFromAssetAsync(assetId, tagName);
            LoadAssetTags(assetId);
        }

        public async void AddTag()
        {
            if (NewTagName == null) return;
            
            var newTag = await _tagRepository.AddTag(NewTagName);
            if (!string.IsNullOrWhiteSpace(NewTagName))
            {
              
                Tags = Tags.Append(newTag).ToList();
                NewTagName = string.Empty; 
                OnPropertyChanged(nameof(Tags));
                OnPropertyChanged(nameof(NewTagName));
            }
        }
    }
}
