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

        [Column(TypeName = "nvarchar(50)")]
        [ValidateNever]
        public string AvatarName { get; set; }
    }
}
