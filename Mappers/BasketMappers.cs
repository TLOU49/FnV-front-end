using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Basket;
using api.Models;

namespace api.Mappers
{
  public static class BasketMappers
  {
    public static BasketDto ToBasketDto(this Basket basketModel)
    {
      return new BasketDto
      {
        Id = basketModel.Id,
        Description = basketModel.Description,
        Image = basketModel.Image,
        UnitPrice = basketModel.UnitPrice,
        Quantity = basketModel.Quantity,
        Total = basketModel.Total,
        UserId = basketModel.UserId
      };
    }

    public static Basket ToBasketFromCreateDTO(this CreateBasketDto basketDtoDto)
    {
      return new Basket
      {
        Description = basketDtoDto.Description,
        Image = basketDtoDto.Image,
        UnitPrice = basketDtoDto.UnitPrice,
        Quantity = basketDtoDto.Quantity,
        Total = basketDtoDto.Total
      };
    }
  }
}