using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;

namespace TorneosWeb.util
{
	public class JoserrasQuery
	{
		private string connectionString;

		public JoserrasQuery(IConfiguration conf)
		{
			connectionString = conf.GetConnectionString( Properties.Resources.joserrasDb);
		}

		public void ExecuteQuery(SqlConnection conn, string query, Action<SqlDataReader> action)
		{
			using( SqlCommand command = new SqlCommand( query, conn ) )
			{
				using( SqlDataReader reader = command.ExecuteReader() )
				{
					action.Invoke( reader );
				}
			}
		}

		public void ExecuteQuery(string query, Action<SqlDataReader> action)
		{
			using( SqlConnection conn = new SqlConnection( connectionString ) )
			{
				conn.Open();
				ExecuteQuery( conn, query, action );
			}
		}

		public T ExecuteQuery<T>(SqlConnection conn, string query, Func<SqlDataReader,T> func)
		{
			using( SqlCommand command = new SqlCommand( query, conn ) )
			{
				using( SqlDataReader reader = command.ExecuteReader() )
				{
					return func.Invoke( reader );
				}
			}
		}

		public T ExecuteQuery<T>(string query, Func<SqlDataReader, T> func)
		{
			using( SqlConnection conn = new SqlConnection( connectionString ) )
			{
				conn.Open();
				return ExecuteQuery( conn, query, func );
			}
		}

	}

}