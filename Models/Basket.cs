using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
  [Table("Basket")]
  public class Basket
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