using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Product
{
  public class CreateProductDto
  {
    public string Description { get; set; }
    public decimal SalePrice { get; set; }
    public string Category { get; set; }
    public int Quantity { get; set; }
    public IFormFile Image { get; set; }
    public string UserId { get; set; }
  }
}