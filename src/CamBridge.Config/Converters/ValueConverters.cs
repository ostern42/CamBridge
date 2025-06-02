// src/CamBridge.Config/Converters/ValueConverters.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace CamBridge.Config.Converters
{
    /// <summary>
    /// Converts boolean values to Visibility
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }

    /// <summary>
    /// Converts boolean values to Visibility (inverted)
    /// </summary>
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility != Visibility.Visible;
            }
            return true;
        }
    }

    /// <summary>
    /// Converts null values to Visibility
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts an enum type to a collection of its values for ComboBox binding
    /// </summary>
    public class EnumToCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Enumerable.Empty<object>();

            Type enumType;

            // Check if value is an enum type itself
            if (value is Type type && type.IsEnum)
            {
                enumType = type;
            }
            else
            {
                // Value is an enum instance
                enumType = value.GetType();
                if (!enumType.IsEnum)
                {
                    return Enumerable.Empty<object>();
                }
            }

            return Enum.GetValues(enumType).Cast<object>().ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts service status to color
    /// </summary>
    public class ServiceStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status switch
                {
                    "Running" => "#FF4CAF50",      // Green
                    "Stopped" => "#FFB71C1C",      // Dark Red
                    "Paused" => "#FFFFA726",       // Orange
                    "StartPending" => "#FF2196F3", // Blue
                    "StopPending" => "#FFFF5722",  // Deep Orange
                    _ => "#FF9E9E9E"               // Gray
                };
            }
            return "#FF9E9E9E";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts TimeSpan to readable string
    /// </summary>
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                if (timeSpan == TimeSpan.Zero)
                    return "Not running";

                if (timeSpan.TotalDays >= 1)
                    return $"{(int)timeSpan.TotalDays} days, {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
                else
                    return $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Formats file size to human readable string
    /// </summary>
    public class FileSizeConverter : IValueConverter
    {
        private static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB", "TB" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long bytes)
            {
                if (bytes == 0)
                    return "0 B";

                int magnitude = (int)Math.Log(bytes, 1024);
                decimal adjustedSize = (decimal)bytes / (1L << (magnitude * 10));

                return $"{adjustedSize:n1} {SizeSuffixes[magnitude]}";
            }
            return "0 B";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter that returns true if the value is greater than zero
    /// </summary>
    public class GreaterThanZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            return value switch
            {
                int intValue => intValue > 0,
                long longValue => longValue > 0,
                double doubleValue => doubleValue > 0,
                float floatValue => floatValue > 0,
                decimal decimalValue => decimalValue > 0,
                _ => false
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter that returns Visibility.Visible if the value is zero, Collapsed otherwise
    /// </summary>
    public class ZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            var isZero = value switch
            {
                int intValue => intValue == 0,
                long longValue => longValue == 0,
                double doubleValue => Math.Abs(doubleValue) < double.Epsilon,
                float floatValue => Math.Abs(floatValue) < float.Epsilon,
                decimal decimalValue => decimalValue == 0,
                _ => false
            };

            return isZero ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
