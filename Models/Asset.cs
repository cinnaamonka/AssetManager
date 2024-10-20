namespace AssetManager.Models
{
    public class Asset
    {
       // public string Id { get; set; }        
        public string FileName { get; set; }       
        public string FilePath { get; set; }  
        public string FileType { get; set; } 
        //public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }

        public AssetMetadata Metadata { get; set; } 

        public Asset(/*string id,*/ string name, string filePath, string fileType)
        {
           //Id = id;
            FileName = name;
            FilePath = filePath;
            FileType = fileType;
            //DateCreated = DateTime.Now;
            //DateModified = DateTime.Now;
            Metadata = new AssetMetadata(); 
        }
    }
}
