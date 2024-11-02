using CommunityToolkit.Mvvm.ComponentModel;


namespace AssetManager.ViewModels
{
    internal class LoaderVM : ObservableObject
    {
        private bool _isLoading = false;
        private string _loadingMessage;

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public string LoadingMessage
        {
            get { return _loadingMessage; }
            set
            {
                _loadingMessage = value;
                OnPropertyChanged(nameof(LoadingMessage));
            }
        }

        public LoaderVM()
        {
            IsLoading = false;
            LoadingMessage = "Loading...";
        }
    }
}
