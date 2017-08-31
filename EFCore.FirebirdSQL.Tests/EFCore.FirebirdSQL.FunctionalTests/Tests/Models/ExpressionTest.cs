using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FirebirdSql.Data.FirebirdClient;
using SouchProd.EntityFrameworkCore.Firebird.FunctionalTests.Models;
using Xunit;

namespace SouchProd.EntityFrameworkCore.Firebird.FunctionalTests.Tests.Models
{

    public class ExpressionTest : IDisposable
    {

        private readonly AppDb _db;
        private readonly DataTypesSimple _simple;
        private readonly DataTypesSimple _simple2;
        private readonly DataTypesVariable _variable;

        public ExpressionTest()
        {
            _db = new AppDb();

            // initialize simple data types
            _simple = new DataTypesSimple
            {
                TypeDateTime = new DateTime(2017, 1, 1, 0, 0, 0),
                TypeDouble = 3.1415,
                TypeDoubleN = -3.1415
            };
            _db.DataTypesSimple.Add(_simple);

            // initialize simple data types
            _simple2 = new DataTypesSimple
            {
                TypeDouble = 1,
                TypeDoubleN = -1
            };
            _db.DataTypesSimple.Add(_simple2);

            // initialize variable data types
            _variable = DataTypesVariable.CreateEmpty();
            _variable.TypeString = "EntityFramework";

            _db.DataTypesVariable.Add(_variable);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            try
            {
                _db.DataTypesSimple.Remove(_simple);
                _db.DataTypesVariable.Remove(_variable);
                _db.SaveChanges();
            }
            finally
            {
                _db.Dispose();
            }
        }


        [Fact]
        public async Task FirebirdContainsOptimizedTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    Contains = m.TypeString.Contains("Fram"),
                    NotContains = m.TypeString.Contains("asdf")
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);

