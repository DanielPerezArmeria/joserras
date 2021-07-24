using Joserras.Client.Torneo.Model;
using Joserras.Commons.Domain;
using System;
using System.Collections.Generic;

namespace Joserras.Client.Torneo.Domain
{
	public class TorneoViewModel : ViewModel<Guid>
	{
		private readonly List<TournamentType> tournamentTypes;

		public TorneoViewModel()
		{
			Tipo = TournamentType.NORMAL;
			Fecha = DateTime.Now;
			PrecioBuyin = 250;

			tournamentTypes = new();
			tournamentTypes.Add( TournamentType.NORMAL );
			tournamentTypes.Add( TournamentType.ADDON );
			tournamentTypes.Add( TournamentType.FREEZEOUT );
			tournamentTypes.Add( TournamentType.SITGO );
			tournamentTypes.Add( TournamentType.BOUNTY );
		}

		private DateTime fecha;
		public DateTime Fecha
		{
			get { return fecha; }
			set { SetProperty( ref fecha, value ); }
		}

		private TournamentType tipo;
		public TournamentType Tipo
		{
			get { return tipo; }
			set { SetProperty( ref tipo, value ); }
		}

		private bool liga;
		public bool Liga
		{
			get { return liga; }
			set { SetProperty( ref liga, value ); }
		}

		private int precioBuyin;
		public int PrecioBuyin
		{
			get { return precioBuyin; }
			set { SetProperty( ref precioBuyin, value ); }
		}

		private string premiacion;
		public string Premiacion
		{
			get { return premiacion; }
			set { SetProperty( ref premiacion, value ); }
		}

		public List<TournamentType> TournamentTypes
		{
			get
			{
				return tournamentTypes;
			}
		}


	}

}