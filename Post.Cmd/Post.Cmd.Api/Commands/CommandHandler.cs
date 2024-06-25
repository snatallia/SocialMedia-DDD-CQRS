
using CQRS.Core.Handlers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IEventSourcingHandler<PostAggregate> eventSourcingHandler;
       // private readonly 
        public CommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
        {
            this.eventSourcingHandler = eventSourcingHandler;
        }

        public async Task HandleAsync(NewPostCommand command)
        {
            var aggregate = new PostAggregate(command.Id, command.Author,command.Message);
            await eventSourcingHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(EditPostCommand command)
        {
            var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
            aggregate.EditPost(command.Message);
            await eventSourcingHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(LikePostCommand command)
        {
            var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
            aggregate.LikePost();
            await eventSourcingHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(DeletePostCommand command)
        {
            var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
            aggregate.DeletePost(command.Username);
            await eventSourcingHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(AddCommentCommand command)
        {
            var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
            aggregate.AddComment(command.Comment, command.Username);
            await eventSourcingHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(EditCommentCommand command)
        {
            var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
            aggregate.EditComment(command.CommentId, command.Comment, command.Username);
            await eventSourcingHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(RemoveCommentCommand command)
        {
            var aggregate = await eventSourcingHandler.GetByIdAsync(command.Id);
            aggregate.RemoveComment(command.CommentId, command.Username);
            await eventSourcingHandler.SaveAsync(aggregate);
        }
    }
}
