// src\CamBridge.Config\Services\HttpApiService.cs
// Version: 0.6.12
// Description: KISS Edition - Removed over-engineering, dead code eliminated
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
    /// KISS implementation of IApiService - no over-engineering!
    /// </summary>
    public class HttpApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public HttpApiService(HttpClient httpClient, object? unused = null)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5050/");
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
            // No custom JSON options - defaults work fine!
            // Note: Second parameter kept for backward compatibility, but ignored (KISS!)
        }

        public async Task<ServiceStatusModel?> GetStatusAsync()
        {
            return await TryGetAsync<ServiceStatusModel>("api/status");
        }

        public async Task<List<DeadLetterItemModel>?> GetDeadLettersAsync()
        {
            return await TryGetAsync<List<DeadLetterItemModel>>("api/deadletters")
                   ?? new List<DeadLetterItemModel>();
        }

        public async Task<bool> ReprocessDeadLetterAsync(Guid id)
        {
            return await TryPostAsync($"api/deadletters/{id}/reprocess");
        }

        public async Task<bool> IsServiceAvailableAsync()
        {
            return await TryGetAsync<object>("health") != null;
        }

        public async Task<DetailedStatisticsModel?> GetStatisticsAsync()
        {
            // KISS: Not implemented, return empty (interface compatibility)
            await Task.CompletedTask;
            return null;
        }

        /// <summary>
        /// KISS Helper: One method to rule them all!
        /// </summary>
        private async Task<T?> TryGetAsync<T>(string endpoint) where T : class
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json); // Default options work!
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"API call failed ({endpoint}): {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// KISS Helper: Simple POST
        /// </summary>
        private async Task<bool> TryPostAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.PostAsync(endpoint, null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"API POST failed ({endpoint}): {ex.Message}");
                return false;
            }
        }
    }
}
