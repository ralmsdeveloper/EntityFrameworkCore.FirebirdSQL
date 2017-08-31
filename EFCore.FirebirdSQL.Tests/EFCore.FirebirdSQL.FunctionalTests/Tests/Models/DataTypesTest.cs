using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SouchProd.EntityFrameworkCore.Firebird.FunctionalTests;
using SouchProd.EntityFrameworkCore.Firebird.FunctionalTests.Models;
using Xunit;

namespace EFCore.Firebird.FunctionalTests.Tests.Models
{
    public class DataTypesTest
    {
        private const sbyte TestSbyte = -128;
        private const byte TestByte = 255;
        private const char TestChar = 'a';
        private const float TestFloat = (float)1.23456789e38;
        private const TestEnum TestEnum = SouchProd.EntityFrameworkCore.Firebird.FunctionalTests.Models.TestEnum.TestOne;
        private const TestEnumByte TestEnumByte = SouchProd.EntityFrameworkCore.Firebird.FunctionalTests.Models.TestEnumByte.TestOne;
        private readonly DateTime _dateTime = new DateTime(2016, 10, 11, 1, 2, 3, 456);
        private TimeSpan _timeSpan = new TimeSpan(1, 2, 3, 4, 5);
        private readonly Guid _guid = Guid.NewGuid();

        private static void TestEmpty(DataTypesSimple emptyDb)
        {
            // bool
            Assert.Equal(default(bool), emptyDb.TypeBool);
            // nullable bool
            // Assert.Equal(null, emptyDb.TypeBoolN);

            // integers
            Assert.Equal(default(short), emptyDb.TypeShort);
            Assert.Equal(default(int), emptyDb.TypeInt);
            Assert.Equal(default(long), emptyDb.TypeLong);
            // nullable integers
            Assert.Equal(null, emptyDb.TypeShortN);
            Assert.Equal(null, emptyDb.TypeIntN);
            Assert.Equal(null, emptyDb.TypeLongN);

            // decimals
            Assert.Equal(default(decimal), emptyDb.TypeDecimal);
            Assert.Equal(default(double), emptyDb.TypeDouble);
            Assert.Equal(default(float), emptyDb.TypeFloat);
            // nullable decimals
            Assert.Equal(null, emptyDb.TypeDecimalN);
            Assert.Equal(null, emptyDb.TypeDoubleN);
            Assert.Equal(null, emptyDb.TypeFloatN);

            // byte
            Assert.Equal(default(sbyte), emptyDb.TypeSbyte);
            Assert.Equal(default(byte), emptyDb.TypeByte);
            Assert.Equal(default(char), emptyDb.TypeChar);
            // nullable byte
            Assert.Equal(null, emptyDb.TypeSbyteN);
            Assert.Equal(null, emptyDb.TypeByteN);
            Assert.Equal(null, emptyDb.TypeCharN);

            // DateTime
            Assert.Equal(default(DateTime), emptyDb.TypeDateTime);
            //Assert.Equal(default(DateTimeOffset), emptyDb.TypeDateTimeOffset);
            //Assert.Equal(default(TimeSpan), emptyDb.TypeTimeSpan);
            // nullable DateTime
            //Assert.Equal(null, emptyDb.TypeDateTimeN);
            //Assert.Equal(null, emptyDb.TypeDateTimeOffsetN);
            //Assert.Equal(null, emptyDb.TypeTimeSpanN);

            // Enum
            Assert.Equal(default(TestEnum), emptyDb.TypeEnum);
            Assert.Equal(default(TestEnumByte), emptyDb.TypeEnumByte);
            // nullableEnum
            Assert.Equal(null, emptyDb.TypeEnumN);
            Assert.Equal(null, emptyDb.TypeEnumByteN);

            // guid
            //Assert.Equal(default(Guid), emptyDb.TypeGuid);
            // nullable guid
            //Assert.Equal(null, emptyDb.TypeGuidN);
        }

