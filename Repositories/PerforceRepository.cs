using Perforce.P4;
using System.Linq;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Windows;

namespace AssetManager.Repositories
{
    public class PerforceRepository
    {
        private Repository _repository;
        private string _workspaceName;

        private string _depotPath = "//grpprj11/Dev/Around The Arena/Assets/ArtAssets/UI/Textures/PlayerIcon.png //mparniuk_Maryia_8712/grpprj11/Dev/Around The Arena/Assets/ArtAssets/UI/Textures/PlayerIcon.png";
        private Server _server;

        public PerforceRepository()
        {


        }

        public void ConnectToPerforce(string serverUri, string username, string password, string workspaceName)
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
                    System.Windows.MessageBox.Show("You are disconnected from Perforce.");
                    throw;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("You are disconnected.");
            }
        }

        //GET
        public Changelist GetLatestChangelist(string depotPath = null)
        {
            try
            {
                var options = new Options();
                options["-m"] = "1";

                if (!string.IsNullOrWhiteSpace(depotPath))
                {
                    options["//"] = depotPath;
                }

                var changelists = _repository.GetChangelists(options);

                return changelists.FirstOrDefault();
            }
            catch (P4Exception ex)
            {
                System.Windows.MessageBox.Show($"Perforce Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return null;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return null;
            }
        }

        // EDIT
        public void EditFile(string filePathToEdit, string depotPath = null)
        {
            try
            {
                Changelist myChange = new Changelist();
                myChange.Description = "This is a custom changelist.";
                myChange = _repository.CreateChangelist(myChange);


                FileSpec editFile = new FileSpec(new LocalPath(filePathToEdit));
                Options editOptions = new Options();
                editOptions["-c"] = myChange.Id.ToString();


                Console.WriteLine("Opening file for edit...");
                _repository.Connection.Client.EditFiles(editOptions, new FileSpec[] { editFile });


                Console.WriteLine("Editing the file...");
                string content = System.IO.File.ReadAllText(filePathToEdit);
                content = $"{content}\nThis is a new line of text";
                System.IO.File.WriteAllText(filePathToEdit, content);


                Console.WriteLine("Submitting the changelist...");
                myChange.Submit(new Options());

                Console.WriteLine("File edited and submitted successfully.");
            }
            catch (P4Exception ex)
            {
                Console.WriteLine($"Perforce Error: {ex.Message} (Code: {ex.ErrorCode})");
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"File IO Error: {ioEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }
        }

        //CHECK OUT
        public void CheckOutFile(string filePath, int changelistId)
        {
            try
            {
                FileSpec editFile = new FileSpec(new LocalPath(filePath));
                Options editOptions = new Options { ["-c"] = changelistId.ToString() };

                _repository.Connection.Client.EditFiles(editOptions, new FileSpec[] { editFile });
                Console.WriteLine($"File {filePath} checked out for editing under changelist {changelistId}.");
            }
            catch (P4Exception ex)
            {
                Console.WriteLine($"Perforce Error: {ex.Message} (Code: {ex.ErrorCode})");
                throw;
            }
        }


        // NEW CHANGELIST
        public Changelist CreateNewChangelist(string description)
        {
            try
            {
                Changelist newChange = new Changelist { Description = description };
                return _repository.CreateChangelist(newChange);
            }
            catch (P4Exception ex)
            {
                Console.WriteLine($"Failed to create changelist: {ex.Message}");
                throw;
            }
        }

        //SUMBIT CHANGELIST
        public void SubmitChangelist(Changelist changelist)
        {
            try
            {
                changelist.Submit(new Options());
                Console.WriteLine($"Changelist {changelist.Id} submitted successfully.");
            }
            catch (P4Exception ex)
            {
                Console.WriteLine($"Failed to submit changelist: {ex.Message}");
                throw;
            }
        }


        // VIEW LAST CHANGES 
        public AssetHelpers.AssetHelpers.ProjectPerforceDetails GetLastChangeDetails(string filePath)
        {
            try
            {
             
                var depotFileSpec = new FileSpec(new LocalPath(filePath), null);
                IList<FileMetaData> fileMetaData = _repository.GetFileMetaData(new FileSpec[] { depotFileSpec }, null);

                if (fileMetaData != null && fileMetaData.Count > 0)
                {
                    var fileData = fileMetaData[0];

                    int lastChange = fileData.HeadChange;
                    DateTime lastModifiedTime = fileData.HeadModTime;

                    Console.WriteLine($"File: {filePath}");
                    Console.WriteLine($"Last Changelist: {lastChange}");
                    Console.WriteLine($"Last Modified Time: {lastModifiedTime}");

                    Changelist changelist = _repository.GetChangelist(lastChange);
                    string lastUser = changelist.OwnerName;

                    Console.WriteLine($"Last Modified By: {lastUser}");

                    AssetHelpers.AssetHelpers.ProjectPerforceDetails details = new AssetHelpers.AssetHelpers.ProjectPerforceDetails
                    {
                        LastChanged = lastModifiedTime,
                        LastChangeMadeByUser = lastUser.ToString(),
                    };

                    return details;
                }
                else
                {
                    Console.WriteLine($"No metadata available for the file: {filePath}");
                    return new AssetHelpers.AssetHelpers.ProjectPerforceDetails();
                }
            }
            catch (P4Exception ex)
            {
                Console.WriteLine($"Perforce Error: {ex.Message}");
                return new AssetHelpers.AssetHelpers.ProjectPerforceDetails();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return new AssetHelpers.AssetHelpers.ProjectPerforceDetails();
            }
        }

    }
}
