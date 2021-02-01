﻿using Google.Apis.Auth.OAuth2;
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

namespace TorneosWeb.service.impl
{
	public class GoogleSheetsProfitsExporter : IProfitsExporter
	{
		private const int DATES_FOR_REPORT = 8;
		private readonly string SHEET = "Deudas";

		private string CredentialFileName;
		private readonly string SpreadsheetId;
		private readonly SheetsService sheetsService;
		private ILogger<GoogleSheetsProfitsExporter> log;
		private IReadService readService;

		static string[] Scopes = { SheetsService.Scope.Spreadsheets };

		public GoogleSheetsProfitsExporter(string credentialFileName, string spreadsheetId, ILogger<GoogleSheetsProfitsExporter> logger,
			IReadService readService)
		{
			CredentialFileName = credentialFileName;
			SpreadsheetId = spreadsheetId;
			log = logger;
			this.readService = readService;

			try
			{
				GoogleCredential credential =
					GoogleCredential.FromStream( new FileStream( CredentialFileName, FileMode.Open ) ).CreateScoped( Scopes );

				sheetsService = new SheetsService( new BaseClientService.Initializer()
				{
					HttpClientInitializer = credential,
					ApplicationName = "TorneosJoserras",
				} );

				log.LogDebug( "Google sheets service succesfully created!" );
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
			log.LogDebug( "Profits Exported successfully!" );
		}

		private void InsertProfitData(List<Torneo> torneos)
		{
			IList<object> firstRowDates = CreateDatesRow( torneos );

			SortedDictionary<string, ProfitRow> rowValues = new SortedDictionary<string, ProfitRow>();
			CreateRowValues( rowValues, torneos );

			string range = string.Format( "{0}!A2:H{1}", SHEET, rowValues.Count + 1 );
			IList<IList<object>> rows = new List<IList<object>>();
			foreach( ProfitRow profitRow in rowValues.Values )
			{
				IList<object> row = new List<object>();
				row.Add( profitRow.Nombre );
				foreach( Torneo torneo in torneos )
				{
					KeyValuePair<Guid, decimal> pair = profitRow.Profits.Where( p => p.Key.Equals( torneo.Id ) ).FirstOrDefault();
					if(pair.Equals(default( KeyValuePair<Guid, int> ) ) )
					{
						row.Add( "" );
					}
					else
					{
						row.Add( pair.Value );
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
			foreach(Torneo torneo in torneos )
			{
				Resultados resultados = readService.FindResultadosTorneo( torneo.Id );
				foreach(Posicion posicion in resultados.Posiciones )
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
					profitRow.Profits.Add( new KeyValuePair<Guid, decimal>( torneo.Id, posicion.ProfitNumber ) );
				}
			}
		}

		private IList<object> CreateDatesRow(List<Torneo> torneos)
		{
			List<object> datesRow = new List<object>();
			foreach(Torneo t in torneos )
			{
				datesRow.Add( t.Fecha );
			}

			IList<IList<object>> rows = new List<IList<object>>();
			rows.Add( datesRow );
			ValueRange valueRequest = new ValueRange
			{
				Range = string.Format( "{0}!B1:H1", SHEET ),
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
			int rowsToClean = readService.GetAllJugadores().Count + 1;

			string range = string.Format("{0}!A1:H{1}", SHEET, rowsToClean);
			IList<object> cellValues;
			IList<IList<object>> rows = new List<IList<object>>();
			for(int i = 1; i <= rowsToClean;  i++ )
			{
				cellValues = new List<object>();
				for(int c = 0; c < 8; c++ )
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