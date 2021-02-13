using TorneosWeb.util;
using Xunit;

namespace TorneosWebTesting.util
{
	public class KODecimalTest
	{
		[Theory]
		[InlineData( 5 )]
		[InlineData( 5.5 )]
		public void ToStringTest(decimal value)
		{
			KODecimal kod = new KODecimal(value);
			Assert.Equal( kod.ToString(), $"{value}" );
		}

	}

}