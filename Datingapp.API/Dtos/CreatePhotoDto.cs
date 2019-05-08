using System;
using Microsoft.AspNetCore.Http;

namespace Datingapp.API.Dtos
{
    public class CreatePhotoDto
    {
        public CreatePhotoDto()
        {
            DateAdded = DateTime.Now;
        }

        public string Url { get; set; }
        public IFormFile File { get; set;}
        public string Description { get; set; }
        public DateTime  DateAdded { get; set; }

        public string PublicId { get; set; }
    }
}