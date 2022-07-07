using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
    public class Resource
    {
        [Key]
        [Required]
        public int ResourceId { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        [Required]
        public string Name { get; set; }
        [Column(TypeName = "varchar(max)")]
        [Required]
        public string Url { get; set; }
        [Required]
        [EnumDataType(typeof(ResourceType))]
        public ResourceType ResourceType { get; set; }
        [Required]
        public virtual int CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; }
    }
}