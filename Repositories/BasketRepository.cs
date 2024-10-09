using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Basket;
using api.Interfaces;
using api.Models;

namespace api.Repositories
{
  public class BasketRepository : IBasketRepository
  {
    private readonly ApplicationDBcontext _context;

    public BasketRepository(ApplicationDBcontext context)
    {
      _context = context;
    }
    // Create
    public async Task<Basket> CreateAsync(Basket basketModel)
    {
      try
      {
        await _context.Baskets.AddAsync(basketModel);
        await _context.SaveChangesAsync();
        return basketModel;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    // Delete
    public Task<Basket?> DeleteAsync(int id)
    {
      throw new NotImplementedException();
    }

    // GetByUserId
    public Task<List<Basket>> GetByUserIdAsync(string id)
    {
      throw new NotImplementedException();
    }

    // Update
    public Task<Basket?> UpdateAsync(int id, UpdateBasketRequestDto basketRequestDto)
    {
      throw new NotImplementedException();
    }
  }
}