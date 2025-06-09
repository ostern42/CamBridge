// src\CamBridge.Config\Services\HttpApiService.cs
// Version: 0.6.11
// Description: Simplified HTTP API service for CamBridge Service communication
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CamBridge.Config.Models;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Simplified implementation of IApiService - no over-engineering!
    /// </summary>
    public class HttpApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public HttpApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5050/");
            _httpClient.Timeout = TimeSpan.FromSeconds(5);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ServiceStatusModel?> GetStatusAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/status");
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                var status = JsonSerializer.Deserialize<ServiceStatusModel>(json, _jsonOptions);

                // Fill in any missing data
                if (status != null)
                {
                    status.Timestamp = DateTime.UtcNow;
                    status.Status = "Running"; // If we got a response, service is running
                }

                return status;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetStatusAsync failed: {ex.Message}");
                return null;
            }
        }

        public async Task<DetailedStatisticsModel?> GetStatisticsAsync()
        {
            // Not implemented in current API - return empty
            await Task.CompletedTask;
            return new DetailedStatisticsModel
            {
                TopErrors = new Dictionary<string, int>(),
                ErrorCategories = new Dictionary<string, int>()
            };
        }

        public async Task<List<DeadLetterItemModel>?> GetDeadLettersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/deadletters");
                if (!response.IsSuccessStatusCode) return new List<DeadLetterItemModel>();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<DeadLetterItemModel>>(json, _jsonOptions)
                    ?? new List<DeadLetterItemModel>();
            }
            catch
            {
                return new List<DeadLetterItemModel>();
            }
        }

        public async Task<bool> ReprocessDeadLetterAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/deadletters/{id}/reprocess", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsServiceAvailableAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Extension method for mock status when service is offline
        public static ServiceStatusModel GetOfflineStatus()
        {
            return new ServiceStatusModel
            {
                Status = "Offline",
                ServiceStatus = "Service Offline",
                Timestamp = DateTime.UtcNow,
                QueueLength = 0,
                ActiveProcessing = 0,
                TotalProcessed = 0,
                TotalSuccessful = 0,
                TotalFailed = 0,
                SuccessRate = 0,
                ProcessingRate = 0,
                Uptime = TimeSpan.Zero,
                DeadLetterCount = 0,
                ActiveItems = new List<ActiveItemModel>()
            };
        }
    }
}
