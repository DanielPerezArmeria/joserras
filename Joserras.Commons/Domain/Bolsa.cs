using Joserras.Commons.Exceptions;

namespace Joserras.Commons.Domain
{
	public class Bolsa
	{
		public decimal Premios { get; set; }
		public decimal Bounties { get; set; }
		public decimal Remanente { get; set; }
		public decimal FixedForPercent { get; set; }

		public Bolsa() { }

		public Bolsa(decimal premios) : this( premios, 0 ) { }

		public Bolsa(decimal premios, decimal bounties)
		{
			Premios = premios;
			Remanente = Premios;
			FixedForPercent = Premios;
			Bounties = bounties;
		}

		public void Otorgar(decimal amount, bool fijo = false)
		{
			Remanente -= amount;
			if( Remanente < 0 )
			{
				string msg = string.Format( "Se ha otorgado más premio de lo que hay en la bolsa" );
				throw new JoserrasException( msg );
			}
			if( fijo )
			{
				FixedForPercent -= amount;
			}
		}

		public decimal Total
		{
			get { return Premios + Bounties; }
		}

	}

}