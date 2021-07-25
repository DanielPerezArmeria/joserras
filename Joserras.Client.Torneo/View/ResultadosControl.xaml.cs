using Joserras.Client.Torneo.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Joserras.Client.Torneo.View
{
	/// <summary>
	/// Interaction logic for ResultadosControl.xaml
	/// </summary>
	public partial class ResultadosControl : UserControl
	{
		public ResultadosControl()
		{
			InitializeComponent();
		}

		private int prevRowIndex = -1;

		public delegate Point GetDragDropPosition(IInputElement theElement);

		private bool IsTheMouseOnTargetRow(Visual theTarget, GetDragDropPosition pos)
		{
			Rect posBounds = VisualTreeHelper.GetDescendantBounds( theTarget );
			Point theMousePos = pos( (IInputElement)theTarget );
			return posBounds.Contains( theMousePos );
		}

		private DataGridRow GetDataGridRowItem(int index)
		{
			if(dg.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
			{
				return null;
			}

			return dg.ItemContainerGenerator.ContainerFromIndex( index ) as DataGridRow;
		}

		private int GetDataGridItemCurrentRowIndex(GetDragDropPosition pos)
		{
			int curIndex = -1;
			for(int i = 0; i < dg.Items.Count; i++)
			{
				DataGridRow item = GetDataGridRowItem( i );
				if (IsTheMouseOnTargetRow( item, pos ))
				{
					curIndex = i;
					break;
				}
			}

			return curIndex;
		}

		private void dg_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			prevRowIndex = GetDataGridItemCurrentRowIndex( e.GetPosition );
			if(prevRowIndex < 0)
			{
				return;
			}

			dg.SelectedIndex = prevRowIndex;

			Resultado resultado = dg.Items[prevRowIndex] as Resultado;
			if(resultado == null)
			{
				return;
			}

			DragDropEffects dragdropeffects = DragDropEffects.Move;
			if(DragDrop.DoDragDrop(dg, resultado, dragdropeffects ) != DragDropEffects.None)
			{
				dg.SelectedItem = resultado;
			}
		}

		private void dg_Drop(object sender, DragEventArgs e)
		{
			if(prevRowIndex < 0)
			{
				return;
			}

			int index = GetDataGridItemCurrentRowIndex( e.GetPosition );
			if(index < 0 || index == prevRowIndex)
			{
				return;
			}
			
			if(index == dg.Items.Count - 1)
			{
				MessageBox.Show( "This row-index cannot be used for Drop operations." );
				return;
			}

			ObservableCollection<Resultado> resultados = (DataContext as ResultadosViewModel).Resultados;
			Resultado movedResult = resultados[prevRowIndex];
			resultados.RemoveAt( prevRowIndex );
			resultados.Insert( index, movedResult );
		}

		private void delButton_Click(object sender, RoutedEventArgs e)
		{
			Resultado resultado = ((Button)sender).DataContext as Resultado;
			if(resultado == null)
			{
				return;
			}

			(DataContext as ResultadosViewModel).Resultados.Remove( resultado );
		}

	}

}