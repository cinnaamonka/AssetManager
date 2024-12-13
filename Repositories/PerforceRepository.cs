using Perforce.P4;
using System.Net;
using System.IO;

namespace AssetManager.Repositories
{
    public class PerforceRepository
    {
        private Repository _repository;
        private string _workspaceName;

        private string _depotPath = "//grpprj11/Dev/Around The Arena/Assets/ArtAssets/UI/Textures/PlayerIcon.png //mparniuk_Maryia_8712/grpprj11/Dev/Around The Arena/Assets/ArtAssets/UI/Textures/PlayerIcon.png";
        private Server _server;

        public PerforceRepository(string serverUri, string username, string password,string workspaceName)
        {
            _server = new Server(new ServerAddress(serverUri));
            _repository = new Repository(_server);

        
            _repository.Connection.UserName = username;

            // Connect using the repository's connection
            bool isConnected = _repository.Connection.Connect(null);
            if (!isConnected)
            {
                throw new Exception("Unable to connect to the Perforce server.");
            }

            // Login using the repository's connection
            if (!string.IsNullOrEmpty(password))
            {
                _repository.Connection.Login(password);
            }

            _workspaceName = workspaceName;

        }

        public bool IsConnected()
        {
            return _repository.Connection != null && _repository.Connection.Status == ConnectionStatus.Connected;
        }

        public void SyncWorkspace(string workspaceName, string selectedProjectPath)
        {
            if (IsConnected())
            {
                var client = _repository.GetClient(workspaceName);
                if (client == null)
                {
                    throw new Exception($"Workspace '{workspaceName}' does not exist or is not accessible.");
                }

                // Assign the client to the repository's connection
                _repository.Connection.Client = client;

                // Log the client root for debugging
                string clientRoot = _repository.Connection.Client.Root;
                Console.WriteLine($"Client Root: {clientRoot}");

                // Ensure selectedProjectPath is absolute
                if (!Path.IsPathRooted(selectedProjectPath))
                {
                    throw new Exception("selectedProjectPath must be an absolute path.");
                }

                // Construct the depot path (use the absolute path directly)
                _depotPath = selectedProjectPath;

                // Log the depot path for debugging
                Console.WriteLine($"Depot Path: {_depotPath}");

                try
                {
                    // Create a FileSpec with the absolute path
                    var depotFileSpec = new FileSpec(new LocalPath(_depotPath), null);

                    // Sync the file
                    _repository.Connection.Client.SyncFiles(new List<FileSpec> { depotFileSpec }, null);

                    Console.WriteLine($"File {_depotPath} synced successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to sync file: {ex.Message}");
                    throw;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("You are disconnected.");
            }
        }



    }
}
