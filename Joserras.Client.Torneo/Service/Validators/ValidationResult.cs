namespace Joserras.Client.Torneo.Service.Validators
{
	public class ValidationResult
	{
		public ValidationResult()
		{
			IsValid = true;
		}

		public bool IsValid { get; set; }
		public string Message { get; set; }
	}

}