using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Product;
using api.Models;

namespace api.Mappers
{
  public static class ProductsMappers
  {
    public static ProductDto ToProductDto(this Products productModel)
    {
      return new ProductDto
      {
        Id = productModel.Id,
        Description = productModel.Description,
        Image = productModel.Image,
        SalePrice = productModel.SalePrice,
        Category = productModel.Category,
        Quantity = productModel.Quantity,
        UserId = productModel.UserId
      };
    }

    public static Products ToProductFromCreateDTO(this CreateProductDto productDto, string image)
    {
      return new Products
      {
        Description = productDto.Description,
        Image = image,
        SalePrice = productDto.SalePrice,
        Category = productDto.Category,
        Quantity = productDto.Quantity
      };
    }
  }
}