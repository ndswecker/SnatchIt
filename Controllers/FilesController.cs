using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using SnatchItAPI.Data;

namespace SnatchItAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly AzureBlobService _blobService;
        private readonly IConfiguration _config;
        public FilesController(IConfiguration config)
        {
            _blobService = new AzureBlobService(config);
            _config = config;
        }

        [HttpGet("ListAllContainers")]
        public async Task<IActionResult> ListAllContainers()
        {
            var containerDtos = await _blobService.ListBlobContainersAsync();
            return Ok(containerDtos);
        }

        [HttpGet("ListAllBlobs")]
        public async Task<IActionResult> ListAllBlobs()
        {
            try
            {
                var blobUris = await _blobService.ListBlobUrisAsync();
                return Ok(blobUris);
            }
            catch (Exception ex)
            {
                // Log the exception, adjust logging based on your setup
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while attempting to list blobs.");
            }
        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is not selected or empty.");
            }

            using (var stream = file.OpenReadStream())
            {
                var loadedUri = await _blobService.UploadFilesAsync(stream, file.FileName);
                return Ok(loadedUri);
            }
        }

        [HttpGet("filename")]
        public async Task<IActionResult> Download(string filename)
        {
            return Ok();
        }

        [HttpDelete("filename")]
        public async Task<IActionResult> Delete(string filename)
        {
            return Ok();
        }
    }
}