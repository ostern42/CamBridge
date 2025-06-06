// src/CamBridge.Infrastructure/Services/INotificationService.cs
using System;
using System.Threading.Tasks;
using CamBridge.Core;

namespace CamBridge.Infrastructure.Services
{
    public interface INotificationService
    {
        Task NotifyInfoAsync(string subject, string message);
        Task NotifyWarningAsync(string subject, string message);
        Task NotifyErrorAsync(string subject, string message, Exception? exception = null);
        Task NotifyCriticalErrorAsync(string subject, string message, Exception? exception = null);
        Task NotifyDeadLetterThresholdAsync(int count, DeadLetterStatistics statistics);
        Task SendDailySummaryAsync(ProcessingSummary summary);
    }
}