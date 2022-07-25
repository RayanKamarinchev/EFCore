using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO.Cars;
using CarDealer.DTO.Parts;
using CarDealer.DTO.Suppliers;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile(typeof(CarDealerProfile)));
            CarDealerContext db = new CarDealerContext();
            string json = File.ReadAllText("../../../Datasets/suppliers.json");
            Console.WriteLine(ImportParts(db, json));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            ImportSuppliersDto[] userDtos = JsonConvert.DeserializeObject<ImportSuppliersDto[]>(inputJson);

            ICollection<Supplier> validUsers = new List<Supplier>();
            foreach (ImportSuppliersDto uDto in userDtos)
            {
                if (!IsValid(uDto))
                {
                    continue;
                }

                Supplier user = Mapper.Map<Supplier>(uDto);
                validUsers.Add(user);
            }

            context.Suppliers.AddRange(validUsers);
            context.SaveChanges();
            return $"Successfully imported {validUsers.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            //ImportPartDto[] partsDtos = JsonConvert.DeserializeObject<ImportPartDto[]>(inputJson);

            //ICollection<Part> parts = new List<Part>();
            //foreach (ImportPartDto partDto in partsDtos)
            //{
            //    if (IsValid(!IsValid(partDto)))
            //    {
            //        continue;
            //    }

            //    Part part = Mapper.Map<Part>(partDto);
            //    parts.Add(part);
            //}
            //context.Parts.AddRange(parts);
            //context.SaveChanges();
            //return $"Successfully imported {parts.Count}.";

            ImportPartDto[] productDtos = JsonConvert
                .DeserializeObject<ImportPartDto[]>(inputJson);

            ICollection<Part> validProducts = new List<Part>();
            foreach (ImportPartDto pDto in productDtos)
            {
                if (!IsValid(pDto))
                {
                    continue;
                }

                Part product = Mapper.Map<Part>(pDto);
                validProducts.Add(product);
            }

            context.Parts.AddRange(validProducts);
            context.SaveChanges();

            return $"Successfully imported {validProducts.Count}";
        }

        static string ImportCars(CarDealerContext context, string inputJson)
        {
            ImportcarDto[] carsDto = JsonConvert.DeserializeObject<ImportcarDto[]>(inputJson);
            Car[] cars = carsDto.Select(c => new Car()
                                {
                                    Model = c.Model,
                                    Make = c.Make,
                                    TravelledDistance = c.TravelledDistance,
                                    PartCars = new PartCar[c.PartsId.Length]
                                })
                                .ToArray();
            List<PartCar> partCars = new List<PartCar>();
            for (int i = 0; i < cars.Length; i++)
            {
                Car car = cars[i];
                for (int j = 0; j < carsDto[i].PartsId.Length; j++)
                {
                    car.PartCars.Add(new PartCar()
                    {
                        PartId = carsDto[i].PartsId[j],
                        CarId = car.Id
                    });
                }
            }
            context.Cars.AddRange(cars);
            context.PartCars.AddRange(partCars);
            return $"Successfully imported {cars.Length}.";
        }


        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}