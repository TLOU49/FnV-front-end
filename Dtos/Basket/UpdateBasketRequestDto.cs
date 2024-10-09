using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Basket
{
  public class UpdateBasketRequestDto
  {
    [Required]
    public string Description { get; set; }
    [Required]
    public string Image { get; set; }
    [Required]
    public string UnitPrice { get; set; }
    [Required]
    public string Total { get; set; }
    [Required]
    public int Quantity { get; set; }
  }
}