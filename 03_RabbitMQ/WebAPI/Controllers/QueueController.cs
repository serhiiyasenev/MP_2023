using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly IQueueService _queueService;

        public QueueController(IQueueService queueService)
        {
            _queueService = queueService;
        }

        [HttpPost("PostMessageToQueue")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<SendResultModel>> PostMessageToQueue([FromBody] SendRequestModel message)
        {
            var result = await _queueService.PostMessageAsync(message);
            return Ok(result);
        }

        [HttpPost("PostFileToQueue")]
        [ProducesResponseType(typeof(SendResultModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<SendResultModel>> PostFileToQueue([FromForm] IFormFile file)
        {
            if (file.Length > 0 && file.FileName.EndsWith(".pdf"))
            {
                var result = await _queueService.PostFileAsync(file.OpenReadStream());
                return Ok(result);
            }
            return BadRequest("Invalid file.");
        }
    }
}