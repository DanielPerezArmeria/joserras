namespace TorneosWeb.domain.models
{
	public class StatProps
	{
		public string Nombre { get; set; }
		public string Valor { get; set; }
		public bool IsPositive { get; set; }

		public StatProps()
		{
			IsPositive = true;
		}

		public StatProps(string nombre, string valor)
		{
			Nombre = nombre;
			Valor = valor;
			IsPositive = true;
		}

		public StatProps(string nombre, string valor, bool pos)
		{
			Nombre = nombre;
			Valor = valor;
			IsPositive = pos;
		}

	}

}