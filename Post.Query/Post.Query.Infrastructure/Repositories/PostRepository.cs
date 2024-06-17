using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly DatabaseContextFactory contextFactory;

        public PostRepository(DatabaseContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task CreateAsync(PostEntity post)
        {
            using DatabaseContext dbContext = contextFactory.CreateDbContext();
            dbContext.Posts.Add(post);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid postId)
        {
            using DatabaseContext dbContext = contextFactory.CreateDbContext();
            var post = await GetPostByIdAsync(postId);
            if (post == null) return;
          
            dbContext.Posts.Remove(post);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<PostEntity>> GetAllAsync()
        {
            using DatabaseContext dbContext = contextFactory.CreateDbContext();
            return await dbContext.Posts
                .Include(p => p.Comments)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<PostEntity>> GetAllByAuthorAsync(string author)
        {
            using DatabaseContext dbContext = contextFactory.CreateDbContext();
            return await dbContext.Posts
                .Where(p => p.Author == author)
                .Include(p => p.Comments)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PostEntity> GetPostByIdAsync(Guid postId)
        {
            using DatabaseContext dbContext = contextFactory.CreateDbContext();
            return await dbContext.Posts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(x => x.PostId == postId);
        }

        public async Task<List<PostEntity>> GetWithCommentsAsync()
        {
            using DatabaseContext dbContext = contextFactory.CreateDbContext();
            return await dbContext.Posts
                .Where(p=>p.Comments != null && p.Comments.Any())
                .Include(p => p.Comments)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<PostEntity>> GetWithLikesAsync(int numberOfLikes)
        {
            using DatabaseContext dbContext = contextFactory.CreateDbContext();
            return await dbContext.Posts
                .Where(p => p.Likes >= numberOfLikes)
                .Include(p => p.Comments)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateAsync(PostEntity post)
        {
            using DatabaseContext dbContext = contextFactory.CreateDbContext();
            dbContext.Posts.Update(post);
            await dbContext.SaveChangesAsync();
        }
    }
}