        private void TestValue(DataTypesSimple valueDb)
        {
            // bool
            Assert.Equal(true, valueDb.TypeBool);

            // nullable bool
            Assert.Equal(true, valueDb.TypeBoolN);

            // integers
            Assert.Equal(short.MinValue, valueDb.TypeShort);
            Assert.Equal(int.MinValue, valueDb.TypeInt);
            Assert.Equal(long.MinValue, valueDb.TypeLong);
            // nullable integers
            Assert.Equal(short.MinValue, valueDb.TypeShortN);
            Assert.Equal(int.MinValue, valueDb.TypeIntN);
            Assert.Equal(long.MinValue, valueDb.TypeLongN);

            // decimals
            Assert.Equal(26768.124M, valueDb.TypeDecimal);
            Assert.Equal(double.MaxValue, valueDb.TypeDouble);
            Assert.InRange(valueDb.TypeFloat, TestFloat * (1 - 7e-1), TestFloat * (1 + 7e-1)); // floats have 7 digits of precision
            // nullable decimals
            Assert.Equal(3.1416M, valueDb.TypeDecimalN);
            Assert.Equal(double.MaxValue, valueDb.TypeDoubleN);
            Assert.InRange(valueDb.TypeFloatN.GetValueOrDefault(), TestFloat * (1 - 7e-1), TestFloat * (1 + 7e-1)); // floats have 7 digits of precision

            // byte
            Assert.Equal(TestSbyte, valueDb.TypeSbyte);
            Assert.Equal(TestByte, valueDb.TypeByte);
            Assert.Equal(TestChar, valueDb.TypeChar);
            // nullable byte
            Assert.Equal(TestSbyte, valueDb.TypeSbyte);
            Assert.Equal(TestByte, valueDb.TypeByteN);
            Assert.Equal(TestChar, valueDb.TypeCharN);

            // DateTime
            Assert.Equal(_dateTime, valueDb.TypeDateTime);
            //Assert.Equal(dateTimeOffset, valueDb.TypeDateTimeOffset);
            //Assert.Equal(timeSpan, valueDb.TypeTimeSpan);
            // nullable DateTime
            Assert.Equal(_dateTime, valueDb.TypeDateTimeN);
            //Assert.Equal(dateTimeOffset, valueDb.TypeDateTimeOffsetN);
            //Assert.Equal(timeSpan, valueDb.TypeTimeSpanN);

            // Enum
            Assert.Equal(TestEnum, valueDb.TypeEnum);
            Assert.Equal(TestEnumByte, valueDb.TypeEnumByte);
            // nullable Enum
            Assert.Equal(TestEnum, valueDb.TypeEnumN);
            Assert.Equal(TestEnumByte, valueDb.TypeEnumByteN);

            // guid
            //Assert.Equal(_guid, valueDb.TypeGuid);
            // nullable guid
            //Assert.Equal(_guid, valueDb.TypeGuidN);
        }

        private DataTypesSimple NewValueMem() => new DataTypesSimple
        {
            // bool
            TypeBool = true,
            // nullable bool
            TypeBoolN = true,

            // integers
            TypeShort = short.MinValue,
            TypeInt = int.MinValue,
            TypeLong = long.MinValue,
            // nullable integers
            TypeShortN = short.MinValue,
            TypeIntN = int.MinValue,
            TypeLongN = long.MinValue,

            // decimals
            TypeDecimal = 26768.124M,
            TypeDouble = double.MaxValue,
            TypeFloat = TestFloat,
            // nullable decimals
            TypeDecimalN = 3.1416M,
            TypeDoubleN = double.MaxValue,
            TypeFloatN = TestFloat,

            // byte
            TypeSbyte = TestSbyte,
            TypeByte = TestByte,
            TypeChar = TestChar,
            // nullable byte
            TypeSbyteN = TestSbyte,
            TypeByteN = TestByte,
            TypeCharN = TestChar,

            // DateTime
            TypeDateTime = _dateTime,
            //TypeDateTimeOffset = dateTimeOffset,
            //TypeTimeSpan = timeSpan,
            // nullable DateTime
            TypeDateTimeN = _dateTime,
            //TypeDateTimeOffsetN = dateTimeOffset,
            //TypeTimeSpanN = timeSpan,

            // Enum
            TypeEnum = TestEnum,
            TypeEnumByte = TestEnumByte,
            // nullable Enum
            TypeEnumN = TestEnum,
            TypeEnumByteN = TestEnumByte,

            // guid
            //TypeGuid = _guid,
            // nullable guid
            //TypeGuidN = _guid,
        };
        
