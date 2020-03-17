using System.ComponentModel.DataAnnotations;

namespace MakersPortal.Core.Models.Activities
{
    public abstract class DescriptiveActivity : BaseActivity
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
    }
}