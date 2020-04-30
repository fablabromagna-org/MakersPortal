using System.ComponentModel.DataAnnotations;

namespace MakersPortal.Core.Dtos
{
    public class JwtTokenDto
    {
        [Required]
        public string Token { get; set; }
    }
}