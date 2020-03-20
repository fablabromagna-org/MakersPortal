using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MakersPortal.Core.Models
{
    public class MakerSpace
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        #region Ef mappings

        public virtual ICollection<Totem> Totems { get; set; }

        #endregion
    }
}