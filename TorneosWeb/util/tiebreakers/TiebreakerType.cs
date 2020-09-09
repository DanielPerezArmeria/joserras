using System.ComponentModel;

namespace TorneosWeb.util.tiebreakers
{
	public enum TiebreakerType
	{
		[Description( "KO's Directos" )]
		KOS_DIRECTOS,

		[Description( "Profit" )]
		PROFIT,

		[Description( "Puntos" )]
		PUNTOS,

		[Description( "Base" )]
		BASE
	}

}