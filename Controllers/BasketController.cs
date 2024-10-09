using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Basket;
using api.Extensions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
  [Route("api/basket")]
  [ApiController]
  public class BasketController : ControllerBase
  {
    private readonly IBasketRepository _basketRepo;
    private readonly UserManager<AppUser> _userManager;

    public BasketController(IBasketRepository basketRepo, UserManager<AppUser> userManager)
    {
      _basketRepo = basketRepo;
      _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBasketDto basketDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var username = User.GetUsername();
      var appUser = await _userManager.FindByNameAsync(username);

      if (appUser == null)
      {
        return Unauthorized("User not found");
      }

      var basketModel = basketDto.ToBasketFromCreateDTO();
      basketModel.UserId = appUser.Id;

      try
      {
        await _basketRepo.CreateAsync(basketModel);
        return Ok(basketModel);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, "Error creating basket");
      }
    }
  }
}