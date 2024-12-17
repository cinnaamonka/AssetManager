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

            bool isConnected = _repository.Connection.Connect(null);
            if (!isConnected)
            {
                throw new Exception("Unable to connect to the Perforce server.");
            }

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

                _repository.Connection.Client = client;

                string clientRoot = _repository.Connection.Client.Root;
                Console.WriteLine($"Client Root: {clientRoot}");

                if (!Path.IsPathRooted(selectedProjectPath))
                {
                    throw new Exception("selectedProjectPath must be an absolute path.");
                }

                _depotPath = selectedProjectPath;

                Console.WriteLine($"Depot Path: {_depotPath}");

                try
                {
                    var depotFileSpec = new FileSpec(new LocalPath(_depotPath), null);

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
