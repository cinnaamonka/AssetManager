using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Text.Json;
using AssetManager.Models;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AssetManager.ViewModels
{
    internal class HomePageVM : ObservableObject
    {
        public MainPageVM MainPageVM { get; }
        public OverviewPageVM OverViewPageVM { get; }

        public RelayCommand OpenOverviewPageCommand { get; set; }
        public RelayCommand BrowseProjectFiles { get; set; }

        private ObservableCollection<Project> _projects;
        public ObservableCollection<Project> Projects
        {
            get { return _projects; }
            set => SetProperty(ref _projects, value);
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
            Projects = new ObservableCollection<Project>();
            LoadProjects();
        }

        private void OpenOverviewPage()
        {
            MainPageVM.OpenOverViewPage();
        }
        private void BrowseFiles()
        {
            OverViewPageVM.OpenFolderDialog();
            var project = new Project()
            {
                Name = OverViewPageVM.UnityProjectPath.Substring(OverViewPageVM.UnityProjectPath.LastIndexOf(@"\") + 1),
                DateAdded = DateTime.Now,
                FileCount = OverViewPageVM.FilteredAssets.Count,
                Id = Projects.Count + 1
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

    }
}

