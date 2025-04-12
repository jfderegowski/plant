using fefek5.Variables.SerializableGuidVariable.Runtime;

namespace fefek5.Variables.SerializableGuidVariable.Tests
{
    using NUnit.Framework;
    using System;

    public class SerializableGuidTests
    {
        [Test]
        public void SerializableGuidEmptyShouldBeZero()
        {
            var emptyGuid = SerializableGuid.Empty;
            Assert.AreEqual(0u, emptyGuid.Part1);
            Assert.AreEqual(0u, emptyGuid.Part2);
            Assert.AreEqual(0u, emptyGuid.Part3);
            Assert.AreEqual(0u, emptyGuid.Part4);
        }

        [Test]
        public void SerializableGuidIsEmptyShouldReturnTrueForEmpty()
        {
            Assert.IsTrue(SerializableGuid.Empty.IsEmpty);
        }

        [Test]
        public void SerializableGuidIsEmptyShouldReturnFalseForNonEmpty()
        {
            var guid = new SerializableGuid(1, 2, 3, 4);
            Assert.IsFalse(guid.IsEmpty);
        }

        [Test]
        public void SerializableGuidConstructorFromUIntsShouldSetParts()
        {
            var guid = new SerializableGuid(1, 2, 3, 4);
            Assert.AreEqual(1u, guid.Part1);
            Assert.AreEqual(2u, guid.Part2);
            Assert.AreEqual(3u, guid.Part3);
            Assert.AreEqual(4u, guid.Part4);
        }

        [Test]
        public void SerializableGuidConstructorFromGuidShouldSetPartsCorrectly()
        {
            var originalGuid = Guid.NewGuid();
            var serializableGuid = new SerializableGuid(originalGuid);
            var convertedGuid = serializableGuid.ToGuid();

            Assert.AreEqual(originalGuid, convertedGuid);
        }

        [Test]
        public void SerializableGuidConstructorFromHexStringShouldSetPartsCorrectly()
        {
            var hexString = "0102030405060708090A0B0C0D0E0F10";
            var serializableGuid = new SerializableGuid(hexString);
            Assert.AreEqual(0x01020304u, serializableGuid.Part1);
            Assert.AreEqual(0x05060708u, serializableGuid.Part2);
            Assert.AreEqual(0x090A0B0Cu, serializableGuid.Part3);
            Assert.AreEqual(0x0D0E0F10u, serializableGuid.Part4);
        }

        [Test]
        public void SerializableGuidConstructorFromInvalidHexStringShouldThrowException()
        {
            Assert.Throws(typeof(ArgumentException), () => {
                var serializableGuid = new SerializableGuid("invalidHexString");
            });
        }

        [Test]
        public void SerializableGuidNewGuidShouldReturnANonEmptyGuid()
        {
            var guid = SerializableGuid.NewGuid();
            Assert.IsFalse(guid.IsEmpty);
        }

        [Test]
        public void SerializableGuidFromHexStringShouldReturnCorrectGuid()
        {
            var hexString = "0102030405060708090A0B0C0D0E0F10";
            var serializableGuid = SerializableGuid.FromHexString(hexString);
            Assert.AreEqual(0x01020304u, serializableGuid.Part1);
            Assert.AreEqual(0x05060708u, serializableGuid.Part2);
            Assert.AreEqual(0x090A0B0Cu, serializableGuid.Part3);
            Assert.AreEqual(0x0D0E0F10u, serializableGuid.Part4);
        }

        [Test]
        public void SerializableGuidFromHexStringShouldThrowException()
        {
            Assert.Throws(typeof(ArgumentException), () => {
                var serializableGuid = SerializableGuid.FromHexString("invalidHexString");
            });
        }

        [Test]
        public void SerializableGuidIsHexStringShouldReturnTrueForValidHexString()
        {
            Assert.IsTrue(SerializableGuid.IsHexString("0102030405060708090A0B0C0D0E0F10"));
        }

        [Test]
        public void SerializableGuidIsHexStringShouldReturnFalseForInvalidHexString()
        {
            Assert.IsFalse(SerializableGuid.IsHexString("invalid"));
            Assert.IsFalse(SerializableGuid.IsHexString("0102030405060708090A0B0C0D0E0F101"));
            Assert.IsFalse(SerializableGuid.IsHexString(null));
        }

        [Test]
        public void SerializableGuidToHexStringShouldReturnCorrectHexString()
        {
            var guid = new SerializableGuid(0x01020304, 0x05060708, 0x090A0B0C, 0x0D0E0F10);
            Assert.AreEqual("0102030405060708090A0B0C0D0E0F10", guid.ToHexString());
        }

        [Test]
        public void SerializableGuidToStringShouldReturnCorrectHexString()
        {
            var guid = new SerializableGuid(0x01020304, 0x05060708, 0x090A0B0C, 0x0D0E0F10);
            Assert.AreEqual("0102030405060708090A0B0C0D0E0F10", guid.ToString());
        }

