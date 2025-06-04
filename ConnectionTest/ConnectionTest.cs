// File: ConnectionTest.cs
// Quick test to verify HTTP API connection

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

Console.WriteLine("CamBridge API Connection Test");
Console.WriteLine("=============================\n");

var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5050/");
client.Timeout = TimeSpan.FromSeconds(5);

try
{
    // Test 1: Health Check
    Console.Write("Testing /health endpoint... ");
    var healthResponse = await client.GetAsync("health");
    Console.WriteLine($"{healthResponse.StatusCode} ✓");

    // Test 2: API Status
    Console.Write("Testing /api/status endpoint... ");
    var statusResponse = await client.GetAsync("api/status");
    if (statusResponse.IsSuccessStatusCode)
    {
        Console.WriteLine($"{statusResponse.StatusCode} ✓");
        
        var status = await statusResponse.Content.ReadFromJsonAsync<ServiceStatusDto>();
        Console.WriteLine("\n📊 Service Status:");
        Console.WriteLine($"  Status: {status.ServiceStatus}");
        Console.WriteLine($"  Version: {status.Version}");
        Console.WriteLine($"  Uptime: {status.Uptime}");
        Console.WriteLine($"  Queue Length: {status.QueueLength}");
        Console.WriteLine($"  Active Processing: {status.ActiveProcessing}");
        Console.WriteLine($"  Total Successful: {status.TotalSuccessful}");
        Console.WriteLine($"  Total Failed: {status.TotalFailed}");
        Console.WriteLine($"  Success Rate: {status.SuccessRate:F1}%");
        Console.WriteLine($"  Dead Letters: {status.DeadLetterCount}");
        
        if (status.Configuration != null)
        {
            Console.WriteLine($"\n⚙️ Configuration:");
            Console.WriteLine($"  Watch Folders: {status.Configuration.WatchFolders}");
            Console.WriteLine($"  Output Folder: {status.Configuration.OutputFolder}");
        }
    }
    else
    {
        Console.WriteLine($"{statusResponse.StatusCode} ❌");
    }

    Console.WriteLine("\n✅ API Connection successful!");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"❌ Connection failed: {ex.Message}");
    Console.WriteLine("\nMake sure the CamBridge Service is running on port 5050!");
}
catch (TaskCanceledException)
{
    Console.WriteLine("❌ Connection timeout!");
    Console.WriteLine("\nThe service didn't respond within 5 seconds.");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
client.Dispose();

// Simple DTOs for deserialization
public class ServiceStatusDto
{
    public string ServiceStatus { get; set; }
    public string Version { get; set; }
    public TimeSpan Uptime { get; set; }
    public int QueueLength { get; set; }
    public int ActiveProcessing { get; set; }
    public int TotalSuccessful { get; set; }
    public int TotalFailed { get; set; }
    public double SuccessRate { get; set; }
    public int DeadLetterCount { get; set; }
    public ConfigInfo Configuration { get; set; }
}

public class ConfigInfo
{
    public int WatchFolders { get; set; }
    public string OutputFolder { get; set; }
}