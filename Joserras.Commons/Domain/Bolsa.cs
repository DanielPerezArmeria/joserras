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
			if( fijo )
			{
				FixedForPercent -= amount;
			}
		}

	}

}