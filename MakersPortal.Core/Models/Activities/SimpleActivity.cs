using System.ComponentModel.DataAnnotations;
using MakersPortal.Core.Enums;

namespace MakersPortal.Core.Models.Activities
{
    public class SimpleActivity : DescriptiveActivity
    {
        [Required] public SimpleActivityStatus Status { get; set; } = SimpleActivityStatus.WAITING;
    }
}