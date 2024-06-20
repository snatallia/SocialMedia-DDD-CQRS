using Microsoft.Extensions.Logging;
using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Handles
{
    public class EventHandler : IEventHandler
    {
        private readonly IPostRepository postRepository;
        private readonly ICommentRepository commentRepository;

        public EventHandler(IPostRepository postRepository, ICommentRepository commentRepository)
        {
            this.postRepository = postRepository;
            this.commentRepository = commentRepository;
        }
        public async Task On(PostCreatedEvent @event)
        {
            var post = new PostEntity
            {
                PostId = @event.Id,
                DatePosted = @event.DatePosted,
                Author = @event.Author,
                Message = @event.Message
            };
            await postRepository.CreateAsync(post);
        }

        public async Task On(MessageUpdatedEvent @event)
        {
            var post = await postRepository.GetPostByIdAsync(@event.Id);
            if (post == null) return;
            post.Message = @event.Message;
            await postRepository.UpdateAsync(post);
        }

        public async Task On(PostLikedEvent @event)
        {
            var post = await postRepository.GetPostByIdAsync(@event.Id);
            if (post == null) return;
            post.Likes++;
            await postRepository.UpdateAsync(post);
        }

        public async Task On(PostRemovedEvent @event)
        {
            await postRepository.DeleteAsync(@event.Id);
        }

        public async Task On(CommentAddedEvent @event)
        {
            var comment = new CommentEntity
            { 
                CommentId = @event.CommentId,
                CommentDate = @event.CommentDate,
                Username = @event.Username,
                PostId = @event.Id,
                CommentText = @event.Comment,
                Edited =false
            };
            await commentRepository.CreateAsync(comment);
        }

        public async Task On(CommentUpdatedEvent @event)
        {
            var comment = await commentRepository.GetByIdAsync(@event.Id);
            if (comment == null) return;
            comment.CommentText = @event.Comment;
            comment.Edited = true;
            comment.CommentDate =@event.EditDate;
            await commentRepository.UpdateAsync(comment);
        }

        public async Task On(CommentRemovedEvent @event)
        {
            await commentRepository.DeleteAsync(@event.Id);
        }
    }
}
