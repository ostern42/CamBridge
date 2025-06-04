// File: src/CamBridge.Config/Services/HttpApiService.cs
// Version: 0.5.24
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions
// Modified: 2025-06-04
// Status: Development

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CamBridge.Config.Models;
using Microsoft.Extensions.Logging;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Implementation of IApiService for HTTP communication with CamBridge Service
    /// </summary>
    public class HttpApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpApiService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public HttpApiService(HttpClient httpClient, ILogger<HttpApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Set base address
            _httpClient.BaseAddress = new Uri("http://localhost:5050/");
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        /// <summary>
        /// Gets the current service status
        /// </summary>
        public async Task<ServiceStatusModel?> GetStatusAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/status");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    // Parse the actual API response
                    var apiResponse = JsonSerializer.Deserialize<ApiStatusResponse>(json, _jsonOptions);

                    if (apiResponse != null)
                    {
                        // Map to our ServiceStatusModel
                        return new ServiceStatusModel
                        {
                            ServiceStatus = apiResponse.ServiceStatus ?? "Unknown",
                            Timestamp = DateTime.UtcNow,
                            QueueLength = apiResponse.QueueLength,
                            ActiveProcessing = apiResponse.ActiveProcessing,
                            TotalProcessed = apiResponse.TotalSuccessful + apiResponse.TotalFailed,
                            TotalSuccessful = apiResponse.TotalSuccessful,
                            TotalFailed = apiResponse.TotalFailed,
                            SuccessRate = apiResponse.SuccessRate,
                            ProcessingRate = 0, // Not provided by API
                            Uptime = apiResponse.Uptime ?? TimeSpan.Zero,
                            DeadLetterCount = apiResponse.DeadLetterCount,
                            ActiveItems = new List<ActiveItemModel>() // Not provided by API
                        };
                    }
                }

                _logger.LogWarning("Failed to get service status: {StatusCode}", response.StatusCode);
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogDebug(ex, "Failed to get service status");
                return null;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogDebug(ex, "Service status request timed out");
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize service status");
                return null;
            }
        }

        /// <summary>
        /// Gets detailed statistics
        /// </summary>
        public async Task<DetailedStatisticsModel?> GetStatisticsAsync()
        {
            // This endpoint doesn't exist in the current API
            // Return empty statistics for now
            await Task.CompletedTask;
            return new DetailedStatisticsModel
            {
                TopErrors = new Dictionary<string, int>(),
                ErrorCategories = new Dictionary<string, int>()
            };
        }

        /// <summary>
        /// Gets dead letter items
        /// </summary>
        public async Task<List<DeadLetterItemModel>?> GetDeadLettersAsync()
        {
            // This endpoint doesn't exist in the current API
            // Return empty list for now
            await Task.CompletedTask;
            return new List<DeadLetterItemModel>();
        }

        /// <summary>
        /// Reprocesses a dead letter item
        /// </summary>
        public async Task<bool> ReprocessDeadLetterAsync(Guid id)
        {
            // This endpoint doesn't exist in the current API
            await Task.CompletedTask;
            return false;
        }

        /// <summary>
        /// Checks if the service is reachable
        /// </summary>
        public async Task<bool> IsServiceAvailableAsync()
        {
            try
            {
                // Use the health endpoint
                var response = await _httpClient.GetAsync("health");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogDebug(ex, "Service not reachable");
                return false;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogDebug(ex, "Service request timed out");
                return false;
            }
        }

        /// <summary>
        /// Internal model to match the actual API response
        /// </summary>
        private class ApiStatusResponse
        {
            public string? ServiceStatus { get; set; }
            public string? Version { get; set; }
            public TimeSpan? Uptime { get; set; }
            public int QueueLength { get; set; }
            public int ActiveProcessing { get; set; }
            public int TotalSuccessful { get; set; }
            public int TotalFailed { get; set; }
            public double SuccessRate { get; set; }
            public int DeadLetterCount { get; set; }
            public ApiConfigurationInfo? Configuration { get; set; }
        }

        private class ApiConfigurationInfo
        {
            public int WatchFolders { get; set; }
            public string? OutputFolder { get; set; }
        }
    }
}
