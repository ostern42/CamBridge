using System.Threading;
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CamBridge.Infrastructure.Tests
{
    public class ProcessingQueueTests
    {
        private readonly Mock<ILogger<ProcessingQueue>> _loggerMock;
        private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
        private readonly Mock<IFileProcessor> _fileProcessorMock;
        private readonly Mock<ILogger<DeadLetterQueue>> _deadLetterLoggerMock;
        private readonly ProcessingOptions _options;
        private readonly DeadLetterQueue _deadLetterQueue;

        public ProcessingQueueTests()
        {
            _loggerMock = new Mock<ILogger<ProcessingQueue>>();
            _scopeFactoryMock = new Mock<IServiceScopeFactory>();
            _fileProcessorMock = new Mock<IFileProcessor>();
            _deadLetterLoggerMock = new Mock<ILogger<DeadLetterQueue>>();
            _options = new ProcessingOptions
            {
                MaxConcurrentProcessing = 2,
                RetryOnFailure = true,
                MaxRetryAttempts = 3,
                RetryDelaySeconds = 1
            };

            // Create DeadLetterQueue
            _deadLetterQueue = new DeadLetterQueue(_deadLetterLoggerMock.Object);

            // Setup scope factory to return file processor
            var scopeMock = new Mock<IServiceScope>();
            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IFileProcessor)))
                .Returns(_fileProcessorMock.Object);

            scopeMock
                .Setup(x => x.ServiceProvider)
                .Returns(serviceProviderMock.Object);

            _scopeFactoryMock
                .Setup(x => x.CreateScope())
                .Returns(scopeMock.Object);
        }

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitialize()
        {
            // Arrange & Act
            var queue = new ProcessingQueue(
                _loggerMock.Object,
                _scopeFactoryMock.Object,
                Options.Create(_options),
                _deadLetterQueue);

            // Assert
            Assert.NotNull(queue);
            Assert.Equal(0, queue.QueueLength);
            Assert.Equal(0, queue.ActiveProcessing);
        }

        [Fact]
        public void TryEnqueue_WithValidFile_ShouldReturnTrue()
        {
            // Arrange
            var queue = new ProcessingQueue(
                _loggerMock.Object,
                _scopeFactoryMock.Object,
                Options.Create(_options),
                _deadLetterQueue);

            const string filePath = @"C:\test\image.jpg";
            _fileProcessorMock
                .Setup(x => x.ShouldProcessFile(filePath))
                .Returns(true);

            // Act
            var result = queue.TryEnqueue(filePath);

            // Assert
            Assert.True(result);
            Assert.Equal(1, queue.QueueLength);
            _scopeFactoryMock.Verify(x => x.CreateScope(), Times.Once);
        }

        [Fact]
        public void TryEnqueue_WithInvalidFile_ShouldReturnFalse()
        {
            // Arrange
            var queue = new ProcessingQueue(
                _loggerMock.Object,
                _scopeFactoryMock.Object,
                Options.Create(_options),
                _deadLetterQueue);

            const string filePath = @"C:\test\image.jpg";
            _fileProcessorMock
                .Setup(x => x.ShouldProcessFile(filePath))
                .Returns(false);

            // Act
            var result = queue.TryEnqueue(filePath);

            // Assert
            Assert.False(result);
            Assert.Equal(0, queue.QueueLength);
        }

        [Fact]
        public void TryEnqueue_WithDuplicateFile_ShouldReturnFalse()
        {
            // Arrange
            var queue = new ProcessingQueue(
                _loggerMock.Object,
                _scopeFactoryMock.Object,
                Options.Create(_options),
                _deadLetterQueue);

            const string filePath = @"C:\test\image.jpg";
            _fileProcessorMock
                .Setup(x => x.ShouldProcessFile(filePath))
                .Returns(true);

            // Act
            queue.TryEnqueue(filePath);
            var result = queue.TryEnqueue(filePath);

            // Assert
            Assert.False(result);
            Assert.Equal(1, queue.QueueLength);
        }

        [Fact]
        public async Task ProcessQueueAsync_WithSuccessfulProcessing_ShouldUpdateStatistics()
        {
            // Arrange
            var queue = new ProcessingQueue(
                _loggerMock.Object,
                _scopeFactoryMock.Object,
                Options.Create(_options),
                _deadLetterQueue);

            const string filePath = @"C:\test\image.jpg";
            var processingResult = ProcessingResult.CreateSuccess(
                filePath,
                @"C:\output\image.dcm",
                TimeSpan.FromSeconds(1));

            _fileProcessorMock
                .Setup(x => x.ShouldProcessFile(filePath))
                .Returns(true);
            _fileProcessorMock
                .Setup(x => x.ProcessFileAsync(filePath))
                .ReturnsAsync(processingResult);

            queue.TryEnqueue(filePath);

            // Act
            var cts = new CancellationTokenSource();
            var processTask = queue.ProcessQueueAsync(cts.Token);

            // Wait for processing to complete
            await Task.Delay(100);
            cts.Cancel();

            try
            {
                await processTask;
            }
            catch (OperationCanceledException)
            {
                // Expected
            }

            // Assert
            var stats = queue.GetStatistics();
            Assert.Equal(1, stats.TotalProcessed);
            Assert.Equal(1, stats.TotalSuccessful);
            Assert.Equal(0, stats.TotalFailed);
            Assert.Equal(100, stats.SuccessRate);
        }

        [Fact]
        public async Task ProcessQueueAsync_WithFailedProcessing_ShouldRetry()
        {
            // Arrange
            var options = new ProcessingOptions
            {
                MaxConcurrentProcessing = 1,
                RetryOnFailure = true,
                MaxRetryAttempts = 2,
                RetryDelaySeconds = 0 // No delay for testing
            };

            var queue = new ProcessingQueue(
                _loggerMock.Object,
                _scopeFactoryMock.Object,
                Options.Create(options),
                _deadLetterQueue);

            const string filePath = @"C:\test\image.jpg";
            var failureResult = ProcessingResult.CreateFailure(
                filePath,
                "Test error",
                TimeSpan.FromSeconds(1));

            _fileProcessorMock
                .Setup(x => x.ShouldProcessFile(filePath))
                .Returns(true);
            _fileProcessorMock
                .Setup(x => x.ProcessFileAsync(filePath))
                .ReturnsAsync(failureResult);

            queue.TryEnqueue(filePath);

            // Act
            var cts = new CancellationTokenSource();
            var processTask = queue.ProcessQueueAsync(cts.Token);

            // Wait for retries
            await Task.Delay(200);
            cts.Cancel();

            try
            {
                await processTask;
            }
            catch (OperationCanceledException)
            {
                // Expected
            }

            // Assert
            _fileProcessorMock.Verify(
                x => x.ProcessFileAsync(filePath),
                Times.Exactly(2)); // Initial + 1 retry

            var stats = queue.GetStatistics();
            Assert.Equal(2, stats.TotalProcessed);
            Assert.Equal(0, stats.TotalSuccessful);
            Assert.Equal(2, stats.TotalFailed);
        }

        [Fact]
        public void GetStatistics_ShouldReturnCorrectValues()
        {
            // Arrange
            var queue = new ProcessingQueue(
                _loggerMock.Object,
                _scopeFactoryMock.Object,
                Options.Create(_options),
                _deadLetterQueue);

            // Act
            var stats = queue.GetStatistics();

            // Assert
            Assert.NotNull(stats);
            Assert.Equal(0, stats.QueueLength);
            Assert.Equal(0, stats.ActiveProcessing);
            Assert.Equal(0, stats.TotalProcessed);
            Assert.Equal(0, stats.SuccessRate);
            Assert.True(stats.UpTime > TimeSpan.Zero);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void TryEnqueue_WithInvalidPath_ShouldReturnFalse(string? filePath)
        {
            // Arrange
            var queue = new ProcessingQueue(
                _loggerMock.Object,
                _scopeFactoryMock.Object,
                Options.Create(_options),
                _deadLetterQueue);

            // Act
            var result = queue.TryEnqueue(filePath!);

            // Assert
            Assert.False(result);
            Assert.Equal(0, queue.QueueLength);
        }
    }
}
