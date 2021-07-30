using Joserras.Client.Torneo.Domain;
using Joserras.Client.Torneo.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Joserras.Client.Torneo.Model
{
	public class ResultadosViewModel : ViewModel
	{
		public ResultadosViewModel()
		{
			resultados = new ObservableCollection<Resultado>();
			Resultados.CollectionChanged += Resultados_CollectionChanged;

			OcPropertyChangedListener<Resultado> rebuysListener =
				OcPropertyChangedListener.Create( Resultados, nameof( Resultado.Rebuys ) );

			rebuysListener.PropertyChanged += RebuysListener_PropertyChanged;
			UpdateBolsa();
		}

		private void RebuysListener_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Rebuys = Resultados.Sum( r => r.Rebuys );
			UpdateBolsa();
		}

		private void Resultados_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			foreach(Resultado resultado in Resultados)
			{
				resultado.Posicion = resultados.IndexOf( resultado ) + 1;
			}

			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Remove:
					Rebuys = Resultados.Sum( r => r.Rebuys );
					UpdateBolsa();
					break;

				default:
					break;
			}
		}

		private void UpdateBolsa()
		{
			Bolsa = Resultados.Count * buyin + Resultados.Sum( r => r.Rebuys * Buyin );
			Entradas = Resultados.Count + Resultados.Sum( r => r.Rebuys );
		}

		public void TorneoModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals( nameof( TorneoViewModel.PrecioBuyin ) ))
			{
				Buyin = (sender as TorneoViewModel).PrecioBuyin;
			}
		}

		private int entradas;
		public int Entradas
		{
			get { return entradas; }
			set { SetProperty( ref entradas, value ); }
		}


		private int rebuys;
		public int Rebuys
		{
			get { return rebuys; }
			set { SetProperty( ref rebuys, value ); }
		}


		private int buyin;
		public int Buyin
		{
			get { return buyin; }
			set { SetProperty( ref buyin, value ); }
		}

		private int bolsa;
		public int Bolsa
		{
			get { return bolsa; }
			set { SetProperty( ref bolsa, value ); }
		}


		private List<JugadorViewModel> jugadores;
		public List<JugadorViewModel> Jugadores
		{
			get { return jugadores; }
			set { SetProperty( ref jugadores, value ); }
		}

		private ObservableCollection<Resultado> resultados;
		public ObservableCollection<Resultado> Resultados
		{
			get { return resultados; }
		}

		public List<Resultado> AsList()
		{
			return Resultados.ToList();
		}

	}


	public class Resultado : ViewModel
	{
		public Resultado()
		{
			Premio = "0";
			Puntualidad = true;
		}

		private string jugador;
		public string Jugador
		{
			get { return jugador; }
			set { SetProperty( ref jugador, value ); }
		}

		private int posicion;
		public int Posicion
		{
			get { return posicion; }
			set { SetProperty( ref posicion, value ); }
		}

		private int rebuys;
		public int Rebuys
		{
			get { return rebuys; }
			set { SetProperty( ref rebuys, value ); }
		}

		private string premio;
		public string Premio
		{
			get { return premio; }
			set { SetProperty( ref premio, value ); }
		}

		private bool puntualidad;
		public bool Puntualidad
		{
			get { return puntualidad; }
			set { SetProperty( ref puntualidad, value ); }
		}

		private bool isOver;
		public bool IsOver
		{
			get { return isOver; }
			set { SetProperty( ref isOver, value ); }
		}

	}

}