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
        public string FileType { get; set; }
        public double FileSize { get; set; }
        public string Format { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateLastChanged { get; set; }

        public int AssetId { get; set; }
        public Asset Asset { get; set; }


        public int VertexCount { get; set; }
        public int FaceCount { get; set; }

    }
}
