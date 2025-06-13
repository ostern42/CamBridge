/**************************************************************************
*  HttpApiService.cs                                                      *
*  PATH: src\CamBridge.Config\Services\HttpApiService.cs                  *
*  VERSION: 0.7.11 | SIZE: ~7KB | MODIFIED: 2025-06-13                   *
*                                                                         *
*  DESCRIPTION: HTTP client for CamBridge Service API with PORT FIX      *
*  Copyright (c) 2025 Claude's Improbably Reliable Software Solutions     *
**************************************************************************/

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
    /// KISS implementation of IApiService - now with correct port!
    /// </summary>
    public class HttpApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public HttpApiService(HttpClient httpClient, object? unused = null)
        {
            _httpClient = httpClient;
            // CRITICAL FIX: Use port 5111 to match Service configuration!
            _httpClient.BaseAddress = new Uri("http://localhost:5111/"); // FIX: Was 5050!
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
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
                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"API call failed ({endpoint}): {response.StatusCode}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"API Response ({endpoint}): {json.Length} characters");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"HTTP error ({endpoint}): {httpEx.Message}");
                return null;
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine($"API call timeout ({endpoint})");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"API call failed ({endpoint}): {ex.Message}");
                return null;
            }
        }
    }
}
