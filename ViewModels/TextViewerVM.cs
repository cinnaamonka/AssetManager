using AssetManager.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AssetManager.ViewModels
{
    internal class TextViewerVM : ObservableObject
    {
        private string _fileContent;

        public string FileContent
        {
            get => _fileContent;
            set => SetProperty(ref _fileContent, value);
        }

        public RelayCommand CloseCommand { get; }

        public TextViewerVM()
        {
            CloseCommand = new RelayCommand(CloseWindow);
        }

        private void CloseWindow()
        {
            App.Current.Windows.OfType<TextViewerWindow>().FirstOrDefault()?.Close();
        }
    }
}
