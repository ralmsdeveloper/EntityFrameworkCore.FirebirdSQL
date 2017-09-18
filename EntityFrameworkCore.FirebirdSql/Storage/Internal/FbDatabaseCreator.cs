/*
 *          Copyright (c) 2017 Rafael Almeida (ralms@ralms.net)
 *							   Jiri Cincura	  (jiri@cincura.net)
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
using EntityFrameworkCore.FirebirdSql.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using FirebirdClientConnection = FirebirdSql.Data.FirebirdClient.FbConnection;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.FirebirdSql.Storage.Internal
{ 
    public class FbDatabaseCreator : RelationalDatabaseCreator
    { 
	    readonly IFbRelationalConnection _connection;
	    readonly IRawSqlCommandBuilder _rawSqlCommandBuilder;

	    public FbDatabaseCreator(RelationalDatabaseCreatorDependencies dependencies, IFbRelationalConnection connection, IRawSqlCommandBuilder rawSqlCommandBuilder)
		    : base(dependencies)
	    {
		    _connection = connection;
		    _rawSqlCommandBuilder = rawSqlCommandBuilder;
	    }

	    public override void Create()
	    {
		    Dependencies.MigrationCommandExecutor.ExecuteNonQuery(CreateDatabaseOperations(), _connection);
	    }

	    public override void Delete()
	    {
		    FirebirdClientConnection.ClearPool((FirebirdClientConnection)_connection.DbConnection);
		    FirebirdClientConnection.DropDatabase(_connection.ConnectionString);
	    }

	    public override bool Exists()
	    {
		    try
		    {
			    _connection.Open();
			    _connection.Close();
				return true;
		    }
		    catch (FbException)
		    {
			    return false;
		    } 
	    }

		private IReadOnlyList<MigrationCommand> CreateDatabaseOperations()
		{
			var operations = new MigrationOperation[]
			{
					  new FbCreateDatabaseOperation
					  {
						  ConnectionStringBuilder = new FbConnectionStringBuilder(_connection.DbConnection.ConnectionString)
					  }
			};
			return Dependencies.MigrationsSqlGenerator.Generate(operations);
		}

		protected override bool HasTables()
		{
			return Dependencies.ExecutionStrategyFactory.Create().Execute(_connection, connection => Convert.ToInt32(CreateHasTablesCommand().ExecuteScalar(connection)) != 0);
		}

		IRelationalCommand CreateHasTablesCommand()
		{
			return _rawSqlCommandBuilder.Build("SELECT COUNT(*) FROM rdb$relations WHERE COALESCE(rdb$system_flag, 0) = 0 AND rdb$view_blr IS NULL");
		}
	}
}
