using System.ComponentModel.DataAnnotations;

namespace MakersPortal.Core.Models.Activities
{
    public class Attendance : DescriptiveActivity
    {
        [Required]
        public int StartTotemId { get; set; }
        
        public int? EndTotemId { get; set; }
        
        #region Ef mappings
        
        public virtual Totem StartTotem { get; set; }
        
        public virtual Totem EndTotem { get; set; }
        
        #endregion
    }
}