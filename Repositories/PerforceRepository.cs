using Perforce.P4;
using System.Net;
using System.Windows;

namespace AssetManager.Repositories
{
    public class PerforceRepository
    {
        private Repository _repository;

        private const string DEPOT_PATH = "//grpprj11/Dev/Around The Arena/Assets/ArtAssets/UI/Textures/PlayerIcon.png //mparniuk_Maryia_8712/grpprj11/Dev/Around The Arena/Assets/ArtAssets/UI/Textures/PlayerIcon.png";
        private Server _server;

        public PerforceRepository(string serverUri, string username, string password)
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

            // Check if connected
            if (IsConnected())
            {
                int a = 10;
            }

        }

        public bool IsConnected()
        {
            return _repository.Connection != null && _repository.Connection.Status == ConnectionStatus.Connected;
        }

        public void SyncWorkspace(string workspaceName)
        {
            if(IsConnected())
            {
                var client = _repository.GetClient(workspaceName);
                if (client == null)
                {
                    throw new Exception($"Workspace '{workspaceName}' does not exist or is not accessible.");
                }


                _repository.Connection.Client = client;

                var smallFile = new FileSpec(new DepotPath(DEPOT_PATH), null);
                _repository.Connection.Client.SyncFiles(new List<FileSpec> { smallFile }, null);

                Console.WriteLine("Workspace synced successfully.");
            }
            else
            {
                System.Windows.MessageBox.Show("You are disconnected.");
            }

         
        }

    

    }
}
