using Joserras.Client.Torneo.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Joserras.Client.Torneo.Model
{
	public class ResultadosViewModel : ViewModel
	{
		public ResultadosViewModel()
		{
			resultados = new ObservableCollection<Resultado>();
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

	}


	public class Resultado : ViewModel
	{
		public Resultado()
		{
			Premio = "0";
			Puntualidad = true;
		}

		private JugadorViewModel jugador;
		public JugadorViewModel Jugador
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

	}

}