using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;

namespace MakersPortal.Core.Dtos
{
    public class JwtTokenDto
    {
        [Required]
        public string Token { get; set; }
    }
}