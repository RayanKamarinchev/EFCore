using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ProductShop.DTOs;
using ProductShop.DTOs.Categories;
using ProductShop.DTOs.CategoryProducts;
using ProductShop.DTOs.Products;
using ProductShop.DTOs.Users;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<ImportUserDto, User>();
            CreateMap<ImportProductDto, Product>();
            CreateMap<ImportCategoryDto, Category>();
            CreateMap<ImportCategoryProductDto, CategoryProduct>();
            CreateMap<Product, ExportProductDto>()
                .ForMember(d => d.Seller
                           , m => 
                               m.MapFrom(s => $"{s.Seller.FirstName} {s.Seller.LastName}"));
            CreateMap<Product, ExportUsersProducts>()
                .ForMember(d => d.BuyerFirstName, m => m.MapFrom(s => s.Buyer.FirstName))
                .ForMember(d => d.BuyerLastName, m => m.MapFrom(s => s.Buyer.LastName));
            CreateMap<User, ExportUserWithSoldProducts>()
                .ForMember(d => d.SoldProducts, m => m.MapFrom(s => s.ProductsSold
                                                                     .Where(p=>p.BuyerId.HasValue)));
            CreateMap<Category, ExportCategoriesByProducts>()
                .ForMember(d => d.ProductsCount, m => m.MapFrom(s => s.CategoryProducts.Count))
                .ForMember(d => d.AveragePrice, m => m.MapFrom(s => $"{s.CategoryProducts.Average(c => c.Product.Price):f2}"))
                .ForMember(d => d.TotalRevenue, m => m.MapFrom(s => $"{s.CategoryProducts.Sum(c => c.Product.Price):f2}"));

            CreateMap<User, ExportUserWithSoldProductAdvanced>()
                .ForMember(d => d.Products, m => m.MapFrom(s => s));
            CreateMap<User, ExportProductsList>()
                .ForMember(d => d.Products, m => m.MapFrom(s => s.ProductsSold
                                                                 .Where(p => p.BuyerId.HasValue)));
            CreateMap<Product, ExportSimpleProduct>();
        }
    }
}
