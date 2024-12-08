using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Models
{
    public class PerforceConfig
    {
        public string ServerUri { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string WorkspaceName { get; set; }
    }
}
