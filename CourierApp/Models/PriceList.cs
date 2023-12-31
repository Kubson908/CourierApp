using System.ComponentModel.DataAnnotations;

namespace CourierAPI.Models;

public class PriceList
{
    [Key]
    public int Id { get; set; }
    [Required]
    public float VerySmallSize { get; set; }
    [Required]
    public float SmallSize { get; set; }
    [Required]
    public float MediumSize { get; set; }
    [Required]
    public float LargeSize { get; set; }
    [Required]
    public float LightWeight { get; set; }
    [Required]
    public float MediumWeight { get; set; }
    [Required]
    public float HeavyWeight { get; set; }
}
