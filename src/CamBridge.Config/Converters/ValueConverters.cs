// src/CamBridge.Config/Converters/ValueConverters.cs
// Version: 0.7.7
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CamBridge.Config.Converters
{
    /// <summary>
    /// Converts boolean values to Visibility
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = value is bool b && b;
            bool invert = parameter as string == "Inverse";

            if (invert)
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            else
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Inverts boolean to visibility conversion
    /// </summary>
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = value is bool b && b;
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts numeric values greater than zero to true
    /// </summary>
    public class GreaterThanZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;

            try
            {
                double numValue = System.Convert.ToDouble(value);
                return numValue > 0;
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts null values to Visibility
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNull = value == null;
            bool invert = parameter as string == "Inverse";

            if (invert)
                return isNull ? Visibility.Visible : Visibility.Collapsed;
            else
                return isNull ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts zero values to Visibility
    /// </summary>
    public class ZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isZero = false;

            if (value != null)
            {
                try
                {
                    double numValue = System.Convert.ToDouble(value);
                    isZero = Math.Abs(numValue) < 0.0001; // Floating point comparison
                }
                catch
                {
                    // If conversion fails, treat as non-zero
                }
            }

            bool invert = parameter as string == "Inverse";

            if (invert)
                return isZero ? Visibility.Collapsed : Visibility.Visible;
            else
                return isZero ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts error count to color brush
    /// </summary>
    public class ErrorCountToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int errorCount = 0;

            if (value != null)
            {
                try
                {
                    errorCount = System.Convert.ToInt32(value);
                }
                catch
                {
                    // Default to 0 if conversion fails
                }
            }

            // Return red color if errors exist, otherwise default
            if (errorCount > 0)
            {
                return new SolidColorBrush(Color.FromRgb(255, 107, 107)); // Light red
            }

            return DependencyProperty.UnsetValue; // Use default style
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts boolean values to inverse boolean
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is bool b && b);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is bool b && b);
        }
    }

    /// <summary>
    /// Converts empty string to visibility
    /// </summary>
    public class EmptyStringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? str = value as string;
            bool isEmpty = string.IsNullOrWhiteSpace(str);
            bool invert = parameter as string == "Inverse";

            if (invert)
                return isEmpty ? Visibility.Visible : Visibility.Collapsed;
            else
                return isEmpty ? Visibility.Collapsed : Visibility.Visible;
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
            string status = value as string ?? "";

            return status.ToLower() switch
            {
                "running" => new SolidColorBrush(Colors.Green),
                "stopped" => new SolidColorBrush(Colors.Red),
                "paused" => new SolidColorBrush(Colors.Orange),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts seconds to milliseconds
    /// </summary>
    public class SecondsToMillisecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double seconds)
                return seconds * 1000;
            if (value is int intSeconds)
                return intSeconds * 1000;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double milliseconds)
                return milliseconds / 1000;
            if (value is int intMilliseconds)
                return intMilliseconds / 1000;
            return 0;
        }
    }

    /// <summary>
    /// Converts enum value to boolean based on parameter
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string enumValue = value.ToString();
            string targetValue = parameter.ToString();

            return enumValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked)
            {
                if (parameter != null && targetType.IsEnum)
                {
                    return Enum.Parse(targetType, parameter.ToString());
                }
            }

            return Binding.DoNothing;
        }
    }

    /// <summary>
    /// Converter for file selection dialogs
    /// </summary>
    public class FileSelectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Simply pass through the value
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Simply pass through the value
            return value;
        }
    }

    /// <summary>
    /// Multi-value boolean OR converter
    /// </summary>
    public class MultiBooleanOrConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0)
                return false;

            foreach (var value in values)
            {
                if (value is bool b && b)
                    return true;
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts enum types to collection for ComboBox binding
    /// </summary>
    public class EnumToCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.GetValues(parameter as Type ?? value?.GetType() ?? typeof(object));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts file size to human-readable format
    /// </summary>
    public class FileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "0 B";

            long bytes = System.Convert.ToInt64(value);
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
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
                if (timeSpan.TotalDays >= 1)
                    return $"{(int)timeSpan.TotalDays}d {timeSpan.Hours}h {timeSpan.Minutes}m";
                else if (timeSpan.TotalHours >= 1)
                    return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m";
                else
                    return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
            }
            return "0s";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Multi-value boolean AND converter
    /// </summary>
    public class MultiBooleanAndConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0)
                return false;

            foreach (var value in values)
            {
                if (!(value is bool b) || !b)
                    return false;
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Null and boolean AND converter
    /// </summary>
    public class NullBooleanAndConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return false;

            // First value should not be null, second should be true
            return values[0] != null && values[1] is bool b && b;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
