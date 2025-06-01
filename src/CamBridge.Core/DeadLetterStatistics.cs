using System;
using System.Collections.Generic;

namespace CamBridge.Core
{
    /// <summary>
    /// Statistics about the dead letter queue
    /// </summary>
    public class DeadLetterStatistics
    {
        public int TotalItems { get; set; }
        public int ItemsLastHour { get; set; }
        public int ItemsLast24Hours { get; set; }
        public DateTime OldestItem { get; set; }
        public Dictionary<string, int> TopErrors { get; set; } = new();
    }
}
