using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MakersPortal.Core.Models
{
    public class Totem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public int Name { get; set; }
        
        public int MakerSpaceId { get; set; }
        
        [Required]
        public Guid AuthToken { get; set; }
        
        public DateTime? LastSeen { get; set; }
        
        public Guid? Otp { get; set; }

        #region Ef mappings
        
        public virtual MakerSpace MakerSpace { get; set; }

        #endregion
    }
}