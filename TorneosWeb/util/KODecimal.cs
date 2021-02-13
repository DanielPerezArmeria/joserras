namespace TorneosWeb.util
{
	public class KODecimal
	{
		private decimal valor;

		public KODecimal()
		{
			valor = 0;
		}

		public KODecimal(decimal d)
		{
			valor = d;
		}

		public decimal Value { get { return valor; } set { valor = value; } }

		public string Valor { get { return valor.ToString( Constants.KOS_FORMAT ); } }

		public override string ToString()
		{
			return Value.ToString( Constants.KOS_FORMAT );
		}

		public static implicit operator KODecimal(decimal v)
		{
			return new KODecimal( v );
		}

		public static implicit operator decimal(KODecimal kod)
		{
			return kod == null ? 0 : kod.Value;
		}

	}

}