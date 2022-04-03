using Newtonsoft.Json;

namespace KLog.DataModel.DTOs.GitHub
{
    public class Alert
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("affected_range")]
        public string AffectedRange { get; set; }
        [JsonProperty("affected_package_name")]
        public string AffectedPackage { get; set; }
        [JsonProperty("fixed_in")]
        public string FixedIn { get; set; }
        [JsonProperty("external_reference")]
        public string Reference { get; set; }
        [JsonProperty("external_identifier")]
        public string Identifier { get; set; }
        [JsonProperty("severity")]
        public string Severity { get; set; }
        [JsonProperty("ghsa_id")]
        public string GHSAId { get; set; }
        [JsonProperty("created_at")]
        public DateTimeOffset? Created { get; set; }
    }
}
