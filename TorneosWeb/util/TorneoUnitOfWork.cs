using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace TorneosWeb.util
{
	public class TorneoUnitOfWork : IDisposable
	{
		private SqlTransaction Transaction { get; set; }
		private SqlConnection Conn { get; set; }
		private string ConnectionString { get; set; }

		public TorneoUnitOfWork(string connString)
		{
			ConnectionString = connString;

			Conn = new SqlConnection( ConnectionString );
			Conn.Open();
			Transaction = Conn.BeginTransaction();
		}

		public void Dispose()
		{
			Transaction.Dispose();
			Conn.Close();
		}

		public int ExecuteNonQuery(string query, params object[] list)
		{
			if(list != null && list.Length > 0 )
			{
				query = string.Format( query, list );
			}
			
			return new SqlCommand( query, Conn, Transaction ).ExecuteNonQuery();
		}

		public int ExecuteNonQuery(string query, IDictionary<string,object> parameters)
		{
			SqlCommand command = new SqlCommand( query, Conn, Transaction );
			foreach(string key in parameters.Keys)
			{
				command.Parameters.AddWithValue( key, parameters[key] ?? DBNull.Value );
			}

			return command.ExecuteNonQuery();
		}

		public object ExecuteScalar(string query, params object[] list)
		{
			if( list != null && list.Length > 0 )
			{
				query = string.Format( query, list );
			}

			return new SqlCommand( query, Conn, Transaction ).ExecuteScalar();
		}

		public object ExecuteScalar(string query, IDictionary<string, object> parameters)
		{
			SqlCommand command = new SqlCommand( query, Conn, Transaction );
			foreach (string key in parameters.Keys)
			{
				command.Parameters.AddWithValue( key, parameters[key] ?? DBNull.Value );
			}

			return command.ExecuteNonQuery();
		}

		public void Commit()
		{
			Transaction.Commit();
		}

		public void Rollback()
		{
			Transaction.Rollback();
		}

	}

}