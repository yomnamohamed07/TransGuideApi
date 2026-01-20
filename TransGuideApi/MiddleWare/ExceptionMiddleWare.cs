using TransGuideApi.Errors;
using System.Net;
using System.Text.Json;

namespace TransGuideApi.MiddleWare
{
	public class ExceptionMiddleWare
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleWare> _logger;
		private readonly IHostEnvironment _env;
		public ExceptionMiddleWare(RequestDelegate next, ILogger<ExceptionMiddleWare> logger, IHostEnvironment env)
		{
			_next = next;
			_logger = logger;
			_env = env;
		}
		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next.Invoke(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				context.Response.ContentType = "application/json";
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				if (_env.IsDevelopment())
				{
					var response = new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace);
					var Options = new JsonSerializerOptions
					{
						PropertyNamingPolicy = JsonNamingPolicy.CamelCase
					};
					var jsonresponse = JsonSerializer.Serialize(response, Options);
					//var jsonresponse = JsonSerializer.Serialize(response);
					context.Response.WriteAsync(jsonresponse);
				}
				else
				{
					var response = new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
					var Options = new JsonSerializerOptions
					{
						PropertyNamingPolicy = JsonNamingPolicy.CamelCase
					};
					var jsonresponse = JsonSerializer.Serialize(response, Options);

					context.Response.WriteAsync(jsonresponse);
				}
			}
		}
	}
}
