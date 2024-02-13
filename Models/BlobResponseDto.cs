using System.Reflection.Metadata;

namespace SnatchItAPI.Models
{
    public class BlobResponseDto
    {
        public BlobResponseDto()
        {
            BlobDto Blob = new BlobDto();
        }
        public string? Status { get; set; }
        public bool Error { get; set; }
        public BlobDto Blob { get; set;}
    }

}