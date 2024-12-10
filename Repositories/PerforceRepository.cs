using Perforce.P4;
using System.Net;
using System.Windows;

namespace AssetManager.Repositories
{
    public class PerforceRepository
    {
        private Repository _repository;

        private Connection _connection;
        private Server _server;

        public PerforceRepository(string serverUri, string username, string password)
        {
            _server = new Server(new ServerAddress(serverUri));

            _connection = new Connection(_server)
            {
                UserName = username
            };

            bool isConnected = _connection.Connect(null);
            if (!isConnected)
            {
                throw new Exception("Unable to connect to the Perforce server.");
            }

            if (!string.IsNullOrEmpty(password))
            {
                _connection.Login(password);
            }


            _repository = new Repository(_server);

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


                var fileSpec = new FileSpec(new DepotPath("//..."), null);

                _repository.Connection.Client.SyncFiles(new List<FileSpec> { fileSpec }, null);

                Console.WriteLine("Workspace synced successfully.");
            }
            else
            {
                System.Windows.MessageBox.Show("You are disconnected.");
            }

         
        }

    

    }
}
