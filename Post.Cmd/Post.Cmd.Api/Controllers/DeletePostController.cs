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
    public class DeletePostController : ControllerBase
    {
        private readonly ILogger<DeletePostController> logger;
        private readonly ICommandDispatcher commandDispatcher;

        public DeletePostController(ILogger<DeletePostController> logger, ICommandDispatcher commandDispatcher)
        {
            this.logger = logger;
            this.commandDispatcher = commandDispatcher;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePostAsync(Guid id, DeletePostCommand command)
        {
            try
            {
                command.Id = id;
                await commandDispatcher.SendAsync(command);
                return Ok(new BaseResponse
                {
                    Message = "Delete post message request completed successfully."
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
                const string SAFE_ERROR_MESSAGE = "Error while processing request to detele the post.";
                logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}
