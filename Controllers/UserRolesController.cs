using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
  [Route("api/userRole")]
  public class UserRolesController : ControllerBase
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserRolesController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _roleManager = roleManager;
    }

    [HttpGet]
    [Route("UserRoles")]
    [Authorize]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
      // Get the user by their ID
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
        return NotFound("User not found.");

      var roles = await _userManager.GetRolesAsync(user);

      return Ok(new { user.UserName, Roles = roles });
    }

    [HttpGet("users-in-RoleManager/{roleName}")]
    [Authorize]
    public async Task<IActionResult> GetUsersInRole(string roleName)
    {
      // Confirm the role exists
      var roleExists = await _roleManager.RoleExistsAsync(roleName);
      if (!roleExists)
        return NotFound("Role does not exist.");

      // Get users assigned to the role
      var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

      // Create a simple response listing the usernames
      var usersList = usersInRole.Select(user => new { user.UserName, user.Email }).ToList();

      return Ok(usersList);
    }

    [HttpPost("change-role")]
    [Authorize]
    public async Task<IActionResult> ChangeUserRole(string userId, string newRole)
    {
      // Check if the new role exists in the system
      var roleExists = await _roleManager.RoleExistsAsync(newRole);
      if (!roleExists)
        return BadRequest("The role does not exist.");

      // Find the user by their ID
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
        return NotFound("User not found.");

      // Get the user's current roles
      var currentRoles = await _userManager.GetRolesAsync(user);

      // Remove all current roles
      var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
      if (!removeRolesResult.Succeeded)
        return StatusCode(500, "Error removing current roles.");

      // Add the new role to the user
      var addRoleResult = await _userManager.AddToRoleAsync(user, newRole);
      if (!addRoleResult.Succeeded)
        return StatusCode(500, "Error adding the new role.");

      return Ok($"User {user.UserName} is now assigned to the role {newRole}.");
    }
  }
}