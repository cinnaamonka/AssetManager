﻿using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Windows.Media;
using static AssetManager.AssetHelpers.AssetHelpers;

namespace AssetManager.Models
{
    public class Asset
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public AssetType FileType { get; }

        public string RelativePath { get; set; }

        public ImageSource PreviewImage { get; set; }

        public AssetMetadata Metadata { get; set; }

        public ICommand OpenMetadataCommand { get; }

        public Asset(string name, string filePath, AssetType fileType, string relativePath = "", ImageSource preview = null)
        {
            FileName = name;
            FilePath = filePath;
            FileType = fileType;
            RelativePath = relativePath;
            PreviewImage = preview;

            Metadata = new AssetMetadata();
            OpenMetadataCommand = new RelayCommand(OpenMetadata);
        }

        private void OpenMetadata()
        {
            MetadataRequested?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler MetadataRequested;
    }

  
}
