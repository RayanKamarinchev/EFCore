using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs;
using ProductShop.DTOs.Categories;
using ProductShop.DTOs.CategoryProducts;
using ProductShop.DTOs.Products;
using ProductShop.DTOs.Users;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        private static string filePath;
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile(typeof(ProductShopProfile)));
            ProductShopContext db = new ProductShopContext();

            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();

            string fileName = "users-and-products.json";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../Datasets/", fileName);
            File.WriteAllText(filePath, GetUsersWithProducts(db));
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            ImportUserDto[] userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson);

            ICollection<User> validUsers = new List<User>();
            foreach (ImportUserDto uDto in userDtos)
            {
                if (!IsValid(uDto))
                {
                    continue;
                }

                User user = Mapper.Map<User>(uDto);
                validUsers.Add(user);
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();

            return $"Successfully imported {validUsers.Count}";
        }

        //Problem 02
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            ImportProductDto[] productDtos = JsonConvert
                .DeserializeObject<ImportProductDto[]>(inputJson);

            ICollection<Product> validProducts = new List<Product>();
            foreach (ImportProductDto pDto in productDtos)
            {
                if (!IsValid(pDto))
                {
                    continue;
                }

                Product product = Mapper.Map<Product>(pDto);
                validProducts.Add(product);
            }

            context.Products.AddRange(validProducts);
            context.SaveChanges();

            return $"Successfully imported {validProducts.Count}";
        }

        //Problem 03
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            ImportCategoryDto[] categoryDtos = JsonConvert
                .DeserializeObject<ImportCategoryDto[]>(inputJson);

            ICollection<Category> validCategories = new List<Category>();
            foreach (ImportCategoryDto cDto in categoryDtos)
            {
                if (!IsValid(cDto))
                {
                    continue;
                }

                Category category = Mapper.Map<Category>(cDto);
                validCategories.Add(category);
            }

            context.Categories.AddRange(validCategories);
            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }

        //Problem 04
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            ImportCategoryProductDto[] categoryProductDtos = JsonConvert
                .DeserializeObject<ImportCategoryProductDto[]>(inputJson);

            ICollection<CategoryProduct> validCp = new List<CategoryProduct>();
            foreach (ImportCategoryProductDto cpDto in categoryProductDtos)
            {
                //No need of validation
                //TODO: It will be good to check if there are Product and Category existing with given IDs
                CategoryProduct categoryProduct = Mapper.Map<CategoryProduct>(cpDto);
                validCp.Add(categoryProduct);
            }

            context.CategoryProducts.AddRange(validCp);
            context.SaveChanges();

            return $"Successfully imported {validCp.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            List<ExportProductDto> productsDtos = context.Products
                                                         .Where(p => p.Price >= 500 && p.Price <= 1000)
                                                         .OrderBy(p => p.Price)
                                                         .ProjectTo<ExportProductDto>()
                                                         .ToList();
            string json = JsonConvert.SerializeObject(productsDtos, Formatting.Indented);
            return json;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            ExportUserWithSoldProducts[] userDtos
                = context.Users
                         .Where(u => u.ProductsSold.Any(p => p.BuyerId.HasValue))
                         .OrderBy(u => u.LastName)
                         .ThenBy(u => u.FirstName)
                         .ProjectTo<ExportUserWithSoldProducts>()
                         .ToArray();
            string json = JsonConvert.SerializeObject(userDtos, Formatting.Indented);
            return json;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            ExportCategoriesByProducts[] categoriesDtos = context.Categories
                                                                 .OrderByDescending(c => c.CategoryProducts.Count)
                                                                 .ProjectTo<ExportCategoriesByProducts>()
                                                                 .ToArray();
            string json = JsonConvert.SerializeObject(categoriesDtos, Formatting.Indented);
            return json;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            UsersAndProductsDto usersDto
                = new UsersAndProductsDto()
                {
                    Users = context.Users
                                   .Where(u => u.ProductsSold
                                                .Any(p => p.BuyerId.HasValue))
                                   .OrderByDescending(u => u.ProductsSold
                                                            .Where(p => p.BuyerId.HasValue).Count())
                                   //.Select(a => new ExportUserWithSoldProductAdvanced
                                   //{
                                   //    FirstName = a.FirstName,
                                   //    LastName = a.LastName,
                                   //    Age = a.Age,
                                   //    Products = new ExportProductsList()
                                   //    {
                                   //        Products = a.ProductsSold.Where(p => p.BuyerId.HasValue)
                                   //                    .Select(p => new ExportSimpleProduct
                                   //                    {
                                   //                        Name = p.Name,
                                   //                        Price = p.Price
                                   //                    }).ToArray()
                                   //    }
                                   //})
                                   .ProjectTo<ExportUserWithSoldProductAdvanced>()
                                   .ToArray()
                };
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(usersDto, Formatting.Indented, serializerSettings);
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }

        private static void InitializeDatasetFilePath(string fileName)
        {
            filePath =
                Path.Combine(Directory.GetCurrentDirectory(), "../../../Datasets/", fileName);
        }
    }
}