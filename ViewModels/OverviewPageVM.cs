using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AssetManager.Models;
using System.Text.RegularExpressions;
using AssetManager.Repositories;
using AssetManager.Views;
using System.IO;
using Assimp;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows.Input;

namespace AssetManager.ViewModels
{
    public class OverviewPageVM : ObservableObject
    {
        public string RemoveIconPath => "C:\\Users\\parni\\Desktop\\2024-2025\\semester5\\GraduationWork\\AssetManager\\Resources\\minus.png";

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
        public RelayCommand AddAssetTagCommand { get; set; }
        public ICommand RemoveTagCommand { get; set; } 

        public MainPageVM MainPageVM { get; } 

        private AssetRepository _assetRepository;
        private TagRepository _tagRepository;



        private string _newTagName;

        public string NewTagName
        {
            get { return _newTagName; }
            set { _newTagName = value; }
        }

        private string _newAssetTagName;
        public string NewAssetTagName
        {
            get { return _newAssetTagName; }
            set { _newAssetTagName = value; }
        }

        private Tag _selectedTag;
        public Tag SetelectedTag
        {
            get { return _selectedTag; }
            set { _selectedTag = value;}
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
            AddAssetTagCommand = new RelayCommand(AddAssetTag);
            RemoveTagCommand = new RelayCommand<Tag>(RemoveTag);

            _assetRepository = new AssetRepository();
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

        public void LoadTags()
        {
            _tagRepository = new TagRepository(MainPageVM.AppDbContext, MainPageVM);

            LoadAllTags();
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
        public async Task<List<AssetTag>> LoadAssetTags(int assetId)
        {
            return await _tagRepository.GetAssetTagsAsync(assetId);
        }

        public async void AddAssetTag()
        {
            if (NewAssetTagName == null) return;

            if (!string.IsNullOrWhiteSpace(NewAssetTagName))
            {
                await _tagRepository.AddAssetTagAsync(SelectedAsset.Id, NewAssetTagName);
                NewAssetTagName = string.Empty;
                OnPropertyChanged(nameof(NewAssetTagName));
                OnPropertyChanged(nameof(SelectedAsset.AssetTags));
                LoadAllTags();
                OnPropertyChanged(nameof(Tags));
            }
        }

        public async void RemoveAssetTag(int assetId, string tagName)
        {
            await _tagRepository.RemoveTagFromAssetAsync(assetId, tagName);
        }
        public async void RemoveTag(Tag tag)
        {
            _tagRepository.RemoveTag(tag.Name); 
            LoadAllTags();
            OnPropertyChanged(nameof(Tags));
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
