using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Basket
{
  public class BasketDto
  {
    public int Id { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public string UnitPrice { get; set; }
    public string Total { get; set; }
    public int Quantity { get; set; }
    public string UserId { get; set; }
  }
}