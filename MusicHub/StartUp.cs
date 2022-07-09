using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MusicHub.Data;
using MusicHub.Data.Models;
using MusicHub.Initializer;

namespace MusicHub
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            MusicHubDbContext context = new MusicHubDbContext();
            DbInitializer.ResetDatabase(context);
            //Console.WriteLine(ExportAlbumsInfo(context, 9));
            Console.WriteLine(ExportSongsAboveDuration(context, 4));
        }

        public class Albums
        {
            public string Name { get; set; }
            public string ReleaseDate { get; set; }
            public string ProducerName { get; set; }
            public decimal AlbumPrice { get; set; }
            public List<Songs> SongsList { get; set; }
        }

        public class Songs
        {
            public string SongName { get; set; }
            public decimal Price { get; set; }
            public string WriterName { get; set; }
        }

        public class Songe
        {
            public string Name { get; set; }
            public string PerformerName { get; set; }
            public string Writer { get; set; }
            public string Producer { get; set; }
            public string Duration { get; set; }
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Albums
                   .Where(a=>a.ProducerId == producerId)
                   .Include(a=>a.Producer)
                   .Include(a=>a.Songs)
                   .ThenInclude(s=>s.Writer)
                   .ToList()
                   .Select(a => new Albums
                   {
                       Name = a.Name,
                       ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy"),
                       ProducerName = a.Producer.Name,
                       AlbumPrice = a.Price,
                       SongsList = a.Songs.Select(s => new Songs
                                {
                                    SongName = s.Name,
                                    Price = s.Price,
                                    WriterName = s.Writer.Name,
                       }).OrderByDescending(s => s.SongName)
                                    .ThenBy(s => s.WriterName)
                                    .ToList()
                   }).OrderByDescending(s => s.AlbumPrice);

            StringBuilder sb = new StringBuilder();
            foreach (var album in albums)
            {
                sb.AppendLine($"-AlbumName: {album.Name}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine($"-Songs:");
                int n = 0;
                foreach (var song in album.SongsList)
                {
                    n++;
                    sb.AppendLine($"---#{n}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.Price:f2}");
                    sb.AppendLine($"---Writer: {song.WriterName}");
                }

                sb.AppendLine($"-AlbumPrice: {album.AlbumPrice:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();
            List<Songe> songs = context.Songs
                                       .Include(s => s.SongPerformers)
                   .ThenInclude(s=>s.Performer)
                   .Include(s=>s.Writer)
                   .Include(s=>s.Album)
                   .ThenInclude(a=>a.Producer)
                   .ToList()
                   .Where(s => s.Duration.TotalSeconds > duration)
                   .Select(s => new Songe() 
                   {
                       Name = s.Name,
                       PerformerName = s.SongPerformers
                                        .Select(sp=>$"{sp.Performer.FirstName} {sp.Performer.LastName}")
                                        .FirstOrDefault(),
                       Writer = s.Writer.Name,
                       Producer = s.Album.Producer.Name,
                       Duration = s.Duration.ToString("c")
                   }).OrderBy(s => s.Name)
                   .ThenBy(s => s.Writer)
                   .ThenBy(s => s.PerformerName)
                   .ToList();
            int n = 0;
            foreach (var song in songs)
            {
                n++;
                sb.AppendLine($"-Song #{n}");
                sb.AppendLine($"---SongName: {song.Name}");
                sb.AppendLine($"---Writer: {song.Writer}");
                sb.AppendLine($"---Performer: {song.PerformerName}");
                sb.AppendLine($"---AlbumProducer: {song.Producer}");
                sb.AppendLine($"---Duration: {song.Duration}");
            }
            return sb.ToString().TrimEnd();
        }
    }
}
