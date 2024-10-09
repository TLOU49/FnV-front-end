using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Basket;
using api.Models;

namespace api.Interfaces
{
  public interface IBasketRepository
  {
    Task<List<Basket>> GetByUserIdAsync(string id);
    Task<Basket> CreateAsync(Basket basketModel);
    Task<Basket?> UpdateAsync(int id, UpdateBasketRequestDto basketRequestDto);
    Task<Basket?> DeleteAsync(int id);
  }
}