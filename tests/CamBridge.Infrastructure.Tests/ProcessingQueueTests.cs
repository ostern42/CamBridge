using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CamBridge.Infrastructure.Tests
{
    public class ProcessingQueueTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly Mock<ILogger<ProcessingQueue>> _loggerMock;
        private readonly Mock<IFileProcessor> _fileProcessorMock;
        private readonly ProcessingOptions _processingOptions;
        private readonly ProcessingQueue _processingQueue;

        public ProcessingQueueTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), $"CamBridgeQueueTest_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);

            _loggerMock = new Mock<ILogger<ProcessingQueue>>();
            _fileProcessorMock = new Mock<IFileProcessor>();

            _processingOptions = new ProcessingOptions
            {
                MaxConcurrentProcessing = 2,
                RetryOnFailure = true,
                MaxRetryAttempts = 3,
                RetryDelaySeconds = 1
            };

            var optionsWrapper = Options.Create(_processingOptions);

            _processingQueue = new ProcessingQueue(
                _loggerMock.Object,
                _fileProcessorMock.Object,
                optionsWrapper);
        }

        [Fact]
        public void TryEnqueue_ValidFile_ReturnsTrue()
        {
            // Arrange
            var testFile = CreateTestFile();
            _fileProcessorMock.Setup(x => x.ShouldProcessFile(testFile))
                .Returns(true);

            // Act
            var result = _processingQueue.TryEnqueue(testFile);

            // Assert
            Assert.True(result);
            Assert.Equal(1, _processingQueue.QueueLength);
        }

        [Fact]
        public void TryEnqueue_DuplicateFile_ReturnsFalse()
        {
            // Arrange
            var testFile = CreateTestFile();
            _fileProcessorMock.Setup(x => x.ShouldProcessFile(testFile))
                .Returns(true);

            // Act
            _processingQueue.TryEnqueue(testFile);
            var result = _processingQueue.TryEnqueue(testFile);

            // Assert
            Assert.False(result);
            Assert.Equal(1, _processingQueue.QueueLength);
        }

        [Fact]
        public void TryEnqueue_InvalidFile_ReturnsFalse()
        {
            // Arrange
            var testFile = "invalid.jpg";
            _fileProcessorMock.Setup(x => x.ShouldProcessFile(testFile))
                .Returns(false);

            // Act
            var result = _processingQueue.TryEnqueue(testFile);

            // Assert
            Assert.False(result);
            Assert.Equal(0, _processingQueue.QueueLength);
        }

        [Fact]
        public async Task ProcessQueueAsync_ProcessesEnqueuedFiles()
        {
            // Arrange
            var testFile = CreateTestFile();
            _fileProcessorMock.Setup(x => x.ShouldProcessFile(testFile))
                .Returns(true);
            _fileProcessorMock.Setup(x => x.ProcessFileAsync(testFile))
                .ReturnsAsync(ProcessingResult.CreateSuccess(testFile, "output.dcm", TimeSpan.FromSeconds(1)));

            _processingQueue.TryEnqueue(testFile);

            // Act
            var cts = new CancellationTokenSource();
            var processTask = _processingQueue.ProcessQueueAsync(cts.Token);

            // Wait for processing to start
            await Task.Delay(500);

            // Stop processing
            cts.Cancel();
            await processTask;

            // Assert
            _fileProcessorMock.Verify(x => x.ProcessFileAsync(testFile), Times.Once);
            Assert.Equal(1, _processingQueue.TotalProcessed);
            Assert.Equal(1, _processingQueue.TotalSuccessful);
            Assert.Equal(0, _processingQueue.TotalFailed);
        }

        [Fact]
        public async Task ProcessQueueAsync_HandlesFailureWithRetry()
        {
            // Arrange
            var testFile = CreateTestFile();
            var attemptCount = 0;

            _fileProcessorMock.Setup(x => x.ShouldProcessFile(testFile))
                .Returns(true);

            _fileProcessorMock.Setup(x => x.ProcessFileAsync(testFile))
                .ReturnsAsync(() =>
                {
                    attemptCount++;
                    if (attemptCount < 2)
                    {
                        return ProcessingResult.CreateFailure(testFile, "Test error", TimeSpan.FromSeconds(1));
                    }
                    return ProcessingResult.CreateSuccess(testFile, "output.dcm", TimeSpan.FromSeconds(1));
                });

            _processingQueue.TryEnqueue(testFile);

            // Act
            var cts = new CancellationTokenSource();
            var processTask = _processingQueue.ProcessQueueAsync(cts.Token);

            // Wait for retry
            await Task.Delay(2500);

            // Stop processing
            cts.Cancel();
            await processTask;

            // Assert
            Assert.Equal(2, attemptCount); // Initial attempt + 1 retry
            Assert.Equal(1, _processingQueue.TotalProcessed);
            Assert.Equal(1, _processingQueue.TotalSuccessful);
        }

        [Fact]
        public async Task ProcessQueueAsync_ConcurrentProcessing()
        {
            // Arrange
            var file1 = CreateTestFile("file1.jpg");
            var file2 = CreateTestFile("file2.jpg");
            var processingCount = 0;
            var maxConcurrent = 0;
            var lockObj = new object();

            _fileProcessorMock.Setup(x => x.ShouldProcessFile(It.IsAny<string>()))
                .Returns(true);

            _fileProcessorMock.Setup(x => x.ProcessFileAsync(It.IsAny<string>()))
                .ReturnsAsync((string file) =>
                {
                    lock (lockObj)
                    {
                        processingCount++;
                        if (processingCount > maxConcurrent)
                            maxConcurrent = processingCount;
                    }

                    Thread.Sleep(500); // Simulate processing

                    lock (lockObj)
                    {
                        processingCount--;
                    }

                    return ProcessingResult.CreateSuccess(file, "output.dcm", TimeSpan.FromMilliseconds(500));
                });

            _processingQueue.TryEnqueue(file1);
            _processingQueue.TryEnqueue(file2);

            // Act
            var cts = new CancellationTokenSource();
            var processTask = _processingQueue.ProcessQueueAsync(cts.Token);

            // Wait for processing
            await Task.Delay(1500);

            // Stop processing
            cts.Cancel();
            await processTask;

            // Assert
            Assert.Equal(2, maxConcurrent); // Should process 2 files concurrently
            Assert.Equal(2, _processingQueue.TotalProcessed);
            Assert.Equal(2, _processingQueue.TotalSuccessful);
        }

        [Fact]
        public void GetStatistics_ReturnsCorrectStats()
        {
            // Arrange
            var testFile = CreateTestFile();
            _fileProcessorMock.Setup(x => x.ShouldProcessFile(testFile))
                .Returns(true);

            _processingQueue.TryEnqueue(testFile);

            // Act
            var stats = _processingQueue.GetStatistics();

            // Assert
            Assert.Equal(1, stats.QueueLength);
            Assert.Equal(0, stats.ActiveProcessing);
            Assert.Equal(0, stats.TotalProcessed);
            Assert.Equal(0, stats.TotalSuccessful);
            Assert.Equal(0, stats.TotalFailed);
            Assert.Equal(0, stats.SuccessRate);
        }

        [Fact]
        public void GetActiveItems_ReturnsEmptyWhenNoProcessing()
        {
            // Act
            var activeItems = _processingQueue.GetActiveItems();

            // Assert
            Assert.Empty(activeItems);
        }

        private string CreateTestFile(string fileName = null)
        {
            fileName ??= $"test_{Guid.NewGuid()}.jpg";
            var filePath = Path.Combine(_testDirectory, fileName);
            File.WriteAllBytes(filePath, new byte[1024]);
            return filePath;
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_testDirectory))
                {
                    Directory.Delete(_testDirectory, true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
