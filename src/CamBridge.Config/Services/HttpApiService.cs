// src/CamBridge.Config/Services/HttpApiService.cs
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CamBridge.Config.Models;
using Microsoft.Extensions.Logging;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Service for communicating with CamBridge Service API
    /// </summary>
    public class HttpApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpApiService> _logger;
        private readonly string _baseUrl;

        public HttpApiService(HttpClient httpClient, ILogger<HttpApiService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _baseUrl = "http://localhost:5050/api";

            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Gets the current service status
        /// </summary>
        public async Task<ServiceStatusModel?> GetStatusAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("status");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ServiceStatusModel>();
                }

                _logger.LogWarning("Failed to get status: {StatusCode}", response.StatusCode);
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error getting status");
                return null;
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("Request timeout getting status");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting status");
                return null;
            }
        }

        /// <summary>
        /// Gets detailed statistics
        /// </summary>
        public async Task<DetailedStatisticsModel?> GetStatisticsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("status/statistics");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<DetailedStatisticsModel>();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics");
                return null;
            }
        }

        /// <summary>
        /// Gets dead letter items
        /// </summary>
        public async Task<List<DeadLetterItemModel>?> GetDeadLettersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("status/deadletters");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<DeadLetterItemModel>>();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dead letters");
                return null;
            }
        }

        /// <summary>
        /// Reprocesses a dead letter item
        /// </summary>
        public async Task<bool> ReprocessDeadLetterAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.PostAsync($"status/deadletters/{id}/reprocess", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reprocessing dead letter {Id}", id);
                return false;
            }
        }

        /// <summary>
        /// Checks if the service is reachable
        /// </summary>
        public async Task<bool> IsServiceAvailableAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("status/health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Interface for API service
    /// </summary>
    public interface IApiService
    {
        Task<ServiceStatusModel?> GetStatusAsync();
        Task<DetailedStatisticsModel?> GetStatisticsAsync();
        Task<List<DeadLetterItemModel>?> GetDeadLettersAsync();
        Task<bool> ReprocessDeadLetterAsync(Guid id);
        Task<bool> IsServiceAvailableAsync();
    }
}
