using static AssetManager.AssetHelpers.AssetHelpers;
using System;
using System.IO;

namespace AssetManager.Models
{
    public class AssetMetadata
    { 
        public string Name { get; set; }
        public string FilePath { get; set; }
        public AssetType FileType { get; set; }
        public double FileSize { get; set; }
        public string Format { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateLastChanged { get; set; }
     

        public AssetMetadata()
        {    
            Name = "Not defined";
            FilePath = "Not defined";
            FileType = AssetType.Other;

            FileSize = 0;
            Format = "Not defined";
        }

      
    }
}
