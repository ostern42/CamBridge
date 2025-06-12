// src/CamBridge.QRBridge/Program.cs
// Version: 0.7.8
// © 2025 Claude's Improbably Reliable Software Solutions

using System.Text;
using System.Windows.Forms;
using CamBridge.QRBridge.Constants;
using CamBridge.QRBridge.Forms;
using CamBridge.QRBridge.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CamBridge.QRBridge;

/// <summary>
/// Entry point for CamBridge QRBridge application
/// </summary>
internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static int Main(string[] args)
    {
        // Set UTF-8 encoding for console (only if console available)
        try
        {
            if (Environment.UserInteractive && !Console.IsOutputRedirected)
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.InputEncoding = Encoding.UTF8;
            }
        }
        catch
        {
            // Ignore - WinForms apps don't have console
        }

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "qrbridge-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();

        try
        {
            Log.Information("CamBridge QRBridge v{Version} starting", QRBridgeConstants.Application.Version);
            Log.Debug("Arguments: {Args}", string.Join(" ", args));

            // Enable visual styles for Windows Forms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            // Build host
            var host = CreateHostBuilder(args).Build();

            // Get services
            var argumentParser = host.Services.GetRequiredService<ArgumentParser>();
            var qrCodeService = host.Services.GetRequiredService<IQRCodeService>();
            var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();

            // Parse arguments
            var (success, request, error) = argumentParser.Parse(args);

            if (!success || request == null)
            {
                // Handle help text or errors
                if (error?.Contains("Usage:") ?? false)
                {
                    // It's help text
                    MessageBox.Show(error, "CamBridge QRBridge Help",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // It's an error
                    MessageBox.Show(error, "CamBridge QRBridge Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return QRBridgeConstants.ExitCodes.InvalidArguments;
            }

            // Create and show form
            var formLogger = loggerFactory.CreateLogger<QRDisplayForm>();
            using var form = new QRDisplayForm(formLogger, qrCodeService, request);

            Application.Run(form);

            Log.Information("QRBridge completed successfully");
            return QRBridgeConstants.ExitCodes.Success;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unhandled exception in QRBridge");

            // Show error in MessageBox for WinForms
            MessageBox.Show($"Fatal Error: {ex.Message}\n\nDetails:\n{ex}",
                "CamBridge QRBridge - Fatal Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return QRBridgeConstants.ExitCodes.UnhandledException;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                // Register services
                services.AddSingleton<IQRCodeService, QRCodeService>();
                services.AddSingleton<ArgumentParser>();

                // Register logging
                services.AddLogging(configure =>
                {
                    configure.ClearProviders();
                    configure.AddSerilog();
                });
            });
}
