using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Joserras.Client.Torneo.Domain
{
	public abstract class BindableBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Checks if a property already matches a desired value. Sets the property and
		/// notifies listeners only when necessary.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="storage">Reference to a property with both getter and setter.</param>
		/// <param name="value">Desired value for the property.</param>
		/// <param name="propertyName">Name of the property used to notify listeners. This
		/// value is optional and can be provided automatically when invoked from compilers that
		/// support CallerMemberName.</param>
		/// <returns>True if the value was changed, false if the existing value matched the
		/// desired value.</returns>
		protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (Equals( storage, value )) return false;

			storage = value;
			OnPropertyChanged( propertyName );

			return true;
		}

		/// <summary>
		/// Notifies listeners that a property value has changed.
		/// </summary>
		/// <param name="propertyName">Name of the property used to notify listeners. This
		/// value is optional and can be provided automatically when invoked from compilers
		/// that support <see cref="CallerMemberNameAttribute"/>.</param>
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
		}

		/// <summary>
		/// Raises this object's PropertyChanged event.
		/// </summary>
		/// <typeparam name="T">The type of the property that has a new value</typeparam>
		/// <param name="propertyExpression">A Lambda expression representing the property that has a new value.</param>
		protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			var propertyName = PropertySupport.ExtractPropertyName( propertyExpression );
			OnPropertyChanged( propertyName );
		}

	}
}

namespace Joserras.Client.Torneo.Domain
{
	public static class PropertySupport
	{
		/// <summary>
		/// Extracts the property name from a property expression.
		/// </summary>
		/// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
		/// <param name="propertyExpression">The property expression (e.g. p => p.PropertyName)</param>
		/// <returns>The name of the property.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="propertyExpression"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown when the expression is:<br/>
		///     Not a <see cref="MemberExpression"/><br/>
		///     The <see cref="MemberExpression"/> does not represent a property.<br/>
		///     Or, the property is static.
		/// </exception>
		public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
		{
			if (propertyExpression == null)
				throw new ArgumentNullException( nameof( propertyExpression ) );

			return ExtractPropertyNameFromLambda( propertyExpression );
		}

		/// <summary>
		/// Extracts the property name from a LambdaExpression.
		/// </summary>
		/// <param name="expression">The LambdaExpression</param>
		/// <returns>The name of the property.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="expression"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown when the expression is:<br/>
		///     The <see cref="MemberExpression"/> does not represent a property.<br/>
		///     Or, the property is static.
		/// </exception>
		internal static string ExtractPropertyNameFromLambda(LambdaExpression expression)
		{
			if (expression == null)
				throw new ArgumentNullException( nameof( expression ) );

			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null)
				throw new ArgumentException( "Acceso de no miembro", nameof( expression ) );

			var property = memberExpression.Member as PropertyInfo;
			if (property == null)
				throw new ArgumentException( "Expresión no es propiedad", nameof( expression ) );

			var getMethod = property.GetMethod;
			if (getMethod.IsStatic)
				throw new ArgumentException( "Expresión estática", nameof( expression ) );

			return memberExpression.Member.Name;
		}

	}

}