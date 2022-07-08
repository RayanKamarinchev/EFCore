using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
    public class Player
    {
        public Player()
        {
            PlayerStatistics = new HashSet<PlayerStatistic>();
        }
        [Key]
        [Required]
        public int PlayerId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int SquadNumber { get; set; }

        [Required]
        public int TeamId { get; set; }

        [ForeignKey(nameof(TeamId))]
        public Team Team { get; set; }

        [Required]
        public int PositionId { get; set; }
        [ForeignKey(nameof(PositionId))]
        public Position Position { get; set; }

        [Required]
        public bool IsInjured { get; set; }

        public ICollection<PlayerStatistic> PlayerStatistics { get; set; }
    }
}