        [Test]
        public void SerializableGuidToGuidShouldReturnCorrectGuid()
        {
            var expectedGuid = Guid.NewGuid();
            var serializableGuid = expectedGuid.ToSerializableGuid();
            var guid = serializableGuid.ToGuid();

            Assert.AreEqual(expectedGuid, guid);
        }

        [Test]
        public void SerializableGuidEqualsSerializableGuidShouldReturnTrueForEqualGuids()
        {
            var guid1 = new SerializableGuid(1, 2, 3, 4);
            var guid2 = new SerializableGuid(1, 2, 3, 4);
            Assert.IsTrue(guid1.Equals(guid2));
        }

        [Test]
        public void SerializableGuidEqualsSerializableGuidShouldReturnFalseForDifferentGuids()
        {
            var guid1 = new SerializableGuid(1, 2, 3, 4);
            var guid2 = new SerializableGuid(4, 3, 2, 1);
            Assert.IsFalse(guid1.Equals(guid2));
        }

        [Test]
        public void SerializableGuidEqualsGuidShouldReturnTrueForEqualGuids()
        {
            var guid1 = SerializableGuid.NewGuid();
            var guid2 = new Guid(guid1.ToGuid().ToByteArray());

            Assert.IsTrue(guid1.Equals(guid2));
        }

        [Test]
        public void SerializableGuidEqualsStringShouldReturnTrueForEqualHexStrings()
        {
            var guid = new SerializableGuid(0x01020304, 0x05060708, 0x090A0B0C, 0x0D0E0F10);
            Assert.IsTrue(guid.Equals("0102030405060708090A0B0C0D0E0F10"));
        }

        [Test]
        public void SerializableGuidEqualsStringShouldReturnFalseForDifferentHexStrings()
        {
            var guid = new SerializableGuid(1, 2, 3, 4);
            Assert.IsFalse(guid.Equals("0403020108070605090A0B0C0D0E0F10"));
        }

        [Test]
        public void SerializableGuidEqualsObjectShouldWorkCorrectly()
        {
            var guid = SerializableGuid.NewGuid();
            Assert.IsTrue(guid.Equals((object)guid));
            Assert.IsTrue(guid.Equals((object)guid.ToGuid()));
            Assert.IsTrue(guid.Equals((object)guid.ToHexString()));
            Assert.IsFalse(guid.Equals((object)123));
        }

        [Test]
        public void SerializableGuidGetHashCodeShouldReturnSameHashCodeForEqualGuids()
        {
            var guid1 = new SerializableGuid(1, 2, 3, 4);
            var guid2 = new SerializableGuid(1, 2, 3, 4);
            Assert.AreEqual(guid1.GetHashCode(), guid2.GetHashCode());
        }

        [Test]
        public void SerializableGuidImplicitOperatorGuidToSerializableGuid()
        {
            var originalGuid = Guid.NewGuid();
            SerializableGuid serializableGuid = originalGuid;
            Assert.AreEqual(originalGuid, serializableGuid.ToGuid());
        }

        [Test]
        public void SerializableGuidImplicitOperatorSerializableGuidToGuid()
        {
            var serializableGuid = new SerializableGuid(Guid.NewGuid());
            Guid guid = serializableGuid;
            Assert.AreEqual(serializableGuid.ToGuid(), guid);
        }

        [Test]
        public void SerializableGuidOperatorEqualsAndNotEqualsSerializableGuid()
        {
            var guid1 = new SerializableGuid(1, 2, 3, 4);
            var guid2 = new SerializableGuid(1, 2, 3, 4);
            var guid3 = new SerializableGuid(4, 3, 2, 1);
            Assert.IsTrue(guid1 == guid2);
            Assert.IsFalse(guid1 == guid3);
            Assert.IsFalse(guid1 != guid2);
            Assert.IsTrue(guid1 != guid3);
        }

        [Test]
        public void SerializableGuidOperatorEqualsAndNotEqualsGuid()
        {
            var serializableGuid = new SerializableGuid(0x01020304, 0x05060708, 0x090A0B0C, 0x0D0E0F10);
            var guid = serializableGuid.ToGuid();
            var guid2 = new Guid("04030201-0807-0605-090A-0B0C0D0E0F10");

            Assert.IsTrue(serializableGuid == guid);
            Assert.IsFalse(serializableGuid == guid2);
            Assert.IsFalse(serializableGuid != guid);
            Assert.IsTrue(serializableGuid != guid2);
        }

        [Test]
        public void SerializableGuidOperatorEqualsAndNotEqualsString()
        {
            var guid = new SerializableGuid(0x01020304, 0x05060708, 0x090A0B0C, 0x0D0E0F10);
            Assert.IsTrue(guid == "0102030405060708090A0B0C0D0E0F10");
            Assert.IsFalse(guid == "0403020108070605090A0B0C0D0E0F10");
            Assert.IsFalse(guid != "0102030405060708090A0B0C0D0E0F10");
            Assert.IsTrue(guid != "0403020108070605090A0B0C0D0E0F10");
        }
    }
}