using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text.Json;

namespace MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos
{
	public enum ZeroSslStatus
	{
		Draft,
		Pending_validation,
		Issued,
		Cancelled,
		Expiring_soon,
		Expired
	}

	public class ZeroSslCertificado : ZeroSslResponse
	{
		public string Id { get; set; }
		public string Type { get; set; }
		public string Common_name { get; set; }
		public string Additional_domains { get; set; }
		public string Created { get; set; }
		public string Expires { get; set; }

		[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
		public ZeroSslStatus Status { get; set; }
		public object Validation_type { get; set; }
		public object Validation_emails { get; set; }
		public string Replacement_for { get; set; }
		public Validation Validation { get; set; }

		public string GetValidationEMail()
		{
			var emailValidation = (JsonElement)Validation.Email_validation;
			var emails = emailValidation.GetProperty(Common_name);
			return emails[0].GetString();
		}
	}
}