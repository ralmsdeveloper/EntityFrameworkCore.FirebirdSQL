/*
 *          Copyright (c) 2017-2018 Rafael Almeida (ralms@ralms.net)
 *
 *                    EntityFrameworkCore.FirebirdSql
 *
 * THIS MATERIAL IS PROVIDED AS IS, WITH ABSOLUTELY NO WARRANTY EXPRESSED
 * OR IMPLIED.  ANY USE IS AT YOUR OWN RISK.
 * 
 * Permission is hereby granted to use or copy this program
 * for any purpose,  provided the above notices are retained on all copies.
 * Permission to modify the code and to distribute modified code is granted,
 * provided the above notices are retained, and a notice that the code was
 * modified is included with the above copyright notice.
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.FirebirdSql.FunctionalTests
{
    public class Author
    {
        [Key]
        public long AuthorId { get; set; }

        [StringLength(100)]
        public string TestString { get; set; }

        public DateTime TestDate { get; set; }

        public Guid TestGuid { get; set; }

        public byte[] TestBytes { get; set; }

        public int TestInt { get; set; }

        public decimal TestDecimal { get; set; }

        public double TestDouble { get; set; }

        public int? TestIntNullable { get; set; }

        public double? TestdoubleNullable { get; set; }

        //public bool Active { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }

    public class Book
    {
        [Key]
        public long BookId { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        public long AuthorId { get; set; }
        public virtual Author Author { get; set; }
    }

    public class Person
    {
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }
    }

    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseID { get; set; }
        public int Credits { get; set; }
        [StringLength(100)]
        public string Title { get; set; }

        public ICollection<Person> Enrollments { get; set; }
    }
}
