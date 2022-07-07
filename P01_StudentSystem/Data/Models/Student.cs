using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
    public class Student
    {
        public Student()
        {
            CourseEnrollments = new HashSet<StudentCourse>();
            HomeworkSubmissions = new HashSet<Homework>();
        }
        [Key]
        [Required]
        public int StudentId { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        [Required]
        public string Name { get; set; }
        [Column(TypeName = "varchar(10)")]
        public string PhoneNumber { get; set; }
        [Required]
        public DateTime RegisteredOn { get; set; }
        public DateTime? Birthday { get; set; }
        public virtual ICollection<StudentCourse> CourseEnrollments { get; set; }

        public virtual ICollection<Homework> HomeworkSubmissions { get; set; }
    }
}