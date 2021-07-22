using System;
using System.Data.SqlClient;

namespace Joserras.Commons.Db
{
	public class JoserrasQuery
	{
		private readonly string connectionString;

		public JoserrasQuery(string conn)
		{
			connectionString = conn;
		}

		public void ExecuteQuery(SqlConnection conn, string query, Action<SqlDataReader> action, params object[] args)
		{
			using( SqlCommand command = new SqlCommand( query, conn ) )
			{
				if(args != null && args.Length > 0 )
				{
					command.Parameters.AddRange( args );
				}
				using( SqlDataReader reader = command.ExecuteReader() )
				{
					action.Invoke( reader );
				}
			}
		}

		public void ExecuteQuery(string query, Action<SqlDataReader> action, params object[] args)
		{
			using( SqlConnection conn = new SqlConnection( connectionString ) )
			{
				conn.Open();
				ExecuteQuery( conn, query, action, args );
			}
		}

		public T ExecuteQuery<T>(SqlConnection conn, string query, Func<SqlDataReader,T> func, params object[] args)
		{
			using( SqlCommand command = new SqlCommand( query, conn ) )
			{
				if( args != null && args.Length > 0 )
				{
					command.Parameters.AddRange( args );
				}

				using( SqlDataReader reader = command.ExecuteReader() )
				{
					return func.Invoke( reader );
				}
			}
		}

		public T ExecuteQuery<T>(string query, Func<SqlDataReader, T> func, params object[] args)
		{
			using( SqlConnection conn = new SqlConnection( connectionString ) )
			{
				conn.Open();
				return ExecuteQuery( conn, query, func, args );
			}
		}

	}

}