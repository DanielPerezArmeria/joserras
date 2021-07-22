using System.ComponentModel;

namespace Joserras.Commons.Domain
{
	public enum TournamentType
	{
		NORMAL = 1,
		FREEZEOUT = 2,
		BOUNTY = 3,

		[Description("ADD ON")]
		ADDON = 4,

		[Description("SIT & GO")]
		SITGO = 5
	}

}