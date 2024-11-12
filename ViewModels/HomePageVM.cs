using CommunityToolkit.Mvvm.ComponentModel;
using AssetManager.Models;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;

namespace AssetManager.ViewModels
{
    internal class HomePageVM : ObservableObject
    {
        public MainPageVM MainPageVM { get; }
        public OverviewPageVM OverViewPageVM { get; }

        public LoaderVM Loader { get; set; } = new LoaderVM();
        public RelayCommand OpenOverviewPageCommand { get; set; }
        public AsyncRelayCommand BrowseProjectFiles { get; set; }
        public AsyncRelayCommand OpenProjectDetailsCommand { get; }

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
                if (_selectedProject != null)
                {
                    OpenProjectDetailsCommand.Execute(_selectedProject);
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
            BrowseProjectFiles = new AsyncRelayCommand(BrowseFiles);
            OpenProjectDetailsCommand = new AsyncRelayCommand(OpenProjectLibraryAsync);
            Projects = new ObservableCollection<Project>();
            LoadProjects();
        }


        private void OpenOverviewPage()
        {
            MainPageVM.OpenOverViewPage();
        }
        async private Task BrowseFiles()
        {
            OpenFolderDialog();

            var project = new Project()
            {
                Name = UnityProjectPath.Substring(UnityProjectPath.LastIndexOf(@"\") + 1),
                DateAdded = DateTime.Now,
                FileCount = OverViewPageVM.FilteredAssets.Count,
                Id = Projects.Count + 1,
                Path = UnityProjectPath
            };

            MainPageVM.AppDbContext.Projects.Add(project);
            MainPageVM.AppDbContext.SaveChanges();

            Loader.IsLoading = true;
            Loader.LoadingMessage = "Loading project...";

            await Task.Delay(10);

            await OverViewPageVM.LoadAssetsFromUnityProject(project.Path, project.Id);

            Loader.IsLoading = false;

            project.FileCount = OverViewPageVM.Assets.Count;
            MainPageVM.AppDbContext.SaveChanges();

            Projects.Add(project);

            OnPropertyChanged(nameof(Projects));
        }


        private void LoadProjects()
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
                await OverViewPageVM.LoadAssetsFromUnityProject(SelectedProject.Path, SelectedProject.Id);

            }
            finally
            {
                Loader.IsLoading = false;
                MainPageVM.OpenOverViewPage();
            }


        }
    }
}

