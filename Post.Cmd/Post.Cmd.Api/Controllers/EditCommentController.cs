using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EditCommentController : ControllerBase
    {
        private readonly ILogger<EditCommentController> logger;
        private readonly ICommandDispatcher commandDispatcher;

        public EditCommentController(ILogger<EditCommentController> logger, ICommandDispatcher commandDispatcher)
        {
            this.logger = logger;
            this.commandDispatcher = commandDispatcher;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> EditCommentAsync(Guid id, EditCommentCommand command)
        {
            try
            {
                command.Id = id;
                await commandDispatcher.SendAsync(command);
                return Ok(new BaseResponse
                {
                    Message = "Edit comment request completed successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                logger.Log(LogLevel.Error, ex, "Client made a bad request.");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (AggregateNotFoundException ex)
            {
                logger.Log(LogLevel.Error, ex, "Could not retrieve aggregate, client passed incorrect post ID.");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to edit the comment on a post.";
                logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}
