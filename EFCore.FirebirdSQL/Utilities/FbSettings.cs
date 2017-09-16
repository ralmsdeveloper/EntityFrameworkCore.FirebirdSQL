/*
 *          Copyright (c) 2017 Rafael Almeida (ralms@ralms.net)
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
using System.Collections.Concurrent;   
using FirebirdSql.Data.FirebirdClient;
using System.Data.Common;
using System.Data;

namespace EntityFrameworkCore.FirebirdSql.Utilities
{
	public class FbSettings
	{

		public Version ServerVersion;
		public bool IsSupportIdentityIncrement => ServerVersion.Major >= 3;
		public int ObjectLengthName => ServerVersion.Major == 3 ? 31 : 63;

		private static readonly ConcurrentDictionary<string, FbSettings> Settings = new ConcurrentDictionary<string, FbSettings>();

		public FbSettings GetSettings(string connectionString)
		{
			var csb = new FbConnectionStringBuilder(connectionString);

			return Settings.GetOrAdd(csb.ConnectionString, key =>
			{
				try
				{
					using (var _connection = new FbConnection(csb.ConnectionString))
					{
						_connection.Open();
						ServerVersion = FirebirdSql.Data.Services.FbServerProperties.ParseServerVersion(_connection.ServerVersion);
					}
				}
				catch
				{
					//
				}
				return this;
			});
		}

		public FbSettings GetSettings(DbConnection connection)
		{
			var csb = new FbConnectionStringBuilder(connection.ConnectionString); 

			return Settings.GetOrAdd(csb.ConnectionString, key =>
			{
				if (connection.State == ConnectionState.Closed)
				{
					connection.Open();
				}
				try
				{
					ServerVersion = FirebirdSql.Data.Services.FbServerProperties.ParseServerVersion(connection.ServerVersion);
					return this;
				}
				finally
				{
					if (connection?.State == ConnectionState.Open)
						connection.Close();
				}
			});
		}
	}
}