            Assert.True(result.Contains);
            Assert.False(result.NotContains);
        }

        [Fact]
        public async Task FirebirdDateAddTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    FutureYear = m.TypeDateTime.AddYears(1),
                    FutureMonth = m.TypeDateTime.AddMonths(1),
                    FutureDay = m.TypeDateTime.AddDays(1),
                    FutureHour = m.TypeDateTime.AddHours(1),
                    FutureMinute = m.TypeDateTime.AddMinutes(1),
                    FutureSecond = m.TypeDateTime.AddSeconds(1),
                    PastYear = m.TypeDateTime.AddYears(-1),
                    PastMonth = m.TypeDateTime.AddMonths(-1),
                    PastDay = m.TypeDateTime.AddDays(-1),
                    PastHour = m.TypeDateTime.AddHours(-1),
                    PastMinute = m.TypeDateTime.AddMinutes(-1),
                    PastSecond = m.TypeDateTime.AddSeconds(-1),
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            Assert.Equal(_simple.TypeDateTime.AddYears(1), result.FutureYear);
            Assert.Equal(_simple.TypeDateTime.AddMonths(1), result.FutureMonth);
            Assert.Equal(_simple.TypeDateTime.AddDays(1), result.FutureDay);
            Assert.Equal(_simple.TypeDateTime.AddHours(1), result.FutureHour);
            Assert.Equal(_simple.TypeDateTime.AddMinutes(1), result.FutureMinute);
            Assert.Equal(_simple.TypeDateTime.AddSeconds(1), result.FutureSecond);
            Assert.Equal(_simple.TypeDateTime.AddYears(-1), result.PastYear);
            Assert.Equal(_simple.TypeDateTime.AddMonths(-1), result.PastMonth);
            Assert.Equal(_simple.TypeDateTime.AddDays(-1), result.PastDay);
            Assert.Equal(_simple.TypeDateTime.AddHours(-1), result.PastHour);
            Assert.Equal(_simple.TypeDateTime.AddMinutes(-1), result.PastMinute);
            Assert.Equal(_simple.TypeDateTime.AddSeconds(-1), result.PastSecond);
        }

        [Fact]
        public async Task FirebirdDatePartTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Year = m.TypeDateTime.Year,
                    Month = m.TypeDateTime.Month,
                    Day = m.TypeDateTime.Day,
                    Hour = m.TypeDateTime.Hour,
                    Minute = m.TypeDateTime.Minute,
                    Second = m.TypeDateTime.Second,
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            Assert.Equal(_simple.TypeDateTime.Year, result.Year);
            Assert.Equal(_simple.TypeDateTime.Month, result.Month);
            Assert.Equal(_simple.TypeDateTime.Day, result.Day);
            Assert.Equal(_simple.TypeDateTime.Hour, result.Hour);
            Assert.Equal(_simple.TypeDateTime.Minute, result.Minute);
            Assert.Equal(_simple.TypeDateTime.Second, result.Second);
        }

        [Fact]
        public async Task FirebirdDateTimeNowTranslator()
        {
            await _db.Database.OpenConnectionAsync();
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Now = DateTime.Now
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            _db.Database.CloseConnection();
            Assert.InRange(result.Now, DateTime.Now - TimeSpan.FromSeconds(5), DateTime.Now + TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task FirebirdEndsWithOptimizedTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    EndsWith = m.TypeString.EndsWith("Framework"),
                    NotEndsWith = m.TypeString.EndsWith("Entity")
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);

            Assert.True(result.EndsWith);
            Assert.False(result.NotEndsWith);
        }

        [Fact]
        public async Task FirebirdMathAbsTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    ValDbl = m.TypeDoubleN, 
                    Abs = Math.Abs(m.TypeDoubleN.Value),
                }).FirstOrDefaultAsync(m => (m.Id == _simple.Id) && (m.ValDbl != null));

            Assert.Equal(Math.Abs(_simple.TypeDoubleN.Value), result.Abs);
        }

        [Fact]
        public async Task FirebirdMathCeilingTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Ceiling = Math.Ceiling(m.TypeDouble),
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            Assert.Equal(Math.Ceiling(_simple.TypeDouble), result.Ceiling);
        }

        [Fact]
        public async Task FirebirdMathFloorTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Floor = Math.Floor(m.TypeDouble),
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            Assert.Equal(Math.Floor(_simple.TypeDouble), result.Floor);
        }

        [Fact]
        public async Task FirebirdMathPowerTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Pow = Math.Pow(m.TypeDouble, m.TypeDouble),
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            Assert.Equal(Math.Pow(_simple.TypeDouble, _simple.TypeDouble), result.Pow);
        }

        [Fact]
        public async Task FirebirdMathRoundTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Round = Math.Round(m.TypeDouble),
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            Assert.Equal(Math.Round(_simple.TypeDouble), result.Round);
        }

        [Fact]
        public async Task FirebirdMathTruncateTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Truncate = Math.Truncate(m.TypeDouble),
                }).FirstOrDefaultAsync(m => m.Id == _simple.Id);

            Assert.Equal(Math.Truncate(_simple.TypeDouble), result.Truncate);
        }

        [Fact]
        public async Task FirebirdStartsWithOptimizedTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    StartsWith = m.TypeString.StartsWith("Entity"),
                    NotStartsWith = m.TypeString.StartsWith("Framework")
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);

            Assert.True(result.StartsWith);
            Assert.False(result.NotStartsWith);
        }

        [Fact]
        public async Task FirebirdStringLengthTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    Length = m.TypeString.Length,
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);
            Assert.Equal(_variable.TypeString.Length, result.Length);
        }

        [Fact]
        public async Task FirebirdStringReplaceTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    Replaced = m.TypeString.Replace("Entity", "Pomelo.Entity")
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);

            Assert.Equal("Pomelo.EntityFramework", result.Replaced);
        }

        [Fact]
        public async Task FirebirdStringSubstringTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    First3Chars = m.TypeString.Substring(0, 3),
                    Last3Chars = m.TypeString.Substring(m.TypeString.Length - 4, 3),
                    MiddleChars = m.TypeString.Substring(1, m.TypeString.Length - 2)
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);

            Assert.Equal(_variable.TypeString.Substring(0, 3), result.First3Chars);
            Assert.Equal(_variable.TypeString.Substring(_variable.TypeString.Length - 4, 3), result.Last3Chars);
            Assert.Equal(_variable.TypeString.Substring(1, _variable.TypeString.Length - 2), result.MiddleChars);
        }

        [Fact]
        public async Task FirebirdStringToLowerTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    ToLower = m.TypeString.ToLower()
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);

            Assert.Equal("entityframework", result.ToLower);
        }

        [Fact]
        public async Task FirebirdStringToUpperTranslator()
        {
            var result = await _db.DataTypesVariable.Select(m =>
                new {
                    Id = m.Id,
                    Upper = m.TypeString.ToUpper()
                }).FirstOrDefaultAsync(m => m.Id == _variable.Id);

            Assert.Equal("ENTITYFRAMEWORK", result.Upper);
        }

        [Fact]
        public async Task FirebirdMathAcosTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Acos = Math.Acos(m.TypeDoubleN.Value),
                }).FirstOrDefaultAsync(m => m.Id == _simple2.Id);

            Assert.Equal(Math.Acos(_simple2.TypeDoubleN.Value), result.Acos);
        }

        [Fact]
        public async Task FirebirdMathCosTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Cos = Math.Cos(m.TypeDouble),
                }).FirstOrDefaultAsync(m => m.Id == _simple2.Id);

            Assert.Equal(Math.Cos(_simple2.TypeDouble), result.Cos);
        }

        [Fact]
        public async Task FirebirdMathSinTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m =>
                new {
                    Id = m.Id,
                    Sin = Math.Sin(m.TypeDouble),
                }).FirstOrDefaultAsync(m => m.Id == _simple2.Id);

            Assert.Equal(Math.Sin(_simple2.TypeDouble), result.Sin);
        }

        [Fact]
        public async Task FirebirdDateToDateTimeConvertTranslator()
        {
            var result = await _db.DataTypesSimple.CountAsync(m => m.TypeDateTimeN <= DateTime.Now.Date);
            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task FirebirdToStringConvertTranslator()
        {
            var result = await _db.DataTypesSimple.Select(m => new {
                ConvertedInt32 = m.Id.ToString(),
                ConvertedLong = m.TypeLong.ToString(),
                ConvertedByte = m.TypeByte.ToString(),
                ConvertedSByte = m.TypeSbyte.ToString(),
                ConvertedBool = m.TypeBool.ToString(),
                ConvertedNullBool = m.TypeBoolN.ToString(),
                ConvertedDecimal = m.TypeDecimal.ToString(),
                ConvertedDouble = m.TypeDouble.ToString(),
                ConvertedFloat = m.TypeFloat.ToString(),
                ConvertedGuid = m.TypeGuid.ToString(),
                Text = m.TypeChar
            }
            ).FirstOrDefaultAsync();

            Assert.NotNull(result);
        }

    }

}
