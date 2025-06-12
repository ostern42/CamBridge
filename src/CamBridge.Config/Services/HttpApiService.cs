// src/CamBridge.Config/Services/HttpApiService.cs
// Version: 0.7.8
// Description: KISS Edition - Dead Letter methods removed!
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
    /// KISS implementation of IApiService - no dead letters!
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

        public async Task<bool> IsServiceAvailableAsync()
        {
            return await TryGetAsync<object>("health") != null;
        }

        public async Task<DetailedStatisticsModel?> GetStatisticsAsync()
        {
            // KISS: Not implemented yet, return null
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
    }
}
