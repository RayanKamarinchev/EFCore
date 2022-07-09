using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace MusicHub.Data.Models
{
    public class Album
    {
        public Album()
        {
            Songs = new HashSet<Song>();
        }
        [Key]
        [Required]
        public int Id { get; set; }
        [MaxLength(40)]
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime ReleaseDate { get; set; }
        [NotMapped]
        public decimal Price => Songs.Any() ? Songs.Sum(s=>s.Price) : 0m;
        [ForeignKey(nameof(Producer))]
        public int ProducerId { get; set; }
        public Producer Producer { get; set; }
        public ICollection<Song> Songs { get; set; }
    }
}
