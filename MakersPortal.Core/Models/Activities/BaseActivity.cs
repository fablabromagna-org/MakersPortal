using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MakersPortal.Core.Models.Activities
{
    public abstract class BaseActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set;  }
        
        [Required]
        public DateTime Start { get; set; }
        
        public DateTime? End { get; set; }
        
        #region Ef mappings
        
        public virtual ApplicationUser User { get; set; }

        #endregion
    }
}