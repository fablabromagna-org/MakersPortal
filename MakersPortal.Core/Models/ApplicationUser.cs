using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MakersPortal.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required] [ProtectedPersonalData] public string Surname { get; set; }

        [Required] [ProtectedPersonalData] public string GivenName { get; set; }

        [NotMapped] [ProtectedPersonalData] public string CommonName => $"{GivenName} {Surname}";
    }
}