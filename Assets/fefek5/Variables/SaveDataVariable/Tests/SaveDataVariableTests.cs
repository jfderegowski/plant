using fefek5.Variables.SaveDataVariable.Runtime;
using fefek5.Variables.SerializableGuidVariable.Runtime;
using NUnit.Framework;

namespace fefek5.Variables.SaveDataVariable.Tests
{
    public class SaveDataVariableTests
    {
        [Test]
        public void CompareSaveKey()
        {
            const string stringKey = "stringKey";
            var serializableGuid = SerializableGuid.NewGuid();

            var saveKey1 = new SaveKey(stringKey);
            var saveKey2 = new SaveKey(stringKey);
            var saveKey3 = new SaveKey(serializableGuid);
            var saveKey4 = new SaveKey(serializableGuid);
            
            // String key to the same string key
            if (saveKey1 == saveKey2) Assert.Pass();
            else Assert.Fail();
            
            // String key to the same string key
            if (saveKey1 != saveKey2) Assert.Fail();
            else Assert.Pass();
            
            // Guid key to the same guid key
            if (saveKey3 == saveKey4) Assert.Pass();
            else Assert.Fail();
            
            // Guid key to the same guid key
            if (saveKey3 != saveKey4) Assert.Fail();
            else Assert.Pass();
            
            // String key to the guid key
            if (saveKey1 == saveKey3) Assert.Fail();
            else Assert.Pass();
            
            // String key to the guid key
            if (saveKey1 != saveKey3) Assert.Pass();
            else Assert.Fail();
        }
    }
}