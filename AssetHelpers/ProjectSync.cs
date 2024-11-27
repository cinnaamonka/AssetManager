using AssetManager.Models;
using AssetManager.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.AssetHelpers
{
    internal class ProjectSync
    {
        private OverviewPageVM _viewModel;
        private ProjectWatcher _watcher;

        public ProjectSync(string projectPath, OverviewPageVM viewModel)
        {
            _viewModel = viewModel;

            _watcher = new ProjectWatcher(projectPath, UpdateDatabase);
        }

        private void UpdateDatabase(string filePath)
        {
            var assetInDb = _viewModel.FilteredAssets.FirstOrDefault(a => a.FilePath == filePath);

            if (File.Exists(filePath))
            {
                if (assetInDb == null)
                {
                    _viewModel.AddAsset(filePath);
                }
                else
                {
                 
                    assetInDb.Metadata.DateLastChanged = File.GetLastWriteTime(filePath);
                    _viewModel.UpdateAsset(assetInDb);
                }
            }
            else
            {
                if (assetInDb != null)
                {
                  
                    _viewModel.RemoveAsset(assetInDb);
                }
            }
        }

        public void StopSync()
        {
            _watcher.StopWatching();
        }
    }
}
