using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AssetManager.Models;
using System.Text.RegularExpressions;
using AssetManager.Repositories;
using AssetManager.Views;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;

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
                ExecuteSearch(SearchText);
            }
        }

        public RelayCommand<string> SearchCommand { get; set; }
        public RelayCommand OpenMetadataCommand { get; set; }
        public RelayCommand OpenHomePageCommand { get; set; }

        public AsyncRelayCommand<string> OpenFullImageCommand { get; set; }
        public RelayCommand ClearSelectionCommand { get; }

        public RelayCommand OpenMetadataFileCommand { get; }
        public RelayCommand RemoveAllTagsCommand { get; set; }
        public RelayCommand AddTagCommand { get; set; }
        public RelayCommand AddAssetTagCommand { get; set; }
        public ICommand RemoveTagCommand { get; set; }
        public ICommand RemoveAssetTagCommand { get; set; }
        public RelayCommand ConvertCommand { get; set; }
        public RelayCommand AddAssetCommand { get; set; }
        public RelayCommand<Asset> RemoveAssetCommand { get; set; }
       

        public MainPageVM MainPageVM { get; }

        private AssetRepository _assetRepository;
        private TagRepository _tagRepository;

        private FileSystemWatcher _fileWatcher;

        private string _newTagName;

        public string NewTagName
        {
            get { return _newTagName; }
            set { _newTagName = value; }
        }

        private string _newTagColor;

        public string NewTagColor
        {
            get { return _newTagColor; }
            set { _newTagColor = value; }
        }

        private string _newAssetTagName;
        public string NewAssetTagName
        {
            get { return _newAssetTagName; }
            set { _newAssetTagName = value; }
        }

        private string _newAssetTagColor;
        public string NewAssetTagColor
        {
            get { return _newAssetTagColor; }
            set { _newAssetTagColor = value; }
        }

        private Tag _selectedTag;
        public Tag SelectedTag
        {
            get { return _selectedTag; }
            set
            {
                _selectedTag = value;
             

            }
        }

        private List<string> _availableFormats;
        public List<string> AvailableFormats
        {
            get { return _availableFormats; }
            set { _availableFormats = value; }
        }

        private string _selectedFileToConvert;
        public string SelectedAssetToConvert
        {
            get { return _selectedFileToConvert; }
            set
            {
                _selectedFileToConvert = value;

                OnPropertyChanged(nameof(SelectedAssetToConvert));
                ExecuteSearch(SelectedAssetToConvert);

            }
        }

        private string _selectedFromFormat;
        public string SelectedFromFormat
        {
            get { return _selectedFromFormat; }
            set
            {
                _selectedFromFormat = value;
            }
        }

        private string _selectedToFormat;
        public string SelectedToFormat
        {
            get { return _selectedToFormat; }
            set
            {
                _selectedToFormat = value;
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

     

        public OverviewPageVM(MainPageVM mainPageVM)
        {
            MainPageVM = mainPageVM;


            Assets = new List<Asset>();
            FilteredAssets = new List<Asset>();
            AvailableFormats = new List<string>();

            SearchCommand = new RelayCommand<string>(ExecuteSearch);
            OpenHomePageCommand = new RelayCommand(OpenHomePage);
            OpenFullImageCommand = new AsyncRelayCommand<string>(OpenFullImage);
            ClearSelectionCommand = new RelayCommand(ClearSelection);
            OpenMetadataFileCommand = new RelayCommand(OpenMetadataFile);
            RemoveAllTagsCommand = new RelayCommand(RemoveAllTags);
            AddTagCommand = new RelayCommand(AddTag);
            AddAssetTagCommand = new RelayCommand(AddAssetTag);
            RemoveTagCommand = new RelayCommand<Tag>(RemoveTag);
            RemoveAssetTagCommand = new RelayCommand<AssetTag>(RemoveAssetTag);
            ConvertCommand = new RelayCommand(ConvertFile);
            RemoveAssetCommand = new RelayCommand<Asset>(RemoveAsset);
            AddAssetCommand = new RelayCommand(AddAsset);
          

            _assetRepository = new AssetRepository();

            foreach (var format in Enum.GetValues(typeof(AssetHelpers.AssetHelpers.AvailableFormats)))
            {
                AvailableFormats.Add(format.ToString());
            }

        

        }

        public OverviewPageVM() { }

        ~OverviewPageVM()
        {
            StopFileWatcher();
        }

        public void AddAsset(string filePath)
        {
            if (!AssetHelpers.AssetHelpers.IsSupportedFileType(filePath))
            {
                return;
            }

            try
            {
                string destinationFilePath = Path.Combine(MainPageVM.SelectedProject.Path, "Assets", Path.GetFileName(filePath));

                if(!File.Exists(destinationFilePath))
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);


                    File.Copy(filePath, destinationFilePath, overwrite: true);
                }


                var newAsset = _assetRepository.CreateAsset(
                 destinationFilePath,
                 MainPageVM.SelectedProject,
                 Path.GetExtension(destinationFilePath),
                 MainPageVM.AppDbContext
             );

                _assetRepository.SaveAsset(newAsset, MainPageVM.AppDbContext);
                 
                var selectedProject = MainPageVM.AppDbContext.Projects.FirstOrDefault(p => p.Id == MainPageVM.SelectedProject.Id);
                selectedProject.FileCount++;

                MainPageVM.AppDbContext.Projects.Update(selectedProject);
                MainPageVM.AppDbContext.SaveChanges();
                RefreshAssets();


            }
            catch (Exception ex)
            {
                Console.Write($"An error occurred while adding the asset: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddAsset()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = AssetHelpers.AssetHelpers.GetOpenFileDialogFilter(),
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                AddAsset(openFileDialog.FileName);
            }
        }

        public void RenameAsset(Asset asset)
        {
            if (asset != null)
            {
                _assetRepository.RenameAsset(asset, MainPageVM.AppDbContext);
                RefreshAssets();
            }
        }
        public void UpdateAsset(Asset asset)
        {
            MainPageVM.AppDbContext.Assets.Update(asset);

            if(asset != null)
            {

                MainPageVM.AppDbContext.SaveChanges();

                RefreshAssets();
            }
         
        }
        public void RemoveAsset(Asset asset)
        {
            _assetRepository.DeleteAssetFromProject(asset, MainPageVM.AppDbContext);
            var selectedProject = MainPageVM.AppDbContext.Projects.FirstOrDefault(p => p.Id == MainPageVM.SelectedProject.Id);
            selectedProject.FileCount--;

            MainPageVM.AppDbContext.Projects.Update(selectedProject);
            MainPageVM.AppDbContext.SaveChanges();
            RefreshAssets();
        }

        private void RefreshAssets()
        {
            Assets = _assetRepository.GetAssets(MainPageVM.AppDbContext);
            OnPropertyChanged(nameof(Assets));
            OnPropertyChanged(nameof(FilteredAssets));
            ExecuteSearch(SearchText);
        }

        void ConvertFile()
        {
            if (string.IsNullOrEmpty(SelectedFromFormat)
                || string.IsNullOrEmpty(SelectedToFormat)
                || SelectedAsset == null)
                return;

            try
            {
                _assetRepository.ProcessAssetConversion(SelectedFromFormat, SelectedToFormat, SelectedAsset, MainPageVM.AppDbContext);

                RefreshAssets();

            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Error during conversion: {ex.Message}");
            }

        }

        private void ExecuteSearch(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                FilteredAssets = new List<Asset>(Assets);
            }
            else
            {
                try
                {
                    var regex = new Regex(Regex.Escape(searchText), RegexOptions.IgnoreCase);
                    var filtered = Assets.Where(a => regex.IsMatch(a.FileName) ||
                        a.AssetTags != null && a.AssetTags.Any(tag => regex.IsMatch(tag.Tag.Name)));

                    FilteredAssets = new List<Asset>(filtered);
                }
                catch (RegexParseException)
                {

                    FilteredAssets = new List<Asset>();
                }
            }

            OnPropertyChanged(nameof(FilteredAssets));

        }

        public void FilterAssetsByTag()
        {
            FilteredAssets = Assets
                .Where(a => a.AssetTags != null && a.AssetTags.Any(tag =>
                tag.Tag.Name.Equals(SelectedTag.Name, StringComparison.OrdinalIgnoreCase)))
                .ToList();

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

        private Task<string> LoadFileAsync(string filePath)
        {
         
            return Task.Run(() =>
            {
               
                using (var reader = new StreamReader(filePath))
                {
                    return reader.ReadToEnd();
                }
            });
        }

        public async Task OpenFullImage(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            IsLoading = true;
            try
            {
                string extension = Path.GetExtension(filePath).ToLower();

                if (extension == ".png" || extension == ".jpg")
                {
                    IsLoading = false;
                    var imageWindow = new ImageViewerWindow(filePath)
                    {
                        Owner = App.Current.MainWindow
                    };
                   
                    imageWindow.ShowDialog();
                }
                else if (extension == ".obj" || extension == ".txt" )
                {
                 

                    string content = await LoadFileAsync(filePath);

                   

                    var textWindow = new TextViewerWindow(content)
                    {
                        Owner = App.Current.MainWindow
                    };
                    IsLoading = false;
                    textWindow.ShowDialog();
                  
                }
                else
                {
                    IsLoading = false;
                    System.Windows.MessageBox.Show("Unsupported file type.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
                System.Windows.MessageBox.Show($"Error reading the file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
         
        }



        public async Task LoadAssetsFromUnityProject(Project selectedProject)
        {
            if(_tagRepository == null)
            {
                _tagRepository = new TagRepository(MainPageVM.AppDbContext, MainPageVM);
            }


            Assets = await _assetRepository.LoadAssetsFromUnityProjectAsync(selectedProject,
                MainPageVM.AppDbContext,  _tagRepository);
            FilteredAssets = Assets;

            if (MainPageVM.SelectedProject != null)
            {
                InitializeFileWatcher(MainPageVM.SelectedProject.Path);
            }

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
                await _tagRepository.AddAssetTagAsync(SelectedAsset.Id, NewAssetTagName,NewAssetTagColor);
                NewAssetTagName = string.Empty;
                OnPropertyChanged(nameof(NewAssetTagName));
                OnPropertyChanged(nameof(SelectedAsset.AssetTags));
                LoadAllTags();
                OnPropertyChanged(nameof(Tags));
            }
        }

        public async void AddAssetTag(string assetTagName)
        {
            await _tagRepository.AddAssetTagAsync(SelectedAsset.Id, assetTagName);

            OnPropertyChanged(nameof(SelectedAsset.AssetTags));
            LoadAllTags();

        }

        public async void RemoveAssetTag(AssetTag assetTag)
        {
            await _tagRepository.RemoveTagFromAssetAsync(assetTag.AssetId, assetTag.Tag.Name);
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

            var newTag = await _tagRepository.AddTag(NewTagName,NewTagColor);


            if (!string.IsNullOrWhiteSpace(NewTagName) && newTag != null)
            {

                Tags = Tags.Append(newTag).ToList();
                NewTagName = string.Empty;
                OnPropertyChanged(nameof(Tags));
                OnPropertyChanged(nameof(NewTagName));
            }
            else
            {
                NewTagName = string.Empty;
                OnPropertyChanged(nameof(NewTagName));

            }
        }

        private void InitializeFileWatcher(string projectPath)
        {
            if (_fileWatcher != null)
            {
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Dispose();
                return;
            }

            _fileWatcher = new FileSystemWatcher
            {
                Path = projectPath,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName
            };

            // Subscribe to events
            _fileWatcher.Changed += OnFileChanged;
            _fileWatcher.Created += OnFileCreated;
            _fileWatcher.Deleted += OnFileDeleted;
            _fileWatcher.Renamed += OnFileRenamed;

            // Start monitoring
            _fileWatcher.EnableRaisingEvents = true;
        }

        public void StopFileWatcher()
        {
            if (_fileWatcher != null)
            {
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Dispose();
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var asset = Assets.FirstOrDefault(a => a.FilePath == e.FullPath);
                if (asset != null)
                {
                    asset.Metadata.DateLastChanged = File.GetLastWriteTime(e.FullPath);
                    UpdateAsset(asset); // Update in the database
                }
            });
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (!File.Exists(e.FullPath) || !AssetHelpers.AssetHelpers.IsSupportedFileType(e.FullPath))
                    return;

                AddAsset(e.FullPath); // Add to database and UI
            });
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var asset = Assets.FirstOrDefault(a => a.FilePath == e.FullPath);
                if (asset != null)
                {
                    RemoveAsset(asset); // Remove from database and UI
                }
            });
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var asset = Assets.FirstOrDefault(a => a.FilePath == e.OldFullPath);
                if (asset != null)
                {
                    asset.FilePath = e.FullPath;
                    asset.FileName = Path.GetFileName(e.FullPath);
                    UpdateAsset(asset); // Update in the database
                }
            });
        }

      
    }
}
