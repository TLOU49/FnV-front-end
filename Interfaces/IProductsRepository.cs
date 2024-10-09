using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
  public interface IProductsRepository
  {
    Task<Products> CreateAsync(Products productModel);
    Task<List<Products>> GetAllAsync();
    Task<Products?> GetByIDAsync(int id);
    Task<Products?> DeleteAsync(int id);
  }
}