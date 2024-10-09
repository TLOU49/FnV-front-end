using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
  public class Products
  {
    public int Id { get; set; }
    public string Description { get; set; }
    public decimal SalePrice { get; set; }
    public string Category { get; set; }
    public int Quantity { get; set; }
    public string Image { get; set; }
    public string UserId { get; set; }
  }
}