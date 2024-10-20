namespace AssetManager.Models
{
    public class AssetMetadata
    {
        // Basic Metadata
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; } // e.g., "Texture", "3D Model", "Audio"
        public long FileSize { get; set; }
        public string Format { get; set; }
        //public DateTime DateCreated { get; set; }
        // public DateTime DateModified { get; set; }

        public AssetMetadata()
        {
            Name = "Not defined";
            FilePath = "Not defined";
            FileType = "Not defined";
            FileSize = 0;
            Format = "Not defined";
        }

        //// Asset-Specific Metadata

        //// 3D Models
        //public int? PolygonCount { get; set; }    // Nullable if not a 3D model
        //public List<string> TextureDependencies { get; set; }    // Only for 3D models with textures

        //// Textures
        //public int? ResolutionWidth { get; set; } // Nullable if not a texture
        //public int? ResolutionHeight { get; set; }
        //public bool? HasAlphaChannel { get; set; }

        //// Audio
        //public TimeSpan? Duration { get; set; }   // Nullable if not an audio file
        //public int? Bitrate { get; set; }
        //public int? SampleRate { get; set; }
        //public string AudioChannels { get; set; } // e.g., "Stereo", "Mono"

        //// Categorization
        //public List<string> Tags { get; set; }    // Tags or keywords to describe the asset
        //public string Category { get; set; }      // Asset category: "Textures", "Models", etc.
        //public string Theme { get; set; }         // Theme such as "Sci-fi", "Fantasy"

        //// Licensing and Usage
        //public string LicenseType { get; set; }   // License type, e.g., "Creative Commons"
        //public string UsageRights { get; set; }   // Rights and restrictions (e.g., "Non-commercial use")
        //public string Author { get; set; }        // Name of the creator or copyright holder

        //// Versioning Information
        //public string Version { get; set; }       // Version number
        //public string Changelog { get; set; }     // Description of what changed in this version
        //public string LastModifiedBy { get; set; } // Who last modified the asset

        // Project-Specific Metadata
        // public string AssignedToProject { get; set; }  // Which project or scene the asset is part of
        //  public bool InUse { get; set; }               // Is this asset currently in use?

        // Custom Fields
        // public Dictionary<string, string> CustomFields { get; set; }  // Any user-defined metadata fields

        // Constructor to initialize the lists

        //TextureDependencies = new List<string>();
        //Tags = new List<string>();
        //CustomFields = new Dictionary<string, string>();

    }
}
