using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateAdded { get; set; }
        public int FileCount { get; set; }

        public string Path { get;set; }

        public bool WasInitialized { get; set; }

        // Perforce-related properties
        public string ServerUri { get; set; }
        public string WorkspaceName { get; set; }
        public string DepotPath { get; set; }
        public string PerforceUser { get; set; }
        public string PerforcePassword { get; set; }

        public bool IsPerforceEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(ServerUri)
                       && !string.IsNullOrEmpty(WorkspaceName)
                       && !string.IsNullOrEmpty(DepotPath);
            }
        }
    }
}
