using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using HomeServer.Formula1Api.DTOs;

namespace HomeServer.Formula1Api
{
    /// <summary>
    /// Manages interactions between this program and the OpenF1 API.
    /// </summary>
    public class OpenF1Client
    {
        private const int _RateLimit = 2000;
        private const string _BaseAddress = "OpenF1";
        private ILogger<OpenF1Client> _logger;
        private IHttpClientFactory _clientFactory;
        private HttpClient _client;

        public OpenF1Client(ILogger<OpenF1Client> logger, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _logger = logger;

            _client = clientFactory.CreateClient(_BaseAddress);
        }
        
        /// <summary>
        /// Performs an HTTP request on the passed in endpoint.  
        /// Returns the generic result of the HTTP request.
        /// </summary>
        public async Task<T?> QueryApiAsync<T>(string endpoint)
        {
            HttpResponseMessage resp = await _client.GetAsync(endpoint);

            if (resp.Content is null)
            {
                throw new Exception($"Failed to receive response from API.");
            }

            string json = await resp.Content.ReadAsStringAsync();
            T? dto = default;

            try
            {
                // Attempting to deserialize the json string.
                dto = JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("There was an error while deserializing an OpenF1 response object. Error: {err}", 
                    ex.Message);

                ErrorDto? err = JsonSerializer.Deserialize<ErrorDto>(json);
                
                // If the API returned the error, continue onward.
                if (err is not null)
                {
                    _logger.LogWarning("There was an error response from the API.");
                    return default;
                }
                // Otherwise, some other genuine error occurred.
                else
                {
                    _logger.LogCritical("A serious error has occured while processing an OpenF1 API request.");
                }
            }

            if (dto is null)
            {
                throw new Exception("Failed to deserialize lap data.");
            }

            // Adding a task delay for OpenF1's rate limiting.
            await Task.Delay(_RateLimit);
            return dto;
        }
    }
}