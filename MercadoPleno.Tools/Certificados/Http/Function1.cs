using MercadoPleno.Tools.Application.Abstracao;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MercadoPleno.Tools
{
	public class Function1 : BaseClass
	{
		private readonly ILogger<Function1> _logger;

		public Function1(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			_logger = GetService<ILogger<Function1>>();
		}

		[FunctionName("Function1")]
		[OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
		[OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
		[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
		public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
		{


			return new OkObjectResult("");
		}
	}
}