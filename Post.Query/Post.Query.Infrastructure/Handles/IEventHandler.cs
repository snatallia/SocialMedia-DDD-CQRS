using Post.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Handles
{
    public interface IEventHandler
    {
        Task On(PostCreatedEvent postCreatedEvent);
        Task On(MessageUpdatedEvent messageUpdatedEvent);
        Task On(PostLikedEvent postLikedEvent);
        Task On(PostRemovedEvent postRemovedEvent);
        Task On(CommentAddedEvent commentAddedEvent);
        Task On(CommentUpdatedEvent commentUpdatedEvent);
        Task On(CommentRemovedEvent commentRemovedEvent);

    }
}
