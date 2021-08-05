using System;

namespace Joserras.Commons.Domain
{
	public class PrizeRange : IEquatable<PrizeRange>
	{
		public int Menor { get; set; }
		public int Mayor { get; set; }
		public string Premiacion { get; set; }

		public PrizeRange() { }

		public PrizeRange(int menor, int mayor, string premiacion)
		{
			Menor = menor;
			Mayor = mayor;
			Premiacion = premiacion;
		}

		public bool IsBetween(int x)
		{
			return Menor <= x && x <= Mayor;
		}

		public bool Equals(PrizeRange other)
		{
			return Menor == other.Menor && Mayor == other.Mayor;
		}

		public override string ToString()
		{
			return string.Format( "{0} - {1}: {2}", Menor, Mayor, Premiacion );
		}

		public override bool Equals(object obj)
		{
			return Equals( obj as PrizeRange );
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

	}

}