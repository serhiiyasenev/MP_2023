using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
        public async Task<ActionResult<SendResultModel>> PostMessageToQueue([Required] SendRequestModel message)
        {
            var result = await _queueService.PostMessageAsync(message);
            return Ok(result);
        }

        [HttpPost("PostFileToQueue")]
        [ProducesResponseType(typeof(SendResultModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<SendResultModel>> PostFileToQueue([Required] IFormFile file)
        {
            if (file.Length > 0 && file.FileName.EndsWith(".pdf") && file.Length <= 5 * 1024 * 1024)
            {
                var result = await _queueService.PostFileAsync(file.FileName, file.OpenReadStream());
                return Ok(result);
            }
            return BadRequest("Invalid file.");
        }

        [HttpGet("GetListOfFilesFromQueue")]
        [ProducesResponseType(typeof(List<FileInfoModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<FileInfoModel>>> GetListOfFilesFromQueue()
        {
            var result = await _queueService.GetUploadedFilesInfoAsync();
            return Ok(result);
        }

        [HttpGet("GetFileFromQueueById")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFileFromQueueById([Required] Guid fileId)
        {
            var (FileName, FileContent) = await _queueService.GetFileByIdAsync(fileId.ToString());

            if (FileName != null && FileContent != null)
            {
                return File(FileContent, "application/octet-stream", FileName);
            }
            else
            {
                return NotFound($"File not found by id '{fileId}'");
            }
        }
    }
}