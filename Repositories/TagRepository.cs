using AssetManager.Models;
using Microsoft.EntityFrameworkCore;
using static AssetManager.AssetHelpers.AssetHelpers;
using System.IO ;
using AssetManager.ViewModels;
using System.Diagnostics;


namespace AssetManager.Repositories
{

    public class TagRepository
    {
        private readonly AppDbContext _dbContext;

        MainPageVM MainPageVM;

        public TagRepository(AppDbContext dbContext, MainPageVM mainPageVM)
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

        public async Task<Tag> AddTag(string tagName,string color = null)
        {
            if (tagName == null) return null;

            var tag = await _dbContext.Tags.SingleOrDefaultAsync(t => t.Name == tagName);

            if (tag != null) return null;
            tag ??= new Tag { Name = tagName };

            if (color != null)
            {
                tag.Color = color;
            }
            else
            {
                Random random = new Random();
                byte r = (byte)random.Next(128, 256);
                byte g = (byte)random.Next(128, 256);
                byte b = (byte)random.Next(128, 256);

                System.Windows.Media.Color brightColor = System.Windows.Media.Color.FromRgb(r, g, b);

                tag.Color = brightColor.ToString();
            }

            if (!await _dbContext.Tags.AnyAsync(t => t.Id == tag.Id))
            {
                _dbContext.Tags.Add(tag);
            }

            await _dbContext.SaveChangesAsync();

            return tag;
        }

        public async Task AddAssetTagAsync(int assetId, string tagName, string color = null)
        {
            var tag = await AddTag(tagName);

            if (tag == null)
            {
                tag = await _dbContext.Tags.SingleOrDefaultAsync(t => t.Name == tagName);
            }
            if( color != null)
            {
                tag.Color = color;
            }
          
            var assetTag = new AssetTag { AssetId = assetId, TagId = tag.Id };
          

            if (!await _dbContext.AssetTags.AnyAsync(at => at.AssetId == assetId && at.TagId == tag.Id))
            {
                _dbContext.AssetTags.Add(assetTag);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<AssetTag>> GetAssetTagsAsync(int assetId)
        {
            return await _dbContext.AssetTags
                .Where(at => at.AssetId == assetId)
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

        public async void RemoveTag(string tagName)
        {
            var tag = await _dbContext.Tags.SingleOrDefaultAsync(t => t.Name == tagName);
            if (tag == null) return;

            _dbContext.Tags.Remove(tag);
            await _dbContext.SaveChangesAsync();
        }
    }
}
