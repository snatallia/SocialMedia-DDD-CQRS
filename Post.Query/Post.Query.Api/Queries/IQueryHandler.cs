using Post.Query.Domain.Entities;

namespace Post.Query.Api.Queries
{
    public interface IQueryHandler
    {
        Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query);
        Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query);
        Task<List<PostEntity>> HandleAsync(FindPostsByAuthor query);
        Task<List<PostEntity>> HandleAsync(FindPostsWithCommentsQuery query);
        Task<List<PostEntity>> HandleAsync(FindPostsWithLikes query);
    }
}
