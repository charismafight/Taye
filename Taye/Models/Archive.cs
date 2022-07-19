using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Taye.Enums;

namespace Taye.Models
{
    public class Archive : BaseModel
    {
        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        public UploadFile MediaFile { get; set; }

        public DateTime PubDate { get; set; }

        public DateTime? ShootDate { get; set; }

        public IdentityUser<int> Author { get; set; }
    }
}
