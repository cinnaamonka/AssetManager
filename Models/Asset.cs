﻿using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Input;
using System.Windows.Media;
using static AssetManager.AssetHelpers.AssetHelpers;

namespace AssetManager.Models
{
    public class Asset
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }


        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }


        public string RelativePath { get; set; }

        public string PreviewImagePath{ get; set; }

        public AssetMetadata Metadata { get; set; }

        public ObservableCollection<AssetTag> AssetTags { get; set; }

        public Asset() { }

        public Asset(string name, string filePath,int projectId, AssetType fileType, string relativePath = "", string previewImagePath = null)
        {
            
            FileName = name;
            FilePath = filePath;
            FileType = fileType.ToString();
            RelativePath = relativePath;
            PreviewImagePath = previewImagePath;
            ProjectId = projectId;

            Metadata = new AssetMetadata();
            AssetTags = new ObservableCollection<AssetTag>();
        }

      
    }

  
}
