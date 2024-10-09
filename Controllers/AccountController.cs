using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Account;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
  [Route("api/account")]
  [ApiController]
  public class AccountController : ControllerBase
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly ApplicationDBcontext _context;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    public AccountController(UserManager<AppUser> userManager, ApplicationDBcontext context, SignInManager<AppUser> signInManager, ITokenService tokenService, IEmailService emailService)
    {
      _userManager = userManager;
      _context = context;
      _signInManager = signInManager;
      _tokenService = tokenService;
      _emailService = emailService;
    }

    // Registering User
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
      try
      {
        if (!ModelState.IsValid)
          return BadRequest(ModelState);

        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
          return BadRequest(new { message = "Email is already taken." });

        var appUser = new AppUser
        {
          UserName = registerDto.Username,
          PhoneNumber = registerDto.PhoneNumber,
          Email = registerDto.Email,
        };

        var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

        if (createdUser.Succeeded)
        {
          var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
          if (roleResult.Succeeded)
          {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
            if (!string.IsNullOrEmpty(token))
            {
              var confirmationLink = Url.Action("ConfirmEmail", "Account",
                  new { userId = appUser.Id, token = token }, Request.Scheme);
              await _emailService.SendEmailConfirmationAsync(appUser.Email, confirmationLink);

            }

            return Ok(
                new NewUserDto
                {
                  UserName = appUser.UserName,
                  Email = appUser.Email,
                  Token = _tokenService.CreateToken(appUser)
                }
            );
          }
          else
          {
            await _userManager.DeleteAsync(appUser);
            return StatusCode(500, roleResult.Errors);
          }
        }
        else
        {
          return StatusCode(500, createdUser.Errors);
        }
      }
      catch (Exception e)
      {
        return StatusCode(500, new { e.Message });
      }
    }

    // Login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

      if (user == null) return Unauthorized("Invalid credentials");

      var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

      if (!result.Succeeded)
      {
        return Unauthorized("Username not found and/or password incorrect");
      }

      return Ok(
          new NewUserDto
          {
            UserName = user.UserName,
            Email = user.Email,
            UserId = user.Id,
            Token = _tokenService.CreateToken(user)
          }
      );
    }

    // Forgot Password
    [HttpPost("forgotPassword")]
    public async Task<IActionResult> ForgotPassword(ForgotLoginDetailsDto forgotLoginDetailsDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var user = await _userManager.FindByNameAsync(forgotLoginDetailsDto.Email);
      if (user == null)
        return NotFound("User with the given Username does not exist");

      // Generate a password reset token
      var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
      var encodedToken = Uri.EscapeDataString(resetToken);

      await _userManager.UpdateAsync(user);

      var resetLink = $"http://localhost:5173/reset-password?userId={user.Id}&token={encodedToken}";

      await _emailService.SendEmailAsync(user.Email, "Reset Password Fruit & Veg",
      $"<p>A password change has been requested for your account. If this was you, please use the link below to reset your password.</p> <a style='color: #ffffff;background-color:#2187AB;padding: 10px 20px;text-decoration: none;border-radius: 5px;font-family:Arial,sans-serif;' href='{resetLink}'>Reset Password</a><br></br>");
      Console.WriteLine(encodedToken);

      return Ok($"Password reset link has been sent to your email, {forgotLoginDetailsDto.Email}, {resetToken}, new:{resetLink}.");
    }

    // Forgot UserName or Password
    [HttpPost("forgotLoginDetails")]
    public async Task<IActionResult> ForgotLoginDetails(ForgotLoginDetailsDto forgotLoginDetailsDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var user = await _userManager.FindByEmailAsync(forgotLoginDetailsDto.Email);
      if (user == null)
        return NotFound("User with the given email does not exist");

      // Generate a password reset token
      var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
      var encodedToken = Uri.EscapeDataString(resetToken);

      await _userManager.UpdateAsync(user);

      var resetLink = $"http://localhost:5173/reset-password?userId={user.Id}&token={encodedToken}";

      await _emailService.SendEmailAsync(user.Email, "Reset Password Doctor Booking App",
      "<p>A password change has been requested for your account. If this was you, please use the link below to reset your password.</p> <a style='color: #ffffff;background-color:#2187AB;padding: 10px 20px;text-decoration: none;border-radius: 5px;font-family:Arial,sans-serif;' href='{resetLink}'>Reset Password</a><br></br>");
      Console.WriteLine(encodedToken);

      return Ok($"Password reset link has been sent to your email.");
    }

    // Reset Password
    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var user = await _userManager.FindByIdAsync(resetPasswordDto.UserId);
      if (user == null)
        return NotFound("User does not exist");

      // Check if the token is valid and not expired
      if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
        return BadRequest("Invalid or expired token");
      Console.WriteLine(resetPasswordDto.Token);

      // Check if the new password is different from the current password
      var passwordCheck = await _userManager.CheckPasswordAsync(user, resetPasswordDto.NewPassword);
      if (passwordCheck)
        return BadRequest("New password cannot be the same as the old password.");

      // Reset the password
      var resetResult = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

      if (!resetResult.Succeeded)
      {
        foreach (var error in resetResult.Errors)
        {
          Console.WriteLine($"Error: {error.Description}");
        }
        return BadRequest(resetResult.Errors);
      }

      // Clear the token from the database
      user.PasswordResetTokenExpiry = null;
      await _userManager.UpdateAsync(user);

      return Ok("Password has been reset successfully");
    }

    // Change Password
    [HttpPost("ChangePassword")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ResetPasswordDto resetPasswordDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == resetPasswordDto.Email.ToLower());
      if (user == null) return NotFound("User with the given email does not exist");

      // Check if the new password is different from the current password
      var passwordCheck = await _userManager.CheckPasswordAsync(user, resetPasswordDto.NewPassword);
      if (passwordCheck)
        return BadRequest("New password cannot be the same as the old password.");

      var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
      var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, resetPasswordDto.NewPassword);

      if (!resetResult.Succeeded)
        return BadRequest(resetResult.Errors);

      return Ok("Password has been changed successfully");
    }
  }
}