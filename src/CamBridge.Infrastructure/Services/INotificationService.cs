// src/CamBridge.Infrastructure/Services/INotificationService.cs
// Version: 0.7.9
// Description: Minimal notification interface - KISS approach!
// Copyright:  2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Threading.Tasks;
using CamBridge.Core;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Minimal notification service interface - KISS!
    /// Most notifications removed in favor of simple logging
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Send daily summary if configured
        /// </summary>
        Task SendDailySummaryAsync(ProcessingSummary summary);

        /// <summary>
        /// Send critical error notification
        /// </summary>
        Task NotifyErrorAsync(string message, Exception? exception = null);
    }
}
