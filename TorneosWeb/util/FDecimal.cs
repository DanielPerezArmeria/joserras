namespace TorneosWeb.util
{
	public class FDecimal
	{
		private readonly string format;

		public FDecimal()
		{
			Valor = 0;
			format = Constants.POINTS_FORMAT;
		}

		public FDecimal(decimal d)
		{
			Valor = d;
			format = Constants.POINTS_FORMAT;
		}

		public FDecimal(decimal d, string format)
		{
			Valor = d;
			this.format = format;
		}

		public decimal Valor { get; set; }

		public string ValorString { get { return Valor.ToString( format ); } }

		public override string ToString()
		{
			return ValorString;
		}

		public static implicit operator FDecimal(decimal v)
		{
			return new FDecimal( v );
		}

		public static implicit operator decimal(FDecimal kod)
		{
			return kod == null ? 0 : kod.Valor;
		}

	}

}