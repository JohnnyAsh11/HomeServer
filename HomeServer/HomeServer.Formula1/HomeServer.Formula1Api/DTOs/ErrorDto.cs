using System.Text.Json.Serialization;

namespace HomeServer.Formula1Api.DTOs
{
    /// <summary>
    /// OpenF1 API 404 Error DTO.
    /// </summary>
    public class ErrorDto
    {
        [JsonPropertyName("detail")]
        public string? Detail { get; set; } = string.Empty;
    }
}
