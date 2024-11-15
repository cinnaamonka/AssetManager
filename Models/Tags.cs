namespace AssetManager.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int AssetId { get; set; }

        public Asset Asset { get; set; }

    }
}
