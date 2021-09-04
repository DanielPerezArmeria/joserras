using Joserras.Commons.Exceptions;

namespace Joserras.Commons.Domain
{
	public class Bolsa
	{
		public decimal Total { get; set; }
		public decimal Remanente { get; set; }
		public decimal FixedForPercent { get; set; }

		public Bolsa() { }

		public Bolsa(decimal total)
		{
			Total = total;
			Remanente = Total;
			FixedForPercent = Total;
		}

		public void Otorgar(decimal amount, bool fijo=false)
		{
			Remanente -= amount;
			if(Remanente < 0)
			{
				string msg = string.Format( "Se ha otorgado más premio de lo que hay en la bolsa" );
				throw new JoserrasException( msg );
			}
			if( fijo )
			{
				FixedForPercent -= amount;
			}
		}

	}

}