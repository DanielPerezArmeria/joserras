using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TorneosWeb.domain;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.service.impl
{
	public class GoogleSheetsProfitsExporter : IProfitsExporter
	{
		private readonly string SHEET = "Deudas";

		private readonly string SpreadsheetId;
		private readonly SheetsService sheetsService;
		private ILogger<GoogleSheetsProfitsExporter> log;
		private IReadService readService;
		private ILigaReader ligaReader;
		private IJugadorService jugadorService;

		static string[] Scopes = { SheetsService.Scope.Spreadsheets };

		public GoogleSheetsProfitsExporter(ISecretsManager secretsManager, ILogger<GoogleSheetsProfitsExporter> logger,
			IReadService readService, ILigaReader ligaReader, IJugadorService jugadorService)
		{
			SpreadsheetId = secretsManager.GetSecret( "spreadsheetId" );
			log = logger;
			this.readService = readService;
			this.ligaReader = ligaReader;
			this.jugadorService = jugadorService;

			try
			{
				string googleSheetCredentials = secretsManager.GetSecret( "googleSheetCredentials" );
				GoogleCredential credential =
					GoogleCredential.FromStream( new MemoryStream( googleSheetCredentials.ToBytes() ) ).CreateScoped( Scopes );

				sheetsService = new SheetsService( new BaseClientService.Initializer()
				{
					HttpClientInitializer = credential,
					ApplicationName = "TorneosJoserras",
				} );

				log.LogInformation( "Google sheets service succesfully created!" );
			}
			catch( Exception e )
			{
				log.LogError( e, e.Message );
				throw;
			}
		}

		public void ExportProfits(List<Torneo> torneos)
		{
			torneos.Reverse();
			CleanRows();
			InsertProfitData( torneos );
			log.LogInformation( "Profits Exported successfully!" );
		}

		private void InsertProfitData(List<Torneo> torneos)
		{
			List<object> firstRowDates = CreateDatesRow( torneos );

			SortedDictionary<string, ProfitRow> rowValues = new SortedDictionary<string, ProfitRow>();
			CreateRowValues( rowValues, torneos );

			string range = string.Format( "{0}!D2:O{1}", SHEET, rowValues.Count + 1 );
			IList<IList<object>> rows = new List<IList<object>>();
			foreach( ProfitRow profitRow in rowValues.Values )
			{
				IList<object> row = new List<object>();
				row.Add( profitRow.Nombre );
				foreach( string fecha in firstRowDates )
				{
					if(!profitRow.Profits.ContainsKey(fecha) )
					{
						row.Add( "" );
					}
					else
					{
						row.Add( profitRow.Profits[fecha] );
					}
				}

				Liga liga = torneos.FirstOrDefault( t => t.Liga?.FechaCierreDate != null )?.Liga;
				if(liga != null )
				{
					if( !profitRow.Profits.ContainsKey(liga.Id.ToString()) )
					{
						row.Add( "" );
					}
					else
					{
						row.Add( profitRow.Profits[ liga.Id.ToString() ] );
					}
				}

				rows.Add( row );
			}
			ValueRange valueRequest = new ValueRange
			{
				Range = range,
				MajorDimension = "ROWS",
				Values = rows
			};
			SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest =
				sheetsService.Spreadsheets.Values.Update( valueRequest, SpreadsheetId, range );
			updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
			UpdateValuesResponse updateResponse = updateRequest.Execute();
		}

		private void CreateRowValues(SortedDictionary<string, ProfitRow> rowValues, List<Torneo> torneos)
		{
			foreach( Torneo torneo in torneos )
			{
				Resultados resultados = readService.FindResultadosTorneo( torneo.Id );
				foreach( Posicion posicion in resultados.Posiciones )
				{
					ProfitRow profitRow = null;
					if( rowValues.ContainsKey( posicion.Nombre ) )
					{
						profitRow = rowValues[ posicion.Nombre ];
					}
					else
					{
						profitRow = new ProfitRow();
						profitRow.Nombre = posicion.Nombre;
						rowValues.Add( posicion.Nombre, profitRow );
					}

					if( profitRow.Profits.ContainsKey( torneo.Fecha ) )
					{
						profitRow.Profits[ torneo.Fecha ] = profitRow.Profits[ torneo.Fecha ] + posicion.ProfitTotalNumber;
					}
					else
					{
						profitRow.Profits.Add( torneo.Fecha, posicion.ProfitTotalNumber );
					}
				}
			}

			Liga liga = torneos.FirstOrDefault( t => t.Liga?.FechaCierreDate != null )?.Liga;
			if(liga != null )
			{
				liga = ligaReader.FindLigaByNombre( liga.Nombre );
				List<Standing> standings = ligaReader.GetStandings( liga );
				foreach(Standing standing in standings )
				{
					ProfitRow profitRow = null;
					if( rowValues.ContainsKey( standing.Jugador ) )
					{
						profitRow = rowValues[ standing.Jugador ];
					}
					else
					{
						profitRow = new ProfitRow();
						profitRow.Nombre = standing.Jugador;
						rowValues.Add( standing.Jugador, profitRow );
					}
					profitRow.Profits.Add( liga.Id.ToString(), standing.PremioLigaNumber );
				}
			}
		}

		private List<object> CreateDatesRow(List<Torneo> torneos)
		{
			List<object> datesRow = new List<object>();
			foreach(Torneo t in torneos )
			{
				if(!datesRow.Contains( t.Fecha ) )
				{
					datesRow.Add( t.Fecha );
				}
			}

			if( torneos.Any( t => t.Liga != null ) )
			{
				datesRow.Add( "Liga" );
			}

			IList<IList<object>> rows = new List<IList<object>>();
			rows.Add( datesRow );
			ValueRange valueRequest = new ValueRange
			{
				Range = string.Format( "{0}!E1:O1", SHEET ),
				MajorDimension = "ROWS",
				Values = new List<IList<object>>( rows )
			};

			SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest =
				sheetsService.Spreadsheets.Values.Update( valueRequest, SpreadsheetId, valueRequest.Range );
			updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
			UpdateValuesResponse updateResponse = updateRequest.Execute();

			return datesRow;
		}

		private void CleanRows()
		{
			int rowsToClean = jugadorService.GetAllJugadores().Count + 1;

			string range = string.Format("{0}!D1:O{1}", SHEET, rowsToClean);
			IList<object> cellValues;
			IList<IList<object>> rows = new List<IList<object>>();
			for(int i = 1; i <= rowsToClean;  i++ )
			{
				cellValues = new List<object>();
				for(int c = 0; c < 11; c++ )
				{
					cellValues.Add( string.Empty );
				}
				rows.Add( cellValues );
			}

			ValueRange valueRequest = new ValueRange
			{
				Range = range,
				MajorDimension = "ROWS",
				Values = rows
			};
			SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest =
				sheetsService.Spreadsheets.Values.Update( valueRequest, SpreadsheetId, range );
			updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
			UpdateValuesResponse updateResponse = updateRequest.Execute();
		}

	}

}