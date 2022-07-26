using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;

namespace CarDealer
{
    public class StartUp
    {
        
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg=>cfg.AddProfile(typeof(CarDealerProfile)));
            CarDealerContext db = new CarDealerContext();

            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();

            string xml = GetCarsWithTheirListOfParts(db);
            File.WriteAllText("../../../Datasets/cars-and-parts.xml", xml);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            ImportSupplierDto[] supplierDtos = Deserialize<ImportSupplierDto[]>(inputXml, "Suppliers");

            Supplier[] suppliers = supplierDtos
                                   .Select(s => new Supplier
                                   {
                                       Name = s.Name,
                                       IsImporter = s.IsImporter
                                   }).ToArray();
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();
            return $"Successfully imported {suppliers.Length}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            ImportPartDto[] supplierDtos = Deserialize<ImportPartDto[]>(inputXml, "Parts");
            ICollection<Part> parts = new List<Part>();
            foreach (var p in supplierDtos)
            {
                if (!context.Suppliers.Any(s => s.Id == p.SupplierId))
                {
                    continue;
                }

                parts.Add(new Part()
                {
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    SupplierId = p.SupplierId
                });
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {parts.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            ImportCarDto[] carsDtos = Deserialize<ImportCarDto[]>(inputXml, "Cars");
            ICollection<Car> cars = new List<Car>(); 
            foreach (var c in carsDtos)
            {
                Car car = new Car()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TraveledDistance,
                };
                ICollection<PartCar> partCars = new List<PartCar>();
                foreach (int partId in c.Parts.Select(p=>p.Id).Distinct())
                {
                    if (!context.Parts.Any(p=>p.Id == partId))
                    {
                        continue;
                    }
                    partCars.Add(new PartCar()
                    {
                        Car = car,
                        PartId = partId
                    });
                }
                car.PartCars = partCars;
                cars.Add(car);
            }
            context.AddRange(cars);
            context.SaveChanges();
            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            ImportCustomersDto[] customersDtos = Deserialize<ImportCustomersDto[]>(inputXml, "Customers");
            Customer[] customers = customersDtos
                                  .Select(c => new Customer()
                                  {
                                      BirthDate = c.BirthDate,
                                      IsYoungDriver = c.IsYoungDriver,
                                      Name = c.Name
                                  }).ToArray();
            context.AddRange(customers);
            context.SaveChanges();
            return $"Successfully imported {customers.Length}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            ImportSaleDto[] salesDtos = Deserialize<ImportSaleDto[]>(inputXml, "Sales");
            ICollection<Sale> sales = new List<Sale>();
            foreach (var s in salesDtos)
            {
                if (!context.Cars.Any(c=>c.Id == s.CarId))
                {
                    continue;
                }

                sales.Add(new Sale()
                {
                    CarId = s.CarId,
                    CustomerId = s.CustomerId,
                    Discount = s.Discount
                });
            }
            context.AddRange(sales);
            context.SaveChanges();
            return $"Successfully imported {sales.Count}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            ExportCarDistanceDto[] carsDtos =
                context.Cars
                       .Where(c => c.TravelledDistance > 2000000)
                       .OrderBy(c => c.Make)
                       .ThenBy(c => c.Model)
                       .Take(10)
                       .Select(x => new ExportCarDistanceDto()
                       {
                           Make = x.Make,
                           Model = x.Model,
                           TravelledDistance = x.TravelledDistance
                       })
                       .ToArray();
            return Serialize(carsDtos, "cars");
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            ExportCarBMWDto[] carsDtos = 
                context.Cars
                       .Where(c=>c.Make=="BMW")
                       .OrderBy(c=>c.Model)
                       .ThenByDescending(c=>c.TravelledDistance)
                       .Select(x=>new ExportCarBMWDto()
                       {
                           Id = x.Id,
                           Model = x.Model,
                           TravelledDistance = x.TravelledDistance
                       })
                       .ToArray();
            return Serialize(carsDtos, "cars");
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            ExportSupplierLocalDto[] suppliersDtos
                = context.Suppliers
                         .Where(s => !s.IsImporter)
                         .Select(x=> new ExportSupplierLocalDto()
                         {
                             Id = x.Id,
                             Name = x.Name,
                             PartsCount = x.Parts.Count
                         })
                         .ToArray();
            return Serialize(suppliersDtos, "suppliers");
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            ExportCarWithPartsDto[] carsDtos
                = context.Cars
                         .OrderByDescending(c => c.TravelledDistance)
                         .ThenBy(c => c.Model)
                         .Select(x => new ExportCarWithPartsDto()
                         {
                             Make = x.Make,
                             Model = x.Model,
                             TravelledDistance = x.TravelledDistance,
                             Parts = x.PartCars.Select(pc => new OuterPartsFromCarDto()
                                      {
                                          Name = pc.Part.Name,
                                          Price = pc.Part.Price
                                      })
                                      .OrderByDescending(p => p.Price)
                                      .ToArray()
                         })
                         .ToArray();
            return Serialize(carsDtos, "cars");
        }

        //public static string GetTotalSalesByCustomer(CarDealerContext context)
        //{
        //    ExportCustomerSales[] customersDtos
        //        = context.Sales
        //                 .Where(s => s.)
        //                 .Select(c => new ExportCustomerSales()
        //                 {
        //                     FullName = c.Name,
        //                     BoughtCars = c.Sales.Count,
        //                     Car = c.Sales.ToArray()[0].Car
        //                 }).ToArray();
        //    foreach (var customersDto in customersDtos)
        //    {
        //        decimal sum = 0;
        //        foreach (var part in customersDto.Car.PartCars)
        //        {
        //            sum += part.Part.Price;
        //        }

        //        customersDto.SpentMoney = sum;
        //    }
        //    return Serialize(customersDtos, "customers");
        //}

        //Helper
        private static T Deserialize<T>(string inputXml, string rootName)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            using StringReader reader = new StringReader(inputXml);
            T dtos = (T)xmlSerializer
                .Deserialize(reader);

            return dtos;
        }

        private static string Serialize<T>(T dto, string rootName)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            using StringWriter writer = new StringWriter(sb);
            xmlSerializer.Serialize(writer, dto, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}