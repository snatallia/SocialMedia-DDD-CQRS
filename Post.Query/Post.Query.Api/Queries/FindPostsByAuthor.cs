using CQRS.Core.Queries;

namespace Post.Query.Api.Queries
{
    public class FindPostsByAuthor:BaseQuery
    {
        public string Author { get; set; }
    }
}
