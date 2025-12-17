namespace TransGuideApi.Errors
{
	public class ApiExceptionResponse :ErrorResponse
	{
		public string? _details { get; set; }
		public ApiExceptionResponse(int statuescode, string? errormessage = null, string? details = null) : base(statuescode, errormessage)
		{
			_details = details;
		}
	}
}
