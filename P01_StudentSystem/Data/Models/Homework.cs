using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
    public class Homework
    {
        [Key]
        [Required]
        public int HomeworkId { get; set; }
        [Column(TypeName = "varchar(max)")]
        [Required]
        public string Content { get; set; }
        [Required]
        [EnumDataType(typeof(ContentType))]
        public ContentType ContentType { get; set; }
        [Required]
        public DateTime SubmissionTime { get; set; }

        [Required]
        public virtual int StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; }
        [Required]
        public virtual int CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; }
    }
}
