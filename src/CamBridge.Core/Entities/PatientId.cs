using System;

namespace CamBridge.Core.ValueObjects
{
    /// <summary>
    /// Value object representing a patient ID
    /// </summary>
    public class PatientId : IEquatable<PatientId>
    {
        public string Value { get; }

        public PatientId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Patient ID cannot be empty", nameof(value));

            Value = value.Trim();
        }

        public override string ToString() => Value;

        public override bool Equals(object? obj) => Equals(obj as PatientId);

        public bool Equals(PatientId? other)
        {
            if (other is null) return false;
            return Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(PatientId? left, PatientId? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(PatientId? left, PatientId? right) => !(left == right);

        public static implicit operator string(PatientId id) => id.Value;
    }
}
