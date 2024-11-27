using System.IO;

namespace AssetManager.AssetHelpers
{
    internal class ProjectWatcher
    {
        private FileSystemWatcher _watcher;
        private string _projectPath;
        private Action<string> _onFileChanged;

        public ProjectWatcher(string projectPath, Action<string> onFileChanged)
        {
            _projectPath = projectPath;
            _onFileChanged = onFileChanged;

            _watcher = new FileSystemWatcher
            {
                Path = _projectPath,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.DirectoryName
            };

            _watcher.Changed += OnChanged;
            _watcher.Created += OnChanged;
            _watcher.Deleted += OnChanged;
            _watcher.Renamed += OnRenamed;

            _watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            _onFileChanged?.Invoke(e.FullPath);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            _onFileChanged?.Invoke(e.FullPath);
        }

        public void StopWatching()
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
        }
    }
}
