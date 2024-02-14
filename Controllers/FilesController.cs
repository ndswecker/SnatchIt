using Azure.Identity;
using Azure.Storage.Blobs;
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

        [HttpGet("Download/{filename}")]
        public async Task<IActionResult> Download(string filename)
        {
            try
            {
                var fileStream = await _blobService.DownloadFileAsync(filename);

                if (fileStream == null)
                {
                    return NotFound();
                }

                // Return the file stream as a file result to download
                return File(fileStream, "application/octet-stream", filename);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while attempting to download the file.");
            }
        }
        

        [HttpDelete("Delete/{filename}")]
        public async Task<IActionResult> Delete(string filename)
        {
            try
            {
                bool isDeleted = await _blobService.DeleteFileAsync(filename);
                if (isDeleted)
                {
                    return Ok($"File {filename} has be successfully deleted. ");
                }
                else{
                    return NotFound($"File {filename} could not be found. Deletion failed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failure to find file {filename}: \n {ex.Message} \n");
                return StatusCode(500, $"Internal error, could not delete {filename}");
            }
        }
    }
}