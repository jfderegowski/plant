using System;
using UnityEngine;
using static System.Text.RegularExpressions.Regex;

namespace fefek5.Variables.SerializableGuidVariable.Runtime
{
    /// <summary>
    /// Represents a globally unique identifier (GUID) that is serializable with Unity and usable in game scripts.
    /// </summary>
    [Serializable]
    public struct SerializableGuid : IEquatable<SerializableGuid>, IEquatable<Guid>, IEquatable<string>
    {
        #region Fields

        [SerializeField, HideInInspector] public uint Part1;
        [SerializeField, HideInInspector] public uint Part2;
        [SerializeField, HideInInspector] public uint Part3;
        [SerializeField, HideInInspector] public uint Part4;

        #endregion

        #region Properties

        public static SerializableGuid Empty => new(0, 0, 0, 0);
        
        public bool IsEmpty => this == Empty;

        #endregion

        #region Constructors

        public SerializableGuid(uint val1, uint val2, uint val3, uint val4)
        {
            Part1 = val1;
            Part2 = val2;
            Part3 = val3;
            Part4 = val4;
        }

        public SerializableGuid(Guid guid)
        {
            var bytes = guid.ToByteArray();

            Part1 = BitConverter.ToUInt32(bytes, 0);
            Part2 = BitConverter.ToUInt32(bytes, 4);
            Part3 = BitConverter.ToUInt32(bytes, 8);
            Part4 = BitConverter.ToUInt32(bytes, 12);
        }

        public SerializableGuid(string hexString)
        {
            if (!IsHexString(hexString))
                throw new ArgumentException("Invalid hex string.", nameof(hexString));
            
            Part1 = Convert.ToUInt32(hexString[..8], 16);
            Part2 = Convert.ToUInt32(hexString.Substring(8, 8), 16);
            Part3 = Convert.ToUInt32(hexString.Substring(16, 8), 16);
            Part4 = Convert.ToUInt32(hexString.Substring(24, 8), 16);
        }

        #endregion

        #region Methods

        public static SerializableGuid NewGuid() => new(Guid.NewGuid());

        public static SerializableGuid FromHexString(string hexString) => new(hexString);

        public static bool IsHexString(string hexString) =>
            hexString is { Length: 32 } && IsMatch(hexString, @"\A\b[0-9a-fA-F]+\b\Z");

        public Guid ToGuid()
        {
            var bytes = new byte[16];
            var span = new Span<byte>(bytes);
            
            BitConverter.TryWriteBytes(span.Slice(0, 4), Part1);
            BitConverter.TryWriteBytes(span.Slice(4, 4), Part2);
            BitConverter.TryWriteBytes(span.Slice(8, 4), Part3);
            BitConverter.TryWriteBytes(span.Slice(12, 4), Part4);
            
            return new Guid(bytes);
        }
        
        #endregion

        #region ToString

        public override string ToString() => ToHexString();

        public string ToHexString()
        {
            // return $"{Part1:X8}{Part2:X8}{Part3:X8}{Part4:X8}";
            
            Span<char> hexChars = stackalloc char[32];
            
            WriteUIntToHex(Part1, hexChars.Slice(0, 8));
            WriteUIntToHex(Part2, hexChars.Slice(8, 8));
            WriteUIntToHex(Part3, hexChars.Slice(16, 8));
            WriteUIntToHex(Part4, hexChars.Slice(24, 8));
            
            return new string(hexChars);
        }

        private static readonly char[] _hexDigits = "0123456789ABCDEF".ToCharArray();
        private static void WriteUIntToHex(uint value, Span<char> destination)
        {
            for (var i = 7; i >= 0; i--)
            {
                var digit = value & 0xF;
                destination[i] = _hexDigits[digit];
                value >>= 4;
            }
        }

        #endregion

        #region Equality

        public bool Equals(SerializableGuid other) =>
            Part1 == other.Part1 && Part2 == other.Part2 && Part3 == other.Part3 && Part4 == other.Part4;
        
        public bool Equals(Guid other) => Equals(new SerializableGuid(other));

        public bool Equals(string other) => 
            IsHexString(other) && string.Equals(ToHexString(), other, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object obj) => obj switch {
            SerializableGuid serializableGuid => Equals(serializableGuid),
            Guid guid => Equals(guid),
            string str => Equals(str),
            _ => false
        };

        #endregion

        #region HashCode

        public override int GetHashCode() => HashCode.Combine(Part1, Part2, Part3, Part4);

        #endregion

        #region Implicit Conversions

        public static implicit operator Guid(SerializableGuid serializableGuid) => serializableGuid.ToGuid();

        public static implicit operator SerializableGuid(Guid guid) => new(guid);

        #endregion

        #region Operators

        public static bool operator ==(SerializableGuid left, SerializableGuid right) => left.Equals(right);

        public static bool operator !=(SerializableGuid left, SerializableGuid right) => !left.Equals(right);

        public static bool operator ==(SerializableGuid left, Guid right) => left.Equals(right);
        
        public static bool operator !=(SerializableGuid left, Guid right) => !left.Equals(right);
        
        public static bool operator ==(Guid left, SerializableGuid right) => right.Equals(left);
        
        public static bool operator !=(Guid left, SerializableGuid right) => !right.Equals(left);

        public static bool operator ==(SerializableGuid left, string right) => left.Equals(right);
        
        public static bool operator !=(SerializableGuid left, string right) => !left.Equals(right);

        public static bool operator ==(string left, SerializableGuid right) => right.Equals(left);

        public static bool operator !=(string left, SerializableGuid right) => !right.Equals(left);

        #endregion
    }
}
