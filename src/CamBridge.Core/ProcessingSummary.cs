using System;
using System.Collections.Generic;

namespace CamBridge.Core
{
    /// <summary>
    /// Summary of daily processing activities
    /// </summary>
    public class ProcessingSummary
    {
        public DateTime Date { get; set; }
        public int TotalProcessed { get; set; }
        public int Successful { get; set; }
        public int Failed { get; set; }
        public double ProcessingTimeSeconds { get; set; }
        public Dictionary<string, int> TopErrors { get; set; } = new();
        public int DeadLetterCount { get; set; }
        public TimeSpan Uptime { get; set; }

        // Berechnete Properties
        public double SuccessRate => TotalProcessed > 0
            ? (double)Successful / TotalProcessed * 100
            : 0;

        public double AverageProcessingTime => TotalProcessed > 0
            ? ProcessingTimeSeconds / TotalProcessed
            : 0;
    }
}
