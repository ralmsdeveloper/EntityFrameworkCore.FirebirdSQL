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
using System.Diagnostics;
using System.IO;
using System.Linq; 
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FB = FirebirdSql.Data.FirebirdClient;

namespace EFCore.FirebirdSql.FunctionalTests
{
    public class ScaffoldingTest
    {
        private TestContext CreateContext() => new TestContext("ScaffoldingSample.fdb");
        private string WorkingDir => Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "EFCore.FirerbirdSql.ScaffoldTest");
        private string ClassDir => Path.Combine(WorkingDir, "Scaffolded");

        private int RunEfScaffold(string connStr)
        {
            if (Directory.Exists(ClassDir))Directory.Delete(ClassDir, recursive : true);

            var cmd = $"ef dbcontext scaffold {connStr} \"EntityFrameworkCore.FirebirdSQL\" -o {ClassDir} -c TestContext --force";
            var p = new Process()
            {
                StartInfo = new ProcessStartInfo("dotnet", cmd)
                {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = WorkingDir
                }
            };
            p.Start();
            p.WaitForExit();

            var errStr = p.StandardError.ReadToEnd();
            var outStr = p.StandardOutput.ReadToEnd();

            Console.WriteLine(outStr);
            Console.WriteLine(errStr);

            return p.ExitCode;
        }

        private FileContent GetEntityMap<T>()
            => GetEntityMap(typeof(T).Name);

        private FileContent GetEntityMap(string className)
        {
            var ctxFile = File.ReadAllLines(Path.Combine(ClassDir, "TestContext.cs"));

            var entityMap = new List<string>();
            int? startIdx = null;
            for (var i = 0; i < ctxFile.Length; i++)
            {
                var line = ctxFile[i];

                if (line.Contains($"modelBuilder.Entity<{className}>"))
                {
                    startIdx = i;
                }
                if (startIdx != null)
                {
                    entityMap.Add(line);
                }
                if (startIdx != null && i > startIdx && line.Contains("modelBuilder.Entity"))
                {
                    break;
                }
            }

            if (entityMap.Count == 0)
            {
                throw new Exception($"Entity mapping for class '{className}' not found.");
            }
            return new FileContent(entityMap);
        }

        private FileContent GetEntity<T>() => GetEntity(typeof(T).Name);
        private FileContent GetEntity(string className) => new FileContent(File.ReadAllLines(Path.Combine(ClassDir, $"{className}.cs")));

        [Fact]
        public void Scaffold_db()
        {
            string connStr;
            using(var context = CreateContext())
            {
                connStr = context.Database.GetDbConnection().ConnectionString;
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // commputed index will have NULL as RBS$FIELD_NAME
                context.Database.ExecuteSqlCommand(@"CREATE INDEX ""IX_AUTHOR_COMPUTED"" ON ""Author"" COMPUTED BY (""AuthorId"")");
                // tables with no primary key are valid
                context.Database.ExecuteSqlCommand(@"CREATE TABLE CourseTemplate(Tile varchar(100))");
            }

            var scaffoldExitCode = RunEfScaffold(connStr);

            Assert.Equal(0, scaffoldExitCode);
            Assert.True(Directory.Exists(ClassDir));
            
            GetEntityMap<BookAuthor>().ShouldContain(new Regex(@"HasKey\(e => new { (e.BookId|e.AuthorId), (e.BookId|e.AuthorId) }\)"), "BookAuthor should have composite key");
            GetEntity<BookAuthor>().ShouldNotContain("long? BookId");
            GetEntity<BookAuthor>().ShouldContain("public long BookId");
            GetEntity<Author>().ShouldContain("public byte[] TestBytes", "byte array should be properly mapped");
            //GetEntity("CourseTemplate").ShouldContain("public string Title");
        }

        class FileContent
        {
            public IEnumerable<string> Lines { get; set; }

            public FileContent(IEnumerable<string> lines)
            {
                Lines = lines;
            }

            public void ShouldNotContain(string text, string message = null)
                => Assert.False(Lines.Any(l => l.Contains(text)), message);

            public void ShouldContain(string text, string message = null)
                => Assert.True(Lines.Any(l => l.Contains(text)), message);

            public void ShouldContain(Regex regex, string message = null)
                => Assert.True(Lines.Any(l => regex.IsMatch(l)), message);
        }
    }
}
