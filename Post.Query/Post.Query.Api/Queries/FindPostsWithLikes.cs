using CQRS.Core.Queries;

namespace Post.Query.Api.Queries
{
    public class FindPostsWithLikes: BaseQuery
    {
        public int NumberOfLikes { get; set; }
    }
}
