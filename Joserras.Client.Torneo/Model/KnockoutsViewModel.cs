using Joserras.Client.Torneo.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Joserras.Client.Torneo.Model
{
	public class KnockoutsViewModel : ViewModel
	{
		public KnockoutsViewModel()
		{
			kos = new ObservableCollection<KO>();
		}

		private List<JugadorViewModel> jugadores;
		public List<JugadorViewModel> Jugadores
		{
			get { return jugadores; }
			set { SetProperty( ref jugadores, value ); }
		}

		private ObservableCollection<KO> kos;
		public ObservableCollection<KO> Kos
		{
			get { return kos; }
		}

	}


	public class KO : ViewModel
	{
		private JugadorViewModel jugador;
		public JugadorViewModel Jugador
		{
			get { return jugador; }
			set { SetProperty( ref jugador, value ); }
		}

		private JugadorViewModel eliminado;
		public JugadorViewModel Eliminado
		{
			get { return eliminado; }
			set { SetProperty( ref eliminado, value ); }
		}

		private string mano;
		public string Mano
		{
			get { return mano; }
			set { SetProperty( ref mano, value ); }
		}

	}

}