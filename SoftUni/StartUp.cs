using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext sc = new SoftUniContext();
            //Console.WriteLine(GetEmployeesFullInformation(sc));
            //Console.WriteLine(GetEmployeesWithSalaryOver50000(sc));
            Console.WriteLine(GetEmployeesFromResearchAndDevelopment(sc));
            //Console.WriteLine(AddNewAddressToEmployee(sc));
            //Console.WriteLine(GetEmployeesInPeriod(sc));
            //Console.WriteLine(GetAddressesByTown(sc));
            //Console.WriteLine(GetEmployee147(sc));
            //Console.WriteLine(GetDepartmentsWithMoreThan5Employees(sc));
            //Console.WriteLine(GetLatestProjects(sc));
            //Console.WriteLine(IncreaseSalaries(sc));
            //Console.WriteLine(DeleteProjectById(sc));
            //Console.WriteLine(RemoveTown(sc));
            //Console.WriteLine(GetEmployeesByFirstNameStartingWithSa(sc));
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            context.Employees
                   .ToArray()
                   .OrderBy(e => e.EmployeeId)
                   .ToList()
                   .ForEach(e => sb.AppendLine(
                                $"{e.FirstName} {e.LastName} {(e.MiddleName == null ? "" : e.MiddleName + " ")}{e.JobTitle} {e.Salary:f2}"));
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            context.Employees
                   .ToArray()
                   .Where(e => e.Salary > 50000)
                   .OrderBy(e => e.FirstName)
                   .ToList()
                   .ForEach(e => sb.AppendLine(
                                $"{e.FirstName} - {e.Salary:f2}"));
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            context.Employees
                   .Where(e => e.Department.Name == "Research and Development")
                   .OrderBy(e => e.Salary)
                   .ThenByDescending(e => e.FirstName)
                   .ToList()
                   .ForEach(e => sb.AppendLine(
                                $"{e.FirstName} {e.LastName} from Research and Development - ${e.Salary:f2}"));
            return sb.ToString().TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            context.Employees
                   .First(e => e.LastName == "Nakov")
                   .Address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };
            context.SaveChangesAsync();
            StringBuilder sb = new StringBuilder();
            context.Addresses
                   .OrderByDescending(a => a.AddressId)
                   .Take(10)
                   .ToList()
                   .ForEach(a => sb.AppendLine(a.AddressText));
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var output = context.Employees
                                .Where(e => e.EmployeesProjects
                                             .Any(ep => ep.Project.StartDate.Year >= 2001
                                                     && ep.Project.StartDate.Year <= 2003))
                                .Take(10)
                                .Select(e => new
                                {
                                    e.FirstName,
                                    e.LastName,
                                    mngF = e.Manager.FirstName,
                                    mngL = e.Manager.LastName,
                                    projects = e.EmployeesProjects
                                                .Select(ep => new
                                                {
                                                    Name = ep.Project.Name,
                                                    Start = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
                                                    End = ep.Project.EndDate.HasValue
                                                        ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt")
                                                        : "not finished"
                                                })
                                });
            foreach (var e in output)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.mngF} {e.mngL}");
                foreach (var project in e.projects)
                {
                    sb.AppendLine($"--{project.Name} - {project.Start} - {project.End}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            context.Addresses
                   .Include(a => a.Town)
                   .Include(a => a.Employees)
                   .OrderByDescending(a => a.Employees.Count)
                   .ThenBy(a => a.Town.Name)
                   .ThenBy(a => a.AddressText)
                   .Take(10)
                   .ToList()
                   .ForEach(a => sb.AppendLine($"{a.AddressText}, {a.Town.Name} - {a.Employees.Count} employees"));
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            Employee employee
                = context.Employees
                         .Include(e => e.EmployeesProjects)
                         .FirstOrDefault(e => e.EmployeeId == 147);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
            context.EmployeesProjects
                   .Include(ep => ep.Project)
                   .Where(ep => ep.EmployeeId == 147)
                   .OrderBy(ep => ep.Project.Name)
                   .ToList()
                   .ForEach(ep => sb.AppendLine(ep.Project.Name));
            return sb.ToString().TrimEnd();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var output = context.Departments
                                .Include(d => d.Employees)
                                .Include(d => d.Manager)
                                .Where(d => d.Employees.Count > 5)
                                .OrderBy(d => d.Employees.Count)
                                .ThenBy(d => d.Name)
                                .Select(d => new
                                {
                                    d.Name,
                                    d.Manager.FirstName,
                                    d.Manager.LastName,
                                    employees = d.Employees.Select(e => new
                                    {
                                        eFirstName = e.FirstName,
                                        eLastName = e.LastName,
                                        eJobTitle = e.JobTitle
                                    })
                                });
            foreach (var department in output)
            {
                sb.AppendLine($"{department.Name} - {department.FirstName} {department.LastName}");
                foreach (var employee in department.employees)
                {
                    sb.AppendLine($"{employee.eFirstName} {employee.eLastName} - {employee.eJobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            context.Projects
                   .OrderByDescending(p => p.StartDate)
                   .Take(10)
                   .OrderBy(p => p.Name)
                   .ToList()
                   .ForEach(p => sb.AppendLine(p.Name + "\n"
                                                      + p.Description + "\n"
                                                      + p.StartDate.ToString("M/d/yyyy h:mm:ss tt")));
            return sb.ToString().TrimEnd();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            context.Employees
                   .Where(e => e.Department.Name == "Engineering"
                            || e.Department.Name == "Tool Design"
                            || e.Department.Name == "Marketing"
                            || e.Department.Name == "Information Services ")
                   .ToList()
                   .ForEach(e => e.Salary *= 1.12M);
            context.SaveChanges();

            context.Employees
                   .Where(e => e.Department.Name == "Engineering"
                            || e.Department.Name == "Tool Design"
                            || e.Department.Name == "Marketing"
                            || e.Department.Name == "Information Services")
                   .OrderBy(e => e.FirstName)
                   .ThenBy(e => e.LastName)
                   .ToList()
                   .ForEach(e => sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})"));
            return sb.ToString().TrimEnd();
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            Project project = context.Projects.Find(2);
            context.EmployeesProjects.Remove(context.EmployeesProjects.Single(ep => ep.Project == project));
            context.Projects.Remove(project);
            context.SaveChanges();

            StringBuilder sb = new StringBuilder();
            context.Projects
                   .Take(10)
                   .ToList()
                   .ForEach(p => sb.AppendLine(p.Name));
            return sb.ToString().TrimEnd();
        }

        public static string RemoveTown(SoftUniContext context)
        {
            Town town = context.Towns.FirstOrDefault(t => t.Name == "Seattle");
            List<Address> addresses = context.Addresses
                   .Where(a => a.TownId == town.TownId)
                   .ToList();

            context.Addresses
                   .Remove(context.Addresses.Single(a => a.TownId == town.TownId));
            context.Employees
                   .Where(e => addresses
                              .Any(a => a.AddressId == e.AddressId))
                   .ToList()
                   .ForEach(e => e.AddressId = null);
            context.Towns.Remove(town);
            context.SaveChanges();
            return $"{addresses.Count} addresses in Seattle were deleted";
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            context.Employees
                   //.Where(e =>
                   //           String.Equals(e.FirstName.Substring(0, 2), "sa"))
                   .Where(employee => EF.Functions.Like(employee.FirstName, "sa%"))
                   .OrderBy(e=>e.FirstName)
                   .ThenBy(e=>e.LastName)
                   .ToList()
                   .ForEach(e=>sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}" +
                                             $" - (${e.Salary:f2})"));
            return sb.ToString().TrimEnd();
        }
    }
}