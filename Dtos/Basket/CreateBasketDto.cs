using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Basket
{
  public class CreateBasketDto
  {
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public string Image { get; set; } = string.Empty;
    [Required]
    public string UnitPrice { get; set; } = string.Empty;
    [Required]
    public string Total { get; set; } = string.Empty;
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }
  }
}