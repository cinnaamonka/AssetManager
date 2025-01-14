using CommunityToolkit.Mvvm.ComponentModel;
using AssetManager.Models;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Data;

namespace AssetManager.ViewModels
{
    internal class HomePageVM : ObservableObject
    {
        public string RemoveIconPath => "C:\\Users\\parni\\Desktop\\2024-2025\\semester5\\GraduationWork\\AssetManager\\Resources\\minus.png";


        public MainPageVM MainPageVM { get; }
        public OverviewPageVM OverViewPageVM { get; }

        public LoaderVM Loader { get; set; } = new LoaderVM();
        public RelayCommand OpenOverviewPageCommand { get; set; }
        public RelayCommand<Project> RemoveProjectCommand { get; set; }
        public AsyncRelayCommand BrowseProjectFiles { get; set; }
        public AsyncRelayCommand OpenProjectDetailsCommand { get; }
        public RelayCommand OpenVersionControlConfigurationCommand { get; set; }

        private string _sourceControlStatus = "Perforce";
        public string SourceControlStatus
        {
            get => _sourceControlStatus;
            set => SetProperty(ref _sourceControlStatus, value);
        }

        private System.Windows.Media.Brush _sourceControlBackground = System.Windows.Media.Brushes.Transparent;
        public System.Windows.Media.Brush SourceControlBackground
        {
            get => _sourceControlBackground;
            set => SetProperty(ref _sourceControlBackground, value);
        }

        private ObservableCollection<Project> _projects;
        public ObservableCollection<Project> Projects
        {
            get { return _projects; }
            set => SetProperty(ref _projects, value);
        }

        private string _unityProjectPath;
        public string UnityProjectPath
        {
            get => _unityProjectPath;
            set => SetProperty(ref _unityProjectPath, value);
        }

        private Project _selectedProject;
        public Project SelectedProject
        {
            get => _selectedProject;
            set
            {
                SetProperty(ref _selectedProject, value);

                if (_selectedProject == null) return;

                if (Directory.Exists(_selectedProject.Path))
                {
                    MainPageVM.SelectedProject = _selectedProject;

                }
             
            }
        }

        public void OpenFolderDialog()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    UnityProjectPath = folderDialog.SelectedPath;
                }
            }
        }
        public HomePageVM()
        {

        }

        public HomePageVM(MainPageVM mainPageVM, OverviewPageVM OverviewPageViewModel)
        {
            MainPageVM = mainPageVM;
            OverViewPageVM = OverviewPageViewModel;
            OpenOverviewPageCommand = new RelayCommand(OpenOverviewPage);
            BrowseProjectFiles = new AsyncRelayCommand(AddProject);
            OpenProjectDetailsCommand = new AsyncRelayCommand(OpenProjectLibraryAsync);
            RemoveProjectCommand = new RelayCommand<Project>(RemoveProject);
            OpenVersionControlConfigurationCommand = new RelayCommand(OpenVersionControl);

            Projects = new ObservableCollection<Project>();
            LoadProjects();


            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Projects");
        }


        private void OpenOverviewPage()
        {
            if (SelectedProject != null)
            {
                OverViewPageVM.LoadAssetsFromUnityProject(SelectedProject);

            }
            else
            {
         
                OverViewPageVM.Assets = [];
                OverViewPageVM.FilteredAssets = [];
            }

            MainPageVM.OpenOverViewPage();
        }
        async private Task AddProject()
        {
            OpenFolderDialog();

            if (UnityProjectPath == null)
            {
                return;
            }

            var project = new Project()
            {
                Name = UnityProjectPath.Substring(UnityProjectPath.LastIndexOf(@"\") + 1),
                DateAdded = DateTime.Now,
                FileCount = OverViewPageVM.FilteredAssets.Count,
                Id = Projects.Count + 1,
                Path = UnityProjectPath,
                ServerUri = String.Empty,
                WorkspaceName = String.Empty,
                DepotPath = String.Empty,
                PerforceUser = String.Empty,
                PerforcePassword = String.Empty
            };

            MainPageVM.AppDbContext.Projects.Add(project);
            MainPageVM.AppDbContext.SaveChanges();

            Loader.IsLoading = true;
            Loader.LoadingMessage = "Loading project...";

            await Task.Delay(10);

            await OverViewPageVM.LoadAssetsFromUnityProject(project);

            Loader.IsLoading = false;

            project.FileCount = OverViewPageVM.Assets.Count;

            MainPageVM.AppDbContext.SaveChanges();

            Projects.Add(project);

            OnPropertyChanged(nameof(Projects));


            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),"Projects",project.Name));
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Projects", project.Name,"Thumbnails"));
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Projects", project.Name, "ObjFiles"));
        }

        public void RemoveProject(Project project)
        {
            var projectAssets = MainPageVM.AppDbContext.Assets.Where(a => a.ProjectId == project.Id).ToList();
            var thumbnails = projectAssets.Select(a => a.PreviewImagePath).ToList();

            MainPageVM.AppDbContext.Projects.Remove(project);
            MainPageVM.AppDbContext.SaveChanges();

            UnityProjectPath = null;

            Projects = new ObservableCollection<Project>(MainPageVM.AppDbContext.Projects.ToList());


            OnPropertyChanged(nameof(Projects));


            SelectedProject = null;
            MainPageVM.SelectedProject = null;

            OnPropertyChanged(nameof(MainPageVM.SelectedProject));
            OnPropertyChanged(nameof(SelectedProject));

            foreach (var thumbnail in thumbnails)
            {
                if (File.Exists(thumbnail) && !thumbnail.ToLower().Contains("resources"))
                {
                    File.Delete(thumbnail);
                }
            }

            OverViewPageVM.StopFileWatcher();
        }

        private async void LoadProjects()
        {

            MainPageVM.AppDbContext.Database.EnsureCreated();
            var projectsFromDb = MainPageVM.AppDbContext.Projects.ToList();
            foreach (var project in projectsFromDb)
            {
                Projects.Add(project);
            }

        }

        private async Task OpenProjectLibraryAsync()
        {


            Loader.IsLoading = true;
            Loader.LoadingMessage = "Loading project...";

            await Task.Delay(10);

            try
            {
                OverViewPageVM.LoadTags();
                if (SelectedProject == null) return;

                await OverViewPageVM.LoadAssetsFromUnityProject(SelectedProject);

            }
            finally
            {
                Loader.IsLoading = false;
                if (SelectedProject != null)
                {
                    MainPageVM.OpenOverViewPage();
                }


            }


        }
        public void OpenVersionControl()
        {
            if(MainPageVM.OpenPerforceConfiguration())
            {
                CollectionViewSource.GetDefaultView(Projects).Refresh();
                OnPropertyChanged(nameof(Projects));
            }

        
        }

      
    }
}

