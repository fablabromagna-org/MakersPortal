using System.ComponentModel.DataAnnotations;

namespace MakersPortal.Core.Models.Activities
{
    public class SimpleActivity : DescriptiveActivity
    {
        [Required] public SimpleActivityStatus Status { get; set; } = SimpleActivityStatus.WAITING;
    }
}