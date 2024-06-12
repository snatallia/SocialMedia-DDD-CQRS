using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands
{
    public class EditCommentCommand: BaseCommand
    {
        public Guid CommantId { get; set; }
        public string Comment { get; set; }
        public string Username { get; set; }
    }
}
