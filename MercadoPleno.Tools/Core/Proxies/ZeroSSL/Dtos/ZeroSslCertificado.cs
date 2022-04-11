using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Text.Json;

namespace MercadoPleno.Tools.Core.Proxies.ZeroSSL.Dtos
{
	public enum ZeroSslStatus
	{
		Undefined = 0,
		Draft = 1,
		Pending_validation = 2,
		Issued = 3,
		Expiring_soon = 4,
		Expired = 5,
		Revoked = 6,
		Cancelled = 7,
	}

	public class ZeroSslCertificado : ZeroSslResponse
	{
		public string Id { get; set; }
		public string Type { get; set; }

		[JsonProperty("common_name")]
		public string Domain { get; set; }
		public string Additional_domains { get; set; }
		public DateTime Created { get; set; }
		public DateTime Expires { get; set; }

		[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
		public ZeroSslStatus Status { get; set; }
		public object Validation_type { get; set; }
		public object Validation_emails { get; set; }
		public string Replacement_for { get; set; }
		public Validation Validation { get; set; }

		public string GetValidationEMail()
		{
			var emailValidation = (JObject)Validation.Email_validation;
			var emails = (JArray)emailValidation.GetValue(Domain);
			return emails[0].Value<string>();
		}
	}
}