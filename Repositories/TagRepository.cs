using AssetManager.Models;
using Microsoft.EntityFrameworkCore;
using static AssetManager.AssetHelpers.AssetHelpers;

namespace AssetManager.Repositories
{
    internal class TagRepository
    {
        private readonly AppDbContext _dbContext;

        public TagRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;

            AddTag(new Tag { Name = Tags.Image.ToString() });
            AddTag(new Tag { Name = Tags.Model.ToString() });

            _dbContext.SaveChanges();
        }

        public async Task AddTagAsync(int assetId, string tagName)
        {
            var tag = await _dbContext.Tags.SingleOrDefaultAsync(t => t.Name == tagName)
                       ?? new Tag { Name = tagName };

            if (!await _dbContext.Tags.AnyAsync(t => t.TagId == tag.TagId))
            {
                _dbContext.Tags.Add(tag);
            }

       
            await _dbContext.SaveChangesAsync();

       
            var assetTag = new AssetTag { AssetId = assetId, TagId = tag.TagId };
            if (!await _dbContext.AssetTags.AnyAsync(at => at.AssetId == assetId && at.TagId == tag.TagId))
            {
                _dbContext.AssetTags.Add(assetTag);
                await _dbContext.SaveChangesAsync();
            }
        }

        private void AddTag(Tag newTag)
        {
            var tag = _dbContext.Tags.FirstOrDefault(t => t.Name == newTag.Name)
                      ?? new Tag { Name = newTag.Name };

            if (!_dbContext.Tags.Any(t => t.TagId == tag.TagId))
            {
                _dbContext.Tags.Add(tag);
                _dbContext.SaveChanges();
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
                .SingleOrDefaultAsync(at => at.TagId == tag.TagId && at.AssetId == assetId);

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
