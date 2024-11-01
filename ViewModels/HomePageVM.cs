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

        public RelayCommand OpenOverviewPageCommand { get; set; }
        public RelayCommand BrowseProjectFiles { get; set; }
        public RelayCommand OpenProjectDetailsCommand { get; }

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
                    // Trigger navigation whenever a project is selected
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
                    OverViewPageVM.LoadAssetsFromUnityProject(UnityProjectPath);

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
            BrowseProjectFiles = new RelayCommand(BrowseFiles);
            OpenProjectDetailsCommand = new RelayCommand(OpenProjectLibrary);
            Projects = new ObservableCollection<Project>();
            LoadProjects();
        }

        private void OpenOverviewPage()
        {
            MainPageVM.OpenOverViewPage();
        }
        private void BrowseFiles()
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
            using (var context = new AppDbContext())
            {
                context.Projects.Add(project);
                context.SaveChanges(); 
            } 

            Projects.Add(project);  

            OnPropertyChanged(nameof(Projects));
        }


        private void LoadProjects()
        {
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();
                var projectsFromDb = context.Projects.ToList();
                foreach (var project in projectsFromDb)
                {
                    Projects.Add(project);
                }
            }
        }

        private void OpenProjectLibrary()
        {
            MainPageVM.OpenOverViewPage();
            OverViewPageVM.LoadAssetsFromUnityProject(SelectedProject.Path);   
        }

    }
}

