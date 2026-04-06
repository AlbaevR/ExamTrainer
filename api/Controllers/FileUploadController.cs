using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using api.Services.Interfaces;

namespace api.Controllers;

[ApiController]
[Route("api/upload")]
public class FileUploadController : ControllerBase
{
    private readonly IFileUploadService _fileUploadService;
    private readonly ILogger<FileUploadController> _logger;

    public FileUploadController(IFileUploadService fileUploadService, ILogger<FileUploadController> logger)
    {
        _fileUploadService = fileUploadService;
        _logger = logger;
    }

    [HttpPost("answer-option-image")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadAnswerOptionImage(IFormFile file)
    {
        try
        {
            if (file == null)
                return BadRequest("No file provided");

            var fileUrl = await _fileUploadService.UploadFileAsync(file, "answer-options");

            return Ok(new { url = fileUrl });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            return StatusCode(500, new { error = "An error occurred while uploading the file" });
        }
    }

    [HttpDelete("answer-option-image")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAnswerOptionImage([FromQuery] string fileUrl)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return BadRequest("File URL is required");

            var deleted = await _fileUploadService.DeleteFileAsync(fileUrl);

            if (!deleted)
                return NotFound("File not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file");
            return StatusCode(500, new { error = "An error occurred while deleting the file" });
        }
    }
}
