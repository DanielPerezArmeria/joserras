using Joserras.Client.Torneo.Domain;
using Joserras.Client.Torneo.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Joserras.Client.Torneo.Model
{
	public class KnockoutsViewModel : ViewModel
	{
		public KnockoutsViewModel()
		{
			kos = new ObservableCollection<KO>();
			Kos.CollectionChanged += Kos_CollectionChanged;

			Totales = new Dictionary<string, decimal>();
			
			OcPropertyChangedListener<KO> jugadorListener =
				OcPropertyChangedListener.Create( Kos, nameof( KO.Jugador ) );

			OcPropertyChangedListener<KO> eliminadoListener =
				OcPropertyChangedListener.Create( Kos, nameof( KO.Eliminado ) );

			OcPropertyChangedListener<KO> elimnacionesListener =
				OcPropertyChangedListener.Create( Kos, nameof( KO.Eliminaciones ) );

			jugadorListener.PropertyChanged += Kos_PropertyChanged;
			eliminadoListener.PropertyChanged += Kos_PropertyChanged;
			elimnacionesListener.PropertyChanged += Kos_PropertyChanged;
		}

		private void Kos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if(e.Action == NotifyCollectionChangedAction.Remove)
			{
				UpdateTotales();
			}
		}

		private void Kos_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			KO ko = sender as KO;
			if(string.IsNullOrEmpty(ko.Jugador) || string.IsNullOrEmpty( ko.Eliminado ))
			{
				return;
			}

			UpdateTotales();
		}

		private void UpdateTotales()
		{
			List<KeyValuePair<string, decimal>> list = Kos.ToList().GroupBy( k => k.Jugador ).Select( s =>
							new KeyValuePair<string, decimal>( s.Key, s.Sum( c => c.Eliminaciones ) ) ).ToList();

			Totales = new Dictionary<string, decimal>( list.OrderBy( k=>k.Value ) );
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

		private IDictionary<string, decimal> totales;
		public IDictionary<string, decimal> Totales
		{
			get { return totales; }
			set { SetProperty( ref totales, value ); }
		}

		public List<KO> AsList()
		{
			return Kos.ToList();
		}

	}


	public class KO : ViewModel
	{
		public KO()
		{
			Eliminaciones = 1;
		}

		private string jugador;
		public string Jugador
		{
			get { return jugador; }
			set { SetProperty( ref jugador, value ); }
		}

		private string eliminado;
		public string Eliminado
		{
			get { return eliminado; }
			set { SetProperty( ref eliminado, value ); }
		}

		private decimal eliminaciones;
		public decimal Eliminaciones
		{
			get { return eliminaciones; }
			set { SetProperty( ref eliminaciones, value ); }
		}

		private string mano;
		public string Mano
		{
			get { return mano; }
			set { SetProperty( ref mano, value ); }
		}

	}

}