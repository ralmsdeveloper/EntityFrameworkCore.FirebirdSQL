// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable UnusedParameter.Local
// ReSharper disable InconsistentNaming
namespace Microsoft.EntityFrameworkCore.Query
{
    public partial class SimpleQueryFirebirdTest : SimpleQueryTestBase<NorthwindQueryirebirdFixture<NoopModelCustomizer>>
    {
        public SimpleQueryFirebirdTest(NorthwindQueryirebirdFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }
         
        public override void Parameter_extraction_short_circuits_1()
        {
            using (var ctx = Fixture.CreateContext())
            {
                var parts = new List<int>();
                var peoples = ctx
                    .Orders
                    .Where(p => p.ShipVia.HasValue && parts.Contains(p.ShipVia.Value))
                    .ToList();
            }

            var sql = Fixture.TestSqlLoggerFactory.Sql;
             
        } 

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        private void AssertContainsSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected, assertOrder: false);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
