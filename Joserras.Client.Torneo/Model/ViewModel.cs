namespace Joserras.Client.Torneo.Model
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


  public abstract class ViewModel : ViewModel<int>
  {

  }

}
