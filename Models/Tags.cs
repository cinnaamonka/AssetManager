﻿using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManager.Models
{
   

    public class Tag
    {
      

        public int Id { get; set; }
        public string Name { get; set; }

        public string Color { get; set; } = "#000000";
        public HashSet<AssetTag> AssetTags { get; set; }

    }
    public class AssetTag
    {
        public int AssetId { get; set; }
        public Asset Asset { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }

    }
}
