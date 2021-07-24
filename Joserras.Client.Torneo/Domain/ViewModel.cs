namespace Joserras.Client.Torneo.Domain
{
	public abstract class ViewModel<K> : BindableBase
	{
		protected K id;
		public K Id
		{
			get { return id; }
			set
			{
				SetProperty( ref id, value );
			}
		}

	}
}

namespace Joserras.Client.Torneo.Domain
{
	public abstract class ViewModel : ViewModel<int>
	{

	}

}
