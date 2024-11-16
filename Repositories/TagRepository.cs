using AssetManager.Models;
using Microsoft.EntityFrameworkCore;
using static AssetManager.AssetHelpers.AssetHelpers;
using System.Windows.Media;
using AssetManager.ViewModels;


namespace AssetManager.Repositories
{

    public class TagRepository
    {
        private readonly AppDbContext _dbContext;

        MainPageVM MainPageVM;  

        public TagRepository(AppDbContext dbContext,MainPageVM mainPageVM)
        {
            _dbContext = dbContext;

            MainPageVM = mainPageVM;


            InitializeTagsAsync().Wait();
        }

        private async Task InitializeTagsAsync()
        {
            if (MainPageVM.SelectedProject != null)
            {
                if (!MainPageVM.SelectedProject.WasInitialized)
                {
                    foreach (var tag in Enum.GetValues(typeof(Tags)))
                    {
                        await AddTag(tag.ToString());
                        MainPageVM.SelectedProject.WasInitialized = true;
                    }
                }
            }
          
        }

        public async Task<Tag> AddTag(string tagName)
        {
            var tag = await _dbContext.Tags.SingleOrDefaultAsync(t => t.Name == tagName)
                      ?? new Tag { Name = tagName };

            Random random = new Random();
            byte r = (byte)random.Next(128, 256);
            byte g = (byte)random.Next(128, 256);
            byte b = (byte)random.Next(128, 256);

            System.Windows.Media.Color brightColor = System.Windows.Media.Color.FromRgb(r, g, b);

            tag.Color = brightColor.ToString();

            if (!await _dbContext.Tags.AnyAsync(t => t.Id == tag.Id))
            {
                _dbContext.Tags.Add(tag);
            }

            await _dbContext.SaveChangesAsync();

            return tag;
        }

        public async Task AddAssetTagAsync(int assetId, string tagName)
        {
            var tag = await AddTag(tagName);

            var assetTag = new AssetTag { AssetId = assetId, TagId = tag.Id };
            if (!await _dbContext.AssetTags.AnyAsync(at => at.AssetId == assetId && at.TagId == tag.Id))
            {
                _dbContext.AssetTags.Add(assetTag);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Tag>> GetAssetTagsAsync(int assetId)
        {
            return await _dbContext.AssetTags
                .Where(at => at.AssetId == assetId)
                .Select(at => at.Tag)
                .ToListAsync();
        }

        public async Task RemoveTagFromAssetAsync(int assetId, string tagName)
        {
            var tag = await _dbContext.Tags.SingleOrDefaultAsync(t => t.Name == tagName);
            if (tag == null) return;

            var assetTag = await _dbContext.AssetTags
                .SingleOrDefaultAsync(at => at.TagId == tag.Id && at.AssetId == assetId);

            if (assetTag != null)
            {
                _dbContext.AssetTags.Remove(assetTag);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveAllTagsAsync()
        {
            _dbContext.Tags.RemoveRange(_dbContext.Tags);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _dbContext.Tags.ToListAsync();
        }
    }
}
