// src/CamBridge.Config/Services/IApiService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CamBridge.Config.Models;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Interface for CamBridge Service API communication
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
        /// Gets dead letter items
        /// </summary>
        Task<List<DeadLetterItemModel>?> GetDeadLettersAsync();

        /// <summary>
        /// Reprocesses a dead letter item
        /// </summary>
        Task<bool> ReprocessDeadLetterAsync(Guid id);

        /// <summary>
        /// Checks if the service is reachable
        /// </summary>
        Task<bool> IsServiceAvailableAsync();
    }
}
