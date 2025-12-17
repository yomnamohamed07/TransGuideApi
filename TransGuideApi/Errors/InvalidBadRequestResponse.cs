namespace TransGuideApi.Errors
{
	public class InvalidBadRequestResponse : ErrorResponse
	{
		public IEnumerable<string> Errors { get; set; }
		public InvalidBadRequestResponse() : base(400)
		{
			Errors = new List<string>();
		}
	}
}
