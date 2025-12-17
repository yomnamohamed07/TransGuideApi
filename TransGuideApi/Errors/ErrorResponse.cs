namespace TransGuideApi.Errors
{
	public class ErrorResponse
	{
		public int _statuescode { get; set; }
		public string? _errormessage { get; set; }
		public ErrorResponse(int statuescode, string errormessage = null)
		{
			_statuescode = statuescode;
			_errormessage = errormessage ?? Geterrormessageforresponsecode(_statuescode);
		}
		public string Geterrormessageforresponsecode(int _statuescode)
		{
			return _statuescode switch
			{
				404 => "Resource Not Found",
				400 => "Bad Request",
				500 => "Internet Server Error",
				401 => "you are not Authorized",
				_ => null

			};
		}
	}
}
