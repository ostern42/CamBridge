using System;

namespace CamBridge.Core.ValueObjects
{
    /// <summary>
    /// Represents a patient identifier as a value object
    /// </summary>
    public class PatientId : IEquatable<PatientId>
    {
        /// <summary>
        /// Gets the patient identifier value
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates a new PatientId instance
        /// </summary>
        /// <param name="value">The patient identifier value</param>
        /// <exception cref="ArgumentNullException">When value is null</exception>
        /// <exception cref="ArgumentException">When value is empty or whitespace</exception>
        public PatientId(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Patient ID cannot be empty or whitespace", nameof(value));

            Value = value.Trim();
        }

        /// <summary>
        /// Creates a new random PatientId for testing purposes
        /// </summary>
        public static PatientId NewId() => new PatientId(Guid.NewGuid().ToString("N"));

        public bool Equals(PatientId? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PatientId)obj);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(PatientId? left, PatientId? right) => Equals(left, right);

        public static bool operator !=(PatientId? left, PatientId? right) => !Equals(left, right);

        public override string ToString() => Value;

        /// <summary>
        /// Implicit conversion from string
        /// </summary>
        public static implicit operator string(PatientId patientId) => patientId.Value;

        /// <summary>
        /// Explicit conversion to PatientId
        /// </summary>
        public static explicit operator PatientId(string value) => new PatientId(value);
    }
}
