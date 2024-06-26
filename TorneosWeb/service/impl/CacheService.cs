﻿using System.Collections.Concurrent;

namespace TorneosWeb.service.impl
{
	public class CacheService : ICacheService
	{
		private readonly ConcurrentDictionary<string, object> map;

		public CacheService()
		{
			map = new ConcurrentDictionary<string, object>();
		}

		public void Clear()
		{
			map.Clear();
		}

		public void Add(string key, object value)
		{
			map.TryAdd( key, value );
		}

		public T Get<T>(string key)
		{
			if( map.TryGetValue( key, out object obj ) )
			{
				return (T)obj;
			}

			return default( T );
		}

		public bool ContainsKey(string key)
		{
			return map.ContainsKey( key );
		}

	}

}