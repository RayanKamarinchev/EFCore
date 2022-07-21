using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookShop.Models.Enums;

namespace BookShop
{
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            var db = new BookShopContext();
            //string command = Console.ReadLine();
            //DbInitializer.ResetDatabase(db);

            //Console.WriteLine(GetBooksByAgeRestriction(db, command));

            //Console.WriteLine(GetGoldenBooks(db));
            //Console.WriteLine(GetBooksByPrice(db));
            //int year = int.Parse(Console.ReadLine());
            //Console.WriteLine(GetBooksNotReleasedIn(db, year));
            //Console.WriteLine(GetBooksByCategory(db, command));
            //Console.WriteLine(GetBooksReleasedBefore(db, command));
            //Console.WriteLine(GetAuthorNamesEndingIn(db, command));
            //Console.WriteLine(GetBookTitlesContaining(db, command));
            //Console.WriteLine(GetBooksByAuthor(db, command));
            //Console.WriteLine(CountBooks(db, int.Parse(command)));
            //Console.WriteLine(CountCopiesByAuthor(db));
            //Console.WriteLine(GetTotalProfitByCategory(db));
            Console.WriteLine(GetMostRecentBooks(db));
            //Console.WriteLine(RemoveBooks(db));
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder sb = new StringBuilder();
            AgeRestriction ageRestriction;
            bool succes = Enum.TryParse(command, true, out ageRestriction);
            context.Books
                   .Where(b => b.AgeRestriction == ageRestriction)
                   .OrderBy(b => b.Title)
                   .Select(b => b.Title)
                   .ToList()
                   .ForEach(b => sb.AppendLine(b));
            return sb.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            context.Books
                   .Where(b => b.Copies < 5000 && b.EditionType == EditionType.Gold)
                   .OrderBy(b => b.BookId)
                   .Select(b => b.Title)
                   .ToList()
                   .ForEach(b => sb.AppendLine(b));
            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            context.Books
                   .Where(b => b.Price > 40)
                   .OrderByDescending(b => b.Price)
                   .Select(b => $"{b.Title} - ${b.Price:f2}")
                   .ToList()
                   .ForEach(b => sb.AppendLine(b));
            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder sb = new StringBuilder();
            context.Books
                   .Where(b => b.ReleaseDate.Value.Year != year)
                   .OrderBy(b => b.BookId)
                   .Select(b => b.Title)
                   .ToList()
                   .ForEach(b => sb.AppendLine(b));
            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();
            List<string> categories = input.Split(" ").ToList();
            categories.ForEach(c => c.ToLower());
            categories.ForEach(c => string.Concat(c[0].ToString().ToUpper(), c.AsSpan(1)));
            context.Books
                   .Where(b => b.BookCategories.Any(item => categories.Contains(item.Category.Name)))
                   .OrderBy(b => b.Title)
                   .Select(b => b.Title)
                   .ToList()
                   .ForEach(b => sb.AppendLine(b));
            return sb.ToString().TrimEnd();
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder sb = new StringBuilder();
            DateTime st = DateTime.ParseExact(date, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
            context.Books
                   .Where(b => b.ReleaseDate.Value < st)
                   .OrderByDescending(b => b.ReleaseDate)
                   .Select(b => $"{b.Title} - {b.EditionType.ToString()} - ${b.Price:f2}")
                   .ToList()
                   .ForEach(b => sb.AppendLine(b));
            return sb.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();
            context.Authors
                   .Where(a => a.FirstName.EndsWith(input))
                   .Select(a => a.FirstName + " " + a.LastName)
                   .ToList()
                   .ForEach(a => sb.AppendLine(a));
            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();
            context.Books
                   .Where(b => b.Author.LastName.StartsWith(input))
                   .OrderBy(b => b.BookId)
                   .Select(b => $"{b.Title} ({b.Author.FirstName} {b.Author.LastName})")
                   .ToList()
                   .ForEach(b => sb.AppendLine(b));
            return sb.ToString().TrimEnd();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();
            context.Books
                   .Where(b => b.Title.Contains(input))
                   .Select(b => b.Title)
                   .ToList()
                   .ForEach(b => sb.AppendLine(b));
            return sb.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books.Count(b => b.Title.Length > lengthCheck);
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            context.Authors
                   .Select(a => new
                   {
                       a.FirstName,
                       a.LastName,
                       copies = a.Books.Sum(b => b.Copies)
                   })
                   .OrderByDescending(a => a.copies)
                   .ToList()
                   .ForEach(a => sb.AppendLine($"{a.FirstName} {a.LastName} - {a.copies}"));
            return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            context.Categories
                   .Select(c => new
                   {
                       c.Name,
                       profit = c.CategoryBooks.Sum(b => b.Book.Price * b.Book.Copies)
                   })
                   .OrderByDescending(a => a.profit)
                   .ToList()
                   .ForEach(a => sb.AppendLine($"{a.Name} ${a.profit:f2}"));
            return sb.ToString().TrimEnd();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var output = context.Categories
                                .Select(c => new
                                {
                                    c.Name,
                                    books = c.CategoryBooks
                                             .OrderByDescending(b => b.Book.ReleaseDate)
                                             .Take(3)
                                             .ToList()
                                })
                                .OrderBy(c => c.Name)
                                .ToList();
            foreach (var category in output)
            {
                sb.AppendLine($"--{category.Name}");
                foreach (var book in category.books)
                {
                    if (book.Book.ReleaseDate != null)
                        sb.AppendLine($"{book.Book.Title} ({book.Book.ReleaseDate.Value.Year})");
                }
            }
            return sb.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            context.Books
                   .Where(b => b.ReleaseDate.Value.Year < 2010)
                   .ToList()
                   .ForEach(b => b.Price += 5);
            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books.Where(b => b.Copies < 4200);
            int n = books.Count();
            foreach (var book in books)
            {
                context.Books.Remove(book);
            }
            context.SaveChanges();
            return n;
        }
    }
}