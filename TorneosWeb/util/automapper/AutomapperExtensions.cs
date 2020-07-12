using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TorneosWeb.util.automapper
{
	public static class AutomapperExtensions
	{
		public static IMappingExpression<TSource,TDestination> IgnoreNoMap<TSource, TDestination>
			(this IMappingExpression<TSource, TDestination> expression)
		{
			Type sourceType = typeof( TDestination );
			foreach(PropertyInfo property in sourceType.GetProperties()){
				PropertyDescriptor descriptor = TypeDescriptor.GetProperties( sourceType )[ property.Name ];
				NoMapAttribute att = (NoMapAttribute)descriptor.Attributes[ typeof( NoMapAttribute ) ];
				if( att != null )
				{
					expression.ForMember( property.Name, opt => opt.Ignore() );
				}
			}
			return expression;
		}
	}

}