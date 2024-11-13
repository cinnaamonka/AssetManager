using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Models
{
    public class Tag
    {
        public int TagId { get; set; }
        public string Name { get; set; }

    }

    public class AssetTag
    {
        public int AssetId { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }

}
