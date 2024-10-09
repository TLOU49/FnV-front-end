using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
  public class UserQueryObject
  {
    public string? Username { get; set; } = null;
    public string? Email { get; set; } = null;
    public string? IDNumber { get; set; } = null;
    public bool IsDescending { get; set; } = false;
  }
}