        /// <summary>
        /// Test each data type with the default value
        /// </summary>
        [Fact]
        public void TestSimpleTypesEmpty()
        {
            // Create test data objects
            var emptyMemSync = new DataTypesSimple();

            // Save them to the database
            using (var db = new AppDb())
            {
                db.DataTypesSimple.Add(emptyMemSync);
                db.SaveChanges();
            }

            // Load them from the database and run tests
            using (var db = new AppDb())
            {
                var item = db.DataTypesSimple.FirstOrDefault(x=>x.Id == emptyMemSync.Id);
                TestEmpty(item);
            }
        }

        /// <summary>
        /// Test each data type with the default value
        /// </summary>
        [Fact]
        public async Task TestSimpleTypesEmptyAsync()
        {
            // Create test data objects
            var emptyMemAsync = new DataTypesSimple();

            // Save them to the database
            using (var db = new AppDb())
            {
                db.DataTypesSimple.Add(emptyMemAsync);
                await db.SaveChangesAsync();
            }

            // Load them from the database and run tests
            using (var db = new AppDb())
            {
                // ReSharper disable once AccessToDisposedClosure
                async Task<DataTypesSimple> FromDbAsync(DataTypesSimple dt) => await db.DataTypesSimple.FirstOrDefaultAsync(m => m.Id == dt.Id);
                TestEmpty(await FromDbAsync(emptyMemAsync));
            }
        }

        [Fact]
        public void TestSimpleTypesNotEmpty()
        {
            // test each data type with a valid value
            // ReSharper disable once ObjectCreationAsStatement

            // create test data objects
            var valueMemSync = NewValueMem();

            using (var db = new AppDb())
            {
                db.DataTypesSimple.Add(valueMemSync);
                db.SaveChanges();
            }

            // load them from the database and run tests
            using (var db = new AppDb())
            {
                // ReSharper disable once AccessToDisposedClosure
                DataTypesSimple FromDbSync(DataTypesSimple dt) => db.DataTypesSimple.FirstOrDefault(m => m.Id == dt.Id);
                TestValue(FromDbSync(valueMemSync));
            }
        }

        [Fact]
        public async Task TestSimpleTypesNotEmptyAsync()
        {
            // test each data type with a valid value
            // ReSharper disable once ObjectCreationAsStatement

            // create test data objects
            var valueMemAsync = NewValueMem();

            // save them to the database
            using (var db = new AppDb())
            {
                db.DataTypesSimple.Add(valueMemAsync);
                await db.SaveChangesAsync();
            }

            // load them from the database and run tests
            using (var db = new AppDb())
            {
                // ReSharper disable once AccessToDisposedClosure
                async Task<DataTypesSimple> FromDbAsync(DataTypesSimple dt) => await db.DataTypesSimple.FirstOrDefaultAsync(m => m.Id == dt.Id);
                TestValue(await FromDbAsync(valueMemAsync));
            }
        }

        [Fact]
	    public async Task TestDataTypesVariable()
	    {
	        void TestEmpty(DataTypesVariable valueDb)
	        {
	            // string not null
	            Assert.Equal("", valueDb.TypeString);
	            Assert.Equal("", valueDb.TypeString255);
	            // string null
	            Assert.Equal(null, valueDb.TypeStringN);
	            Assert.Equal(null, valueDb.TypeString255N);

	            // binary not null
	            Assert.Equal(DataTypesVariable.EmptyByteArray, valueDb.TypeByteArray);
	            Assert.Equal(DataTypesVariable.EmptyByteArray, valueDb.TypeByteArray255);
	            // binary null
	            Assert.Equal(null, valueDb.TypeByteArrayN);
	            Assert.Equal(null, valueDb.TypeByteArray255N);

	            // json not null
	            Assert.Equal(DataTypesVariable.EmptyJsonArray.Json, valueDb.TypeJsonArray.Json);
	            Assert.Equal(DataTypesVariable.EmptyJsonObject.Json, valueDb.TypeJsonObject.Json);
	            // json null
	            Assert.Equal(null, valueDb.TypeJsonArrayN);
	            Assert.Equal(null, valueDb.TypeJsonObjectN);
	        }

	        var string255 = new string('a', 255);
            var string4K = new string('a', 4000);

            var byte255 = new byte[255];
            var byte10K = new byte[10000];
            for (var i = 0; i < byte10K.Length; i++)
            {
                if (i < 255)
                    byte255[i] = (byte) 'a';
                byte10K[i] = (byte) 'a';
            }

            var jsonArray = new JsonObject<List<string>>(new List<string> {"test"});
            var jsonObject = new JsonObject<Dictionary<string, string>>(new Dictionary<string, string> {{"test", "test"}});

            // test each data type with a valid value
	        DataTypesVariable NewValueMem() => new DataTypesVariable
	        {
	            // string not null
	            TypeString = string4K,
	            TypeString255 = string255, // should be truncated by DBMS
                
                // string null
                TypeStringN = string4K,
                TypeString255N = string255, // should be truncated by DBMS

                // binary not null
                TypeByteArray = byte10K,
                TypeByteArray255 = byte255, // should be truncated by DBMS
                
                // binary null
                TypeByteArrayN =  byte10K,
                TypeByteArray255N = byte255, // should be truncated by DBMS

                // json not null
                TypeJsonArray = jsonArray,
	            TypeJsonObject = jsonObject,
                
                // json null
                TypeJsonArrayN = jsonArray,
                TypeJsonObjectN = jsonObject,
            };

	        void TestValue(DataTypesVariable valueDb)
	        {
	            // string not null
	            Assert.Equal(string4K, valueDb.TypeString);
	            Assert.Equal(string255, valueDb.TypeString255);
	            // string null
	            Assert.Equal(string4K, valueDb.TypeStringN);
	            Assert.Equal(string255, valueDb.TypeString255N);

	            // binary not null
	            Assert.Equal(byte10K, valueDb.TypeByteArray);
	            Assert.Equal(byte255, valueDb.TypeByteArray255);
	            // binary null
	            Assert.Equal(byte10K, valueDb.TypeByteArrayN);
	            Assert.Equal(byte255, valueDb.TypeByteArray255N);

	            // json not null
	            Assert.Equal(jsonArray.Json, valueDb.TypeJsonArray.Json);
	            Assert.Equal(jsonObject.Json, valueDb.TypeJsonObject.Json);
	            // json null
	            Assert.Equal(jsonArray.Json, valueDb.TypeJsonArrayN.Json);
	            Assert.Equal(jsonObject.Json, valueDb.TypeJsonObjectN.Json);
	        }

	        // create test data objects
		    var emptyMemAsync = DataTypesVariable.CreateEmpty();
		    var emptyMemSync = DataTypesVariable.CreateEmpty();
		    var valueMemAsync = NewValueMem();
		    var valueMemSync = NewValueMem();

		    // save them to the database
		    using (var db = new AppDb()){
			    db.DataTypesVariable.Add(emptyMemAsync);
			    db.DataTypesVariable.Add(valueMemAsync);
			    await db.SaveChangesAsync();

			    db.DataTypesVariable.Add(emptyMemSync);
			    db.DataTypesVariable.Add(valueMemSync);
			    db.SaveChanges();
		    }

		    // load them from the database and run tests
		    using (var db = new AppDb())
		    {
			    // ReSharper disable once AccessToDisposedClosure
		        async Task<DataTypesVariable> FromDbAsync(DataTypesVariable dt) => await db.DataTypesVariable.FirstOrDefaultAsync(m => m.Id == dt.Id);

		        // ReSharper disable once AccessToDisposedClosure
		        DataTypesVariable FromDbSync(DataTypesVariable dt) => db.DataTypesVariable.FirstOrDefault(m => m.Id == dt.Id);

		        TestEmpty(await FromDbAsync(emptyMemAsync));
			    TestEmpty(FromDbSync(emptyMemSync));
			    TestValue(await FromDbAsync(valueMemAsync));
			    TestValue(FromDbSync(valueMemSync));
		    }

	    }

        [Fact]
        private void CheckGUIDWrite()
        {
            using (var db = new AppDb())
            {
                var itm = new TableGuidType
                {
                    uid = new Guid()
                };
                db.TableGuidType.Add(itm);
                db.SaveChanges();
            }
        }

        [Fact]
        private void CheckGUIDReadWrite()
        {
            using (var db = new AppDb())
            {
                var itm = new TableGuidType
                {
                    uid = new Guid("3d2a2463-3120-4663-80c8-df3d3e166258")
                };
                db.TableGuidType.Add(itm);
                db.SaveChanges();

                var uid = db.TableGuidType.OrderByDescending(x => x.Id).FirstOrDefault();
                Assert.Equal(uid.uid.ToString(), "3d2a2463-3120-4663-80c8-df3d3e166258");
            }
        }

        [Fact]
        private void CheckIntReadWrite()
        {
            using (var db = new AppDb())
            {
                var itm = new TableIntType
                {
                    IntField = 31416147
                };
                db.TableIntType.Add(itm);
                db.SaveChanges();

                var uid = db.TableIntType.OrderByDescending(x => x.Id).FirstOrDefault();
                Assert.Equal(uid.IntField, 31416147);
            }
        }

        [Fact]
        private void CheckIntWrite()
        {
            using (var db = new AppDb())
            {
                var itm = new TableIntType
                {
                    IntField = 31416147
                };
                db.TableIntType.Add(itm);
                db.SaveChanges();
            }
        }

        [Fact]
        private void CheckDoubleWrite()
        {
            using (var db = new AppDb())
            {
                var itm = new TableDoubleType
                {
                    Dbl = 3.1416
                };
                db.TableDoubleType.Add(itm);
                db.SaveChanges();
            }
        }

        [Fact]
        private void CheckDoubleReadWrite()
        {
            using (var db = new AppDb())
            {
                var itm = new TableDoubleType
                {
                    Dbl = 3.1416
                };
                db.TableDoubleType.Add(itm);
                db.SaveChanges();

                var uid = db.TableDoubleType.OrderByDescending(x => x.Id).FirstOrDefault();
                Assert.Equal(uid.Dbl, 3.1416);
            }
        }

        [Fact]
        private void CheckStringWrite()
        {
            using (var db = new AppDb())
            {
                var itm = new TableStringType
                {
                    StrField = this.GetType().ToString()
                };
                db.TableStringType.Add(itm);
                db.SaveChanges();
            }
        }

        [Fact]
        private void CheckStringReadWrite()
        {
            using (var db = new AppDb())
            {
                var itm = new TableStringType
                {
                    StrField = this.GetType().ToString()
                };
                db.TableStringType.Add(itm);
                db.SaveChanges();

                var uid = db.TableStringType.OrderByDescending(x => x.Id).FirstOrDefault();
                Assert.Equal(uid.StrField, this.GetType().ToString());
            }
        }

    }
}

