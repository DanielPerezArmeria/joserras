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
			FDecimal kod = new FDecimal(value);
			Assert.Equal( kod.ToString(), $"{value}" );
		}

	}

}