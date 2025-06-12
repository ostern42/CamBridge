// src/CamBridge.Config/Services/IApiService.cs
// Version: 0.7.8
// Description: Interface for CamBridge Service API - KISS without DeadLetter!
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CamBridge.Config.Models;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Interface for CamBridge Service API communication
    /// KISS: Removed DeadLetter methods!
    /// </summary>
    public interface IApiService
    {
        /// <summary>
        /// Gets the current service status
        /// </summary>
        Task<ServiceStatusModel?> GetStatusAsync();

        /// <summary>
        /// Gets detailed statistics
        /// </summary>
        Task<DetailedStatisticsModel?> GetStatisticsAsync();

        /// <summary>
        /// Checks if the service is reachable
        /// </summary>
        Task<bool> IsServiceAvailableAsync();
    }
}
