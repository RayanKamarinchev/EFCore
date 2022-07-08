using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
    public class Game
    {
        public Game()
        {
            PlayerStatistics = new HashSet<PlayerStatistic>();
            Bets = new HashSet<Bet>();
        }

        [Key]
        [Required]
        public int GameId { get; set; }

        [Required]
        public int HomeTeamId { get; set; }

        [ForeignKey(nameof(HomeTeamId))]
        public Team HomeTeam { get; set; }

        [Required]
        public int AwayTeamId { get; set; }

        [ForeignKey(nameof(AwayTeamId))]
        public Team AwayTeam { get; set; }

        [Required]
        public int HomeTeamGoals { get; set; }

        [Required]
        public int AwayTeamGoals { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public decimal HomeTeamBetRate { get; set; }

        [Required]
        public decimal AwayTeamBetRate { get; set; }

        [Required]
        public decimal DrawBetRate { get; set; }

        [Required]
        public Result Result { get; set; } //resee (+ top 3)

        public ICollection<PlayerStatistic> PlayerStatistics { get; set; }
        public ICollection<Bet> Bets { get; set; }
    }
}