using System;
using System.Collections.Generic;
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

		public void ExecuteQuery(string query, Action<SqlDataReader> action)
		{
			ExecuteQuery( query, null, action );
		}

		public void ExecuteQuery(string query, IDictionary<string, object> parameters, Action<SqlDataReader> action)
		{
			using( SqlConnection conn = new( connectionString ) )
			{
				using (SqlCommand command = new( query, conn ))
				{
					conn.Open();
					if (parameters != null)
					{
						foreach (string key in parameters.Keys)
						{
							command.Parameters.AddWithValue( key, parameters[key] );
						}
					}

					using (SqlDataReader reader = command.ExecuteReader())
					{
						action.Invoke( reader );
					}
				}
			}
		}

		public T ExecuteQuery<T>(string query, Func<SqlDataReader, T> func)
		{
			return ExecuteQuery( query, null, func );
		}

		public T ExecuteQuery<T>(string query, IDictionary<string, object> parameters, Func<SqlDataReader, T> func)
		{
			using( SqlConnection conn = new( connectionString ) )
			{
				using (SqlCommand command = new( query, conn ))
				{
					conn.Open();
					if (parameters != null)
					{
						foreach (string key in parameters.Keys)
						{
							command.Parameters.AddWithValue( key, parameters[key] );
						}
					}

					using (SqlDataReader reader = command.ExecuteReader())
					{
						return func.Invoke( reader );
					}
				}
			}
		}

	}

}