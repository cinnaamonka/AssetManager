using Perforce.P4;
using System.Net;

namespace AssetManager.Repositories
{
    public class PerforceRepository
    {
        private Repository _repository;

        public PerforceRepository(string serverUri, string username, string password)
        {
            var server = new Server(new ServerAddress(serverUri));

            var connection = new Connection(server)
            {
                UserName = username
            };

            bool isConnected = connection.Connect(null);
            if (!isConnected)
            {
                throw new Exception("Unable to connect to the Perforce server.");
            }

            if (!string.IsNullOrEmpty(password))
            {
                connection.Login(password);
            }


            _repository = new Repository(server);
        }

        public PerforceRepository()
        {

        }

        public bool IsConnected()
        {
            return _repository.Connection != null && _repository.Connection.Status == ConnectionStatus.Connected;
        }

        public void SyncWorkspace(string workspaceName)
        {
            _repository.Connection.Client = _repository.GetClient(workspaceName);

            var fileSpec = new FileSpec(new DepotPath("//..."), null);

            _repository.Connection.Client.SyncFiles(new List<FileSpec> { fileSpec }, null);

            Console.WriteLine("Workspace synced successfully.");
        }
    }
}
