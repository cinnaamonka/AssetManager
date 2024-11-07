using static AssetManager.AssetHelpers.AssetHelpers;
using System;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace AssetManager.Models
{
    public class AssetMetadata
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public AssetType FileType { get; set; }
        public double FileSize { get; set; }
        public string Format { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateLastChanged { get; set; }

      
    }
}
