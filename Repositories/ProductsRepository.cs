using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
  public class ProductsRepository : IProductsRepository
  {
    private readonly ApplicationDBcontext _context;

    public ProductsRepository(ApplicationDBcontext context)
    {
      _context = context;
    }

    // Createing a Product
    public async Task<Products> CreateAsync(Products productModel)
    {
      await _context.Products.AddAsync(productModel);
      await _context.SaveChangesAsync();
      return productModel;
    }

    // Delete a Product
    public async Task<Products?> DeleteAsync(int id)
    {
      var productModel = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

      if (productModel == null)
      {
        return null;
      }

      _context.Products.Remove(productModel);
      await _context.SaveChangesAsync();

      return productModel;
    }

    // Get All Products
    public async Task<List<Products>> GetAllAsync()
    {
      var products = _context.Products.AsQueryable();

      return await products.ToListAsync();
    }

    // Get Product by Id
    public async Task<Products?> GetByIDAsync(int id)
    {
      return await _context.Products.FirstOrDefaultAsync(i => i.Id == id);
    }
  }
}