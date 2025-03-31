using System;

namespace fefek5.Variables.SerializableGuidVariable.Runtime
{
    public static class GuidExtensions
    {
        public static SerializableGuid ToSerializableGuid(this Guid guid) => new(guid);

        public static Guid ToGuid(this SerializableGuid serializableGuid)
        {
            var bytes = new byte[16];
            
            Buffer.BlockCopy(BitConverter.GetBytes(serializableGuid.Part1), 0, bytes, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(serializableGuid.Part2), 0, bytes, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(serializableGuid.Part3), 0, bytes, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(serializableGuid.Part4), 0, bytes, 12, 4);
            
            return new Guid(bytes);
        }
    }
}