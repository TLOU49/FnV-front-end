using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Account
{
  public class ResetPasswordDto
  {
    public string Token { get; set; }
    [Required]
    public string NewPassword { get; set; }
    public string UserId { get; set; }
    public string Email { get; set; }

  }
}