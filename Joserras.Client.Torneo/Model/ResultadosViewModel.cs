using Humanizer;
using Joserras.Client.Torneo.Domain;
using Joserras.Client.Torneo.Properties;
using Joserras.Client.Torneo.Service;
using Joserras.Client.Torneo.Utils;
using Joserras.Commons.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Joserras.Client.Torneo.Model
{
	public class ResultadosViewModel : ViewModel
	{
		private readonly IHttpService httpService;

		public ResultadosViewModel(IHttpService httpService)
		{
			this.httpService = httpService;

			resultados = new ObservableCollection<Resultado>();
			Resultados.CollectionChanged += Resultados_CollectionChanged;

			OcPropertyChangedListener<Resultado> rebuysListener =
				OcPropertyChangedListener.Create( Resultados, nameof( Resultado.Rebuys ) );

			rebuysListener.PropertyChanged += RebuysListener_PropertyChanged;
			UpdateBolsa();

			CalcularPremiosCommand = new DelegateCommand( CalculatePrizes, CanCalculatePrizes );

			AllEnabled = true;
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
			else if (e.PropertyName.Equals( nameof( TorneoViewModel.Liga ) ))
			{
				Liga = (sender as TorneoViewModel).Liga;
			}
		}

		public IEnumerable<PrizeRange> PrizeRanges { get; set; }

		private List<PrizeEntry> premios;
		public List<PrizeEntry> Premios
		{
			get { return premios; }
			set{ SetProperty( ref premios, value ); }
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
			set
			{
				SetProperty( ref bolsa, value );
			}
		}

		private bool isRebuyPeriodOver;
		public bool IsRebuyPeriodOver
		{
			get { return isRebuyPeriodOver; }
			set
			{
				SetProperty( ref isRebuyPeriodOver, value );
				AllEnabled = !isRebuyPeriodOver;
				CalcularPremiosCommand.RaiseCanExecuteChanged();
			}
		}

		private bool allEnabled;
		public bool AllEnabled
		{
			get { return allEnabled; }
			set { SetProperty( ref allEnabled, value ); }
		}


		private DelegateCommand calcularPremiosCommand;
		public DelegateCommand CalcularPremiosCommand
		{
			get { return calcularPremiosCommand; }
			set { SetProperty( ref calcularPremiosCommand, value ); }
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

		private bool CanCalculatePrizes(object arg)
		{
			return IsRebuyPeriodOver;
		}

		private bool liga;
		public bool Liga
		{
			get { return liga; }
			set { SetProperty( ref liga, value ); }
		}


		private async void CalculatePrizes()
		{
			PrizeRange range = PrizeRanges.SingleOrDefault( p => Entradas >= p.Menor && Entradas <= p.Mayor );
			string apiCall = string.Format( Resources.API_GET_PRIZES, range.Premiacion, Buyin, Liga, Bolsa );

			IDictionary<int, string> prizes = await httpService.GetAsync<IDictionary<int, string>>( apiCall );
			List<PrizeEntry> list = prizes.Select( p =>
					new PrizeEntry { Lugar = p.Key, LugarString = p.Key.Ordinalize(),
						Premio = string.Format( decimal.Parse( p.Value ).ToString( "$###,##0.0#" ) ) } ).ToList();
			list.Sort();
			Premios = list;
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

	public class PrizeEntry : IComparable<PrizeEntry>
	{
		public int Lugar { get; set; }
		public string LugarString { get; set; }
		public string Premio { get; set; }

		public int CompareTo(PrizeEntry other)
		{
			if(Lugar > other.Lugar)
			{
				return 1;
			}
			else if(Lugar < other.Lugar)
			{
				return -1;
			}
			return 0;
		}

	}

}