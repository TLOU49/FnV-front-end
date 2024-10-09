using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Product;
using api.Extensions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
  [Route("api/product")]
  public class ProductsController : ControllerBase
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly ApplicationDBcontext _context;
    private readonly IProductsRepository _productsRepo;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(UserManager<AppUser> userManager, ApplicationDBcontext context, IProductsRepository productsRepo, ILogger<ProductsController> logger)
    {
      _userManager = userManager;
      _context = context;
      _productsRepo = productsRepo;
      _logger = logger;
    }

    // Get By Id
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var product = await _productsRepo.GetByIDAsync(id);

      if (product == null)
        return NotFound();

      return Ok(product.ToProductDto());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateProductDto productDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var fileExtension = Path.GetExtension(productDto.Image.FileName);
      var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

      if (!allowedExtensions.Contains(fileExtension.ToLower()))
      {
        return BadRequest("Unsupported file extension, please upload either jpg, jpeg or png.");
      }

      var fileName = Guid.NewGuid().ToString() + "_" + DateTime.UtcNow.Ticks + fileExtension;
      var bucketName = "deanapp-1";

      try
      {
        using (var stream = new MemoryStream())
        {
          await productDto.Image.CopyToAsync(stream);
          stream.Position = 0;

          var credential = GoogleCredential.FromFile("wwwroot/image-gallery-app-431013-dcba6102e8ac.json");
          var storageClient = StorageClient.Create(credential);

          await storageClient.UploadObjectAsync(bucketName, fileName, null, stream);
        }

        var imageURL = $"https://storage.googleapis.com/{bucketName}/{fileName}";
        var username = User.GetUsername();
        var appUser = await _userManager.FindByNameAsync(username);

        var productModel = productDto.ToProductFromCreateDTO(imageURL);
        productModel.UserId = appUser.Id;

        await _productsRepo.CreateAsync(productModel);

        return CreatedAtAction(nameof(GetById), new { id = productModel.Id }, productModel.ToProductDto());

      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while toggling like for ImageId {ImageId}");

        return StatusCode(500, "Internal server error");
      }
    }
  }
}