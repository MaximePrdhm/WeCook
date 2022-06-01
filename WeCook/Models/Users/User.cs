using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeCook.Models.Users
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public DateTime BirthDate { get; set; }
        [NotMapped]
        public IFormFile? AvatarFile { get; set; }
        [ValidateNever]
        public string AvatarName { get; set; }
    }
}
