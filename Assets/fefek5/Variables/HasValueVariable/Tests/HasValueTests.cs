using fefek5.Variables.HasValueVariable.Runtime;
using NUnit.Framework;

namespace fefek5.Variables.HasValueVariable.Tests
{
    public class HasValueTests
    {
        [Test]
        public void HasValueEqualsSameValueAndHasValueReturnsTrue()
        {
            var value1 = new HasValue<int>(10, true);
            var value2 = new HasValue<int>(10, true);
            Assert.IsTrue(value1.Equals(value2));
        }

        [Test]
        public void HasValueEqualsSameValueDifferentHasValueReturnsFalse()
        {
            var value1 = new HasValue<int>(10, true);
            var value2 = new HasValue<int>(10, false);
            Assert.IsTrue(value1.Equals(value2));
        }

        [Test]
        public void HasValueEqualsDifferentValueSameHasValueReturnsFalse()
        {
            var value1 = new HasValue<int>(10, true);
            var value2 = new HasValue<int>(20, true);
            Assert.IsFalse(value1.Equals(value2));
        }

        [Test]
        public void HasValueEqualsDifferentValueDifferentHasValueReturnsFalse()
        {
            var value1 = new HasValue<int>(10, true);
            var value2 = new HasValue<int>(20, false);
            Assert.IsFalse(value1.Equals(value2));
        }

        [Test]
        public void HasValueEqualsHasValueAndTSameValueReturnsTrue()
        {
            var value = new HasValue<int>(10, true);
            Assert.IsTrue(value.Equals(10));
        }

        [Test]
        public void HasValueEqualsHasValueAndTDifferentValueReturnsFalse()
        {
            var value = new HasValue<int>(10, true);
            Assert.IsFalse(value.Equals(20));
        }

        [Test]
        public void HasValueEqualsObjectHasValueSameValueAndHasValueReturnsTrue()
        {
            var value1 = new HasValue<int>(10, true);
            object value2 = new HasValue<int>(10, true);
            Assert.IsTrue(value1.Equals(value2));
        }

        [Test]
        public void HasValueEqualsObjectTSameValueReturnsTrue()
        {
            var value = new HasValue<int>(10, true);
            object obj = 10;
            Assert.IsTrue(value.Equals(obj));
        }

        [Test]
        public void HasValueEqualsObjectNullReturnsFalse()
        {
            var value = new HasValue<int>(10, true);
            object obj = null;
            Assert.IsFalse(value.Equals(obj));
        }

        [Test]
        public void HasValueEqualityOperatorSameValueAndHasValueReturnsTrue()
        {
            var value1 = new HasValue<int>(10, true);
            var value2 = new HasValue<int>(10, true);
            Assert.IsTrue(value1 == value2);
        }

        [Test]
        public void HasValueInequalityOperatorDifferentValueReturnsTrue()
        {
            var value1 = new HasValue<int>(10, true);
            var value2 = new HasValue<int>(20, true);
            Assert.IsTrue(value1 != value2);
        }

        [Test]
        public void HasValueEqualityOperatorHasValueAndTSameValueReturnsTrue()
        {
            var value = new HasValue<int>(10, true);
            Assert.IsTrue(value == 10);
        }

        [Test]
        public void HasValueInequalityOperatorHasValueAndTDifferentValueReturnsTrue()
        {
            var value = new HasValue<int>(10, true);
            Assert.IsTrue(value != 20);
        }

        [Test]
        public void HasValueImplicitConversionTToHasValueValueAndHasValueAreCorrect()
        {
            const int intValue = 10;
            HasValue<int> hasValue = intValue;
            Assert.AreEqual(10, hasValue.value);
            Assert.IsTrue(hasValue.hasValue);
        }

        [Test]
        public void HasValueImplicitConversionHasValueToTValueIsCorrect()
        {
            var hasValue = new HasValue<int>(10, true);
            int intValue = hasValue;
            Assert.AreEqual(10, intValue);
        }

        [Test]
        public void HasValueToStringHasValueTrueReturnsValueAsString()
        {
            var value = new HasValue<int>(10, true);
            Assert.AreEqual("10", value.ToString());
        }

        [Test]
        public void HasValueToStringHasValueFalseReturnsNull()
        {
            var value = new HasValue<int>(10, false);
            Assert.AreEqual("10", value.ToString());
        }

        [Test]
        public void HasValueGetHashCodeSameValueAndHasValueSameHashCode()
        {
            var value1 = new HasValue<int>(10, true);
            var value2 = new HasValue<int>(10, true);
            Assert.AreEqual(value1.GetHashCode(), value2.GetHashCode());
        }

        [Test]
        public void HasValueGetHashCodeDifferentHasValueDifferentHashCode()
        {
            var value1 = new HasValue<int>(10, true);
            var value2 = new HasValue<int>(10, false);
            Assert.AreEqual(value1.GetHashCode(), value2.GetHashCode());
        }
    }
}