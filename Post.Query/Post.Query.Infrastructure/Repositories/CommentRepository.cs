using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
    public class CommentRepository : ICommentRepository
    {
        private readonly DatabaseContextFactory contextFactory;

        public CommentRepository(DatabaseContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task CreateAsync(CommentEntity comment)
        {
            using DatabaseContext dbContext = contextFactory.CreateDbContext();
            dbContext.Comments.Add(comment);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid commentId)
        {
            using DatabaseContext dbContext = contextFactory.CreateDbContext();
            var comment = await GetByIdAsync(commentId);
            if (comment == null) return;

            dbContext.Comments.Remove(comment);
            await dbContext.SaveChangesAsync();
        }

        public async Task<CommentEntity> GetByIdAsync(Guid commentId)
        {
            using DatabaseContext dbContext = contextFactory.CreateDbContext();
            return await dbContext.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
        }

        public async Task UpdateAsync(CommentEntity comment)
        {
            using DatabaseContext dbContext = contextFactory.CreateDbContext();
            dbContext.Update(comment);
            await dbContext.SaveChangesAsync();
        }
    }
}
