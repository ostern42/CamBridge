// src/CamBridge.Config/Converters/ValueConverters.cs
// Version: 0.8.11 - SAFE VERSION (nur Unicode-Fixes, keine riskanten neuen Features)
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using CamBridge.Config.ViewModels;
using CamBridge.Config.Models;
using CamBridge.Core;

namespace CamBridge.Config.Converters
{
    /// <summary>
    /// SAFE: Fixed expand/collapse icons (Unicode → proper symbols)
    /// </summary>
    public class ExpandIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isExpanded)
            {
                return isExpanded ? "▼" : "▶";  // FIXED: proper Unicode symbols
            }
            return "▶";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts integer to Visibility based on comparison with parameter
    /// </summary>
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue = 0;
            int compareValue = 0;

            if (value != null)
            {
                try { intValue = System.Convert.ToInt32(value); }
                catch { /* Default to 0 */ }
            }

            if (parameter != null)
            {
                try { compareValue = System.Convert.ToInt32(parameter); }
                catch { /* Default to 0 */ }
            }

            return intValue == compareValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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
    /// SAFE: Converts numeric values greater than zero to Visibility
    /// </summary>
    public class GreaterThanZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;

            try
            {
                double numValue = System.Convert.ToDouble(value);
                return numValue > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch
            {
                return Visibility.Collapsed;
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
                    isZero = Math.Abs(numValue) < 0.0001;
                }
                catch { /* If conversion fails, treat as non-zero */ }
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
                try { errorCount = System.Convert.ToInt32(value); }
                catch { /* Default to 0 */ }
            }

            if (errorCount > 0)
            {
                return new SolidColorBrush(Color.FromRgb(255, 107, 107));
            }

            return DependencyProperty.UnsetValue;
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
    /// SAFE: Service status to color converter (no changes, was working)
    /// </summary>
    public class ServiceStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string ?? "";

            return status.ToLower() switch
            {
                "running" => new SolidColorBrush(Color.FromRgb(76, 175, 80)),      // Green
                "online" => new SolidColorBrush(Color.FromRgb(76, 175, 80)),       // Green
                "stopped" => new SolidColorBrush(Color.FromRgb(255, 193, 7)),      // Yellow
                "offline" => new SolidColorBrush(Color.FromRgb(255, 193, 7)),      // Yellow
                "paused" => new SolidColorBrush(Color.FromRgb(255, 152, 0)),       // Orange
                "startpending" => new SolidColorBrush(Color.FromRgb(255, 152, 0)), // Orange
                "stoppending" => new SolidColorBrush(Color.FromRgb(255, 152, 0)),  // Orange
                "continuepending" => new SolidColorBrush(Color.FromRgb(255, 152, 0)), // Orange
                "pausepending" => new SolidColorBrush(Color.FromRgb(255, 152, 0)),    // Orange
                "error" => new SolidColorBrush(Color.FromRgb(244, 67, 54)),        // Red
                "notinstalled" => new SolidColorBrush(Color.FromRgb(244, 67, 54)), // Red
                _ => new SolidColorBrush(Color.FromRgb(158, 158, 158))             // Gray
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
            if (value is double seconds) return seconds * 1000;
            if (value is int intSeconds) return intSeconds * 1000;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double milliseconds) return milliseconds / 1000;
            if (value is int intMilliseconds) return intMilliseconds / 1000;
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
            if (value == null || parameter == null) return false;

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
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
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
            if (values == null || values.Length == 0) return false;

            foreach (var value in values)
            {
                if (value is bool b && b) return true;
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
            if (values == null || values.Length == 0) return false;

            foreach (var value in values)
            {
                if (!(value is bool b) || !b) return false;
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
            if (values == null || values.Length < 2) return false;

            return values[0] != null && values[1] is bool b && b;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts boolean to color (Green for true, Red for false)
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isTrue)
            {
                return new SolidColorBrush(isTrue ? Colors.Green : Colors.Red);
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// SAFE: Transform enum converter (fixed Unicode but kept simple)
    /// </summary>
    public class TransformToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ValueTransform transform)
            {
                return transform switch
                {
                    ValueTransform.None => "→",
                    ValueTransform.DateToDicom => "Date→",
                    ValueTransform.TimeToDicom => "Time→",
                    ValueTransform.DateTimeToDicom => "DateTime→",
                    ValueTransform.MapGender => "Gender→",
                    ValueTransform.RemovePrefix => "Trim→",
                    ValueTransform.ExtractDate => "Extract→",
                    ValueTransform.ExtractTime => "Extract→",
                    ValueTransform.ToUpperCase => "UPPER→",
                    ValueTransform.ToLowerCase => "lower→",
                    ValueTransform.Trim => "Trim→",
                    _ => "→"
                };
            }
            return "→";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts Transform enum to a descriptive text
    /// </summary>
    public class TransformToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ValueTransform transform)
            {
                return transform switch
                {
                    ValueTransform.None => "Direct mapping",
                    ValueTransform.DateToDicom => "Convert date to DICOM format (YYYYMMDD)",
                    ValueTransform.TimeToDicom => "Convert time to DICOM format (HHMMSS)",
                    ValueTransform.DateTimeToDicom => "Convert datetime to DICOM format",
                    ValueTransform.MapGender => "Map gender values (M/F/O)",
                    ValueTransform.RemovePrefix => "Remove prefix from value",
                    ValueTransform.ExtractDate => "Extract date from datetime",
                    ValueTransform.ExtractTime => "Extract time from datetime",
                    ValueTransform.ToUpperCase => "Convert to uppercase",
                    ValueTransform.ToLowerCase => "Convert to lowercase",
                    ValueTransform.Trim => "Remove leading/trailing spaces",
                    _ => "Unknown transformation"
                };
            }
            return "No transformation";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Combines stage entries and ungrouped entries into a flat list for compact tree view
    /// </summary>
    public class CombineStagesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2) return null;

            var stages = values[0] as ObservableCollection<StageGroup>;
            var ungrouped = values[1] as ObservableCollection<LogEntry>;

            var combined = new List<LogEntry>();

            if (stages != null)
            {
                foreach (var stage in stages.OrderBy(s => s.StartTime))
                {
                    combined.AddRange(stage.Entries);
                }
            }

            if (ungrouped != null)
            {
                combined.AddRange(ungrouped);
            }

            return combined.OrderBy(e => e.Timestamp).ToList();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// MISSING: Status to color converter for TreeView
    /// </summary>
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProcessingStatus status)
            {
                return status switch
                {
                    ProcessingStatus.Completed => "#4CAF50",  // Green
                    ProcessingStatus.Failed => "#F44336",     // Red
                    ProcessingStatus.InProgress => "#2196F3", // Blue
                    _ => "#9E9E9E"                          // Gray
                };
            }
            return "#9E9E9E";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts color string to SolidColorBrush with opacity
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorString)
            {
                try
                {
                    var color = (Color)ColorConverter.ConvertFromString(colorString);
                    color.A = 30; // Very light background
                    return new SolidColorBrush(color);
                }
                catch
                {
                    return Brushes.Transparent;
                }
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
