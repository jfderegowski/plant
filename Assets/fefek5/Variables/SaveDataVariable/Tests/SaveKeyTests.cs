using System;
using fefek5.Variables.SaveDataVariable.Runtime;
using fefek5.Variables.SerializableGuidVariable.Runtime;
using NUnit.Framework;

namespace fefek5.Variables.SaveDataVariable.Tests
{
    public class SaveKeyTests
{
    private SerializableGuid _guid1;
    private SerializableGuid _guid2;
    private string _guid1Hex;
    private string _stringKey1;
    private string _stringKey2;
    private string _comment1;

    [SetUp]
    public void Setup()
    {
        var dotnetGuid1 = Guid.NewGuid();
        _guid1 = new SerializableGuid(dotnetGuid1);
        _guid2 = SerializableGuid.NewGuid();
        _guid1Hex = _guid1.ToHexString();
        _stringKey1 = "MyStringKey";
        _stringKey2 = "AnotherStringKey";
        _comment1 = "My comment";
    }

    [Test]
    public void ConstructorWithGuidSetsCorrectFields()
    {
        var saveKey = new SaveKey(_guid1, _comment1);

        Assert.That(saveKey.Key, Is.EqualTo(_guid1));
        Assert.That(saveKey.StringKey.hasValue, Is.False);
        Assert.That(saveKey.StringKey.value, Is.EqualTo(string.Empty));
        Assert.That(saveKey.Comment.hasValue, Is.True);
        Assert.That(saveKey.Comment.value, Is.EqualTo(_comment1));
    }

    [Test]
    public void ConstructorWithStringSetsCorrectFields()
    {
        var saveKey = new SaveKey(_stringKey1, _comment1);

        Assert.That(saveKey.Key, Is.EqualTo(SerializableGuid.Empty));
        Assert.That(saveKey.StringKey.hasValue, Is.True);
        Assert.That(saveKey.StringKey.value, Is.EqualTo(_stringKey1));
        Assert.That(saveKey.Comment.hasValue, Is.True);
        Assert.That(saveKey.Comment.value, Is.EqualTo(_comment1));
    }

     [Test]
    public void ConstructorWithStringEmptyStringSetsCorrectFields()
    {
        var saveKey = new SaveKey("", _comment1);

        Assert.That(saveKey.Key, Is.EqualTo(SerializableGuid.Empty));
        Assert.That(saveKey.StringKey.hasValue, Is.True);
        Assert.That(saveKey.StringKey.value, Is.EqualTo(""));
        Assert.That(saveKey.Comment.hasValue, Is.True);
        Assert.That(saveKey.Comment.value, Is.EqualTo(_comment1));
        Assert.That(saveKey.IsEmpty(), Is.True);
    }

     [Test]
    public void ConstructorWithGuidEmptyCommentSetsCorrectFields()
    {
        var saveKey = new SaveKey(_guid1);

        Assert.That(saveKey.Key, Is.EqualTo(_guid1));
        Assert.That(saveKey.StringKey.hasValue, Is.False);
        Assert.That(saveKey.Comment.hasValue, Is.False);
        Assert.That(saveKey.Comment.value, Is.Null.Or.Empty);
    }

    [Test]
    public void CopyConstructorCopiesValuesCorrectly()
    {
        var originalGuidKey = new SaveKey(_guid1, _comment1);
        var copyGuidKey = new SaveKey(originalGuidKey);

        var originalStringKey = new SaveKey(_stringKey1, _comment1);
        var copyStringKey = new SaveKey(originalStringKey);

        // Test kopii klucza GUID
        Assert.That(copyGuidKey.Key, Is.EqualTo(originalGuidKey.Key));
        Assert.That(copyGuidKey.StringKey.hasValue, Is.EqualTo(originalGuidKey.StringKey.hasValue));
        Assert.That(copyGuidKey.StringKey.value, Is.EqualTo(originalGuidKey.StringKey.value));
        Assert.That(copyGuidKey.Comment.hasValue, Is.EqualTo(originalGuidKey.Comment.hasValue));
        Assert.That(copyGuidKey.Comment.value, Is.EqualTo(originalGuidKey.Comment.value));
        Assert.That(copyGuidKey, Is.EqualTo(originalGuidKey));

        // Test kopii klucza string
        Assert.That(copyStringKey.Key, Is.EqualTo(originalStringKey.Key));
        Assert.That(copyStringKey.StringKey.hasValue, Is.EqualTo(originalStringKey.StringKey.hasValue));
        Assert.That(copyStringKey.StringKey.value, Is.EqualTo(originalStringKey.StringKey.value));
        Assert.That(copyStringKey.Comment.hasValue, Is.EqualTo(originalStringKey.Comment.hasValue));
        Assert.That(copyStringKey.Comment.value, Is.EqualTo(originalStringKey.Comment.value));
        Assert.That(copyStringKey, Is.EqualTo(originalStringKey));
    }


    [Test]
    public void StaticPropertiesReturnCorrectKeys()
    {
        var randomKey = SaveKey.RandomKey;
        var emptyKey = SaveKey.Empty;

        Assert.That(randomKey.Key, Is.Not.EqualTo(SerializableGuid.Empty));
        Assert.That(randomKey.StringKey.hasValue, Is.False);
        Assert.That(randomKey.IsValid(), Is.True);

        Assert.That(emptyKey.Key, Is.EqualTo(SerializableGuid.Empty));
        Assert.That(emptyKey.StringKey.hasValue, Is.False);
        Assert.That(emptyKey.IsEmpty(), Is.True);
        Assert.That(emptyKey.IsValid(), Is.False);
    }

    [Test]
    public void SettersModifyKeyAndCommentCorrectly()
    {
        var saveKey = new SaveKey(_guid1);

        saveKey = saveKey.SetKey(_guid2);
        Assert.That(saveKey.Key, Is.EqualTo(_guid2), "SetKey failed");

        saveKey = saveKey.SetComment(_comment1);
        Assert.That(saveKey.Comment.hasValue, Is.True, "SetComment failed - hasValue");
        Assert.That(saveKey.Comment.value, Is.EqualTo(_comment1), "SetComment failed - value");


        saveKey = saveKey.SetComment(""); // Ustawienie pustego komentarza
        Assert.That(saveKey.Comment.hasValue, Is.False, "SetComment empty failed - hasValue");
        Assert.That(saveKey.Comment.value, Is.EqualTo(""), "SetComment empty failed - value");


        // Test SetStringKey (dodanej metody)
        saveKey = new SaveKey(_guid1); // Reset
        saveKey = saveKey.SetKey(_stringKey1);
        Assert.That(saveKey.StringKey.hasValue, Is.True, "SetStringKey failed - hasValue");
        Assert.That(saveKey.StringKey.value, Is.EqualTo(_stringKey1), "SetStringKey failed - value");
    }

    [Test]
    public void ConditionalsIsEmptyIsValidWorkCorrectly()
    {
        var guidKey = new SaveKey(_guid1);
        var stringKey = new SaveKey(_stringKey1);
        var emptyGuidKey = SaveKey.Empty;
        var emptyStringKey = new SaveKey("");

        Assert.That(guidKey.IsEmpty(), Is.False, "GuidKey IsEmpty");
        Assert.That(guidKey.IsValid(), Is.True, "GuidKey IsValid");

        Assert.That(stringKey.IsEmpty(), Is.False, "StringKey IsEmpty");
        Assert.That(stringKey.IsValid(), Is.True, "StringKey IsValid");

        Assert.That(emptyGuidKey.IsEmpty(), Is.True, "EmptyGuidKey IsEmpty");
        Assert.That(emptyGuidKey.IsValid(), Is.False, "EmptyGuidKey IsValid");

        Assert.That(emptyStringKey.IsEmpty(), Is.True, "EmptyStringKey IsEmpty");
        Assert.That(emptyStringKey.IsValid(), Is.False, "EmptyStringKey IsValid");
    }

    [Test]
    public void EqualitySaveKeyGuidBased()
    {
        var key1A = new SaveKey(_guid1);
        var key1B = new SaveKey(_guid1);
        var key2 = new SaveKey(_guid2);
        var emptyKey = SaveKey.Empty;

        Assert.That(key1A.Equals(key1B), Is.True, "key1a == key1b");
        Assert.That(key1A == key1B, Is.True, "key1a == key1b (operator)");
        Assert.That(key1A.Equals(key2), Is.False, "key1a != key2");
        Assert.That(key1A != key2, Is.True, "key1a != key2 (operator)");
        Assert.That(key1A.Equals(emptyKey), Is.False, "key1a != emptyKey");
        Assert.That(key1A != emptyKey, Is.True, "key1a != emptyKey (operator)");
        Assert.That(emptyKey.Equals(SaveKey.Empty), Is.True, "emptyKey == Empty");
    }

     [Test]
    public void EqualitySaveKeyStringBased()
    {
        var key1A = new SaveKey(_stringKey1);
        var key1B = new SaveKey(_stringKey1);
        var key2 = new SaveKey(_stringKey2);
        var emptyStringKey = new SaveKey("");

        Assert.That(key1A.Equals(key1B), Is.True, "key1a == key1b");
        Assert.That(key1A == key1B, Is.True, "key1a == key1b (operator)");
        Assert.That(key1A.Equals(key2), Is.False, "key1a != key2");
        Assert.That(key1A != key2, Is.True, "key1a != key2 (operator)");
        Assert.That(key1A.Equals(emptyStringKey), Is.False, "key1a != emptyStringKey");
        Assert.That(key1A != emptyStringKey, Is.True, "key1a != emptyStringKey (operator)");
        Assert.That(emptyStringKey.Equals(new SaveKey("")), Is.True, "emptyStringKey == emptyStringKey");
    }

     [Test]
    public void EqualitySaveKeyMixedTypesAreNotEqual()
    {
        var guidKey = new SaveKey(_guid1);
        var stringKey = new SaveKey(_stringKey1);
        var stringKeyLooksLikeGuid = new SaveKey(_guid1Hex);

        // Klucz GUID vs Klucz String - ZAWSZE false wg logiki Equals(SaveKey)
        Assert.That(guidKey.Equals(stringKey), Is.False, "GUID Key vs String Key");
        Assert.That(guidKey == stringKey, Is.False, "GUID Key == String Key (operator)");

        // Klucz GUID vs Klucz String (wyglądający jak GUID) - ZAWSZE true wg logiki Equals(SaveKey)
        Assert.That(guidKey.Equals(stringKeyLooksLikeGuid), Is.True, "GUID Key vs String Key (like GUID)");
        Assert.That(guidKey == stringKeyLooksLikeGuid, Is.True, "GUID Key == String Key (like GUID) (operator)");

        // Klucz String (wyglądający jak GUID) vs Klucz GUID - ZAWSZE true wg logiki Equals(SaveKey)
        Assert.That(stringKeyLooksLikeGuid.Equals(guidKey), Is.True, "String Key (like GUID) vs GUID Key");
        Assert.That(stringKeyLooksLikeGuid == guidKey, Is.True, "String Key (like GUID) == GUID Key (operator)");
    }


    [Test]
    public void EqualitySaveKeyWithSerializableGuid()
    {
        var guidKey = new SaveKey(_guid1);
        var stringKey = new SaveKey(_stringKey1);

        // Klucz GUID vs jego SerializableGuid - Powinny być równe
        Assert.That(guidKey.Equals(_guid1), Is.True, "GuidKey Equals guid1");
        Assert.That(guidKey == _guid1, Is.True, "GuidKey == guid1 (operator)");
        Assert.That(_guid1 == guidKey, Is.True, "guid1 == GuidKey (operator)");

        // Klucz GUID vs inny SerializableGuid - Powinny być różne
        Assert.That(guidKey.Equals(_guid2), Is.False, "GuidKey Not Equals guid2");
        Assert.That(guidKey != _guid2, Is.True, "GuidKey != guid2 (operator)");
        Assert.That(_guid2 != guidKey, Is.True, "guid2 != GuidKey (operator)");

        // Klucz String vs SerializableGuid - ZAWSZE false
        Assert.That(stringKey.Equals(_guid1), Is.False, "StringKey Not Equals guid1");
        Assert.That(stringKey != _guid1, Is.True, "StringKey != guid1 (operator)");
        Assert.That(_guid1 != stringKey, Is.True, "guid1 != StringKey (operator)");
    }

    [Test]
    public void EqualitySaveKeyWithString()
    {
        var guidKey = new SaveKey(_guid1);
        var stringKey = new SaveKey(_stringKey1);
        var emptyStringKey = new SaveKey("");

        // Klucz GUID vs jego Hex String - Powinny być równe
        Assert.That(guidKey.Equals(_guid1Hex), Is.True, "GuidKey Equals guid1Hex");
        Assert.That(guidKey == _guid1Hex, Is.True, "GuidKey == guid1Hex (operator)");
        Assert.That(_guid1Hex == guidKey, Is.True, "guid1Hex == GuidKey (operator)");

        // Klucz GUID vs inny String - Powinny być różne
        Assert.That(guidKey.Equals(_stringKey1), Is.False, "GuidKey Not Equals stringKey1");
        Assert.That(guidKey != _stringKey1, Is.True, "GuidKey != stringKey1 (operator)");
        Assert.That(_stringKey1 != guidKey, Is.True, "stringKey1 != GuidKey (operator)");

        // Klucz String vs jego String - Powinny być równe
        Assert.That(stringKey.Equals(_stringKey1), Is.True, "StringKey Equals stringKey1");
        Assert.That(stringKey == _stringKey1, Is.True, "StringKey == stringKey1 (operator)");
        Assert.That(_stringKey1 == stringKey, Is.True, "stringKey1 == StringKey (operator)");

        // Klucz String vs inny String - Powinny być różne
        Assert.That(stringKey.Equals(_stringKey2), Is.False, "StringKey Not Equals stringKey2");
        Assert.That(stringKey != _stringKey2, Is.True, "StringKey != stringKey2 (operator)");
        Assert.That(_stringKey2 != stringKey, Is.True, "stringKey2 != StringKey (operator)");

        // Klucz String vs Hex String - ZAWSZE false (bo StringKey ma pierwszeństwo)
        Assert.That(stringKey.Equals(_guid1Hex), Is.False, "StringKey Not Equals guid1Hex");
        Assert.That(stringKey != _guid1Hex, Is.True, "StringKey != guid1Hex (operator)");
        Assert.That(_guid1Hex != stringKey, Is.True, "guid1Hex != StringKey (operator)");

        // Pusty Klucz String vs Pusty String - Powinny być równe
        Assert.That(emptyStringKey.Equals(""), Is.True, "EmptyStringKey Equals \"\"");
        Assert.That(emptyStringKey == "", Is.True, "EmptyStringKey == \"\" (operator)");
        Assert.That("" == emptyStringKey, Is.True, "\"\" == EmptyStringKey (operator)");
    }

    [Test]
    public void EqualityWithObject()
    {
        var guidKey = new SaveKey(_guid1);
        var stringKey = new SaveKey(_stringKey1);
        object guidKeyObj = new SaveKey(_guid1);
        object stringKeyObj = new SaveKey(_stringKey1);
        object otherObj = new object();
        object nullObj = null;

        Assert.That(guidKey.Equals(guidKeyObj), Is.True, "GuidKey Equals object(GuidKey)");
        Assert.That(stringKey.Equals(stringKeyObj), Is.True, "StringKey Equals object(StringKey)");
        Assert.That(guidKey.Equals(otherObj), Is.False, "GuidKey Not Equals object(other)");
        Assert.That(guidKey.Equals(nullObj), Is.False, "GuidKey Not Equals object(null)");
        Assert.That(guidKey.Equals((object)_guid1), Is.True, "GuidKey Equals object(SerializableGuid)");
        Assert.That(guidKey.Equals((object)_guid1Hex), Is.True, "GuidKey Equals object(HexString)");
        Assert.That(stringKey.Equals((object)_stringKey1), Is.True, "StringKey Equals object(String)");
    }


    [Test]
    public void GetHashCodeConsistency()
    {
        var key1A = new SaveKey(_guid1);
        var key1B = new SaveKey(_guid1);
        var key2 = new SaveKey(_guid2);
        var stringKey1A = new SaveKey(_stringKey1);
        var stringKey1B = new SaveKey(_stringKey1);
        var emptyGuidKey = SaveKey.Empty;
        var emptyStringKey = new SaveKey("");

        Assert.That(key1A.GetHashCode(), Is.EqualTo(key1B.GetHashCode()), "HashCode GuidBased equal");
        Assert.That(key1A.GetHashCode(), Is.Not.EqualTo(key2.GetHashCode()), "HashCode GuidBased different");
        Assert.That(stringKey1A.GetHashCode(), Is.EqualTo(stringKey1B.GetHashCode()), "HashCode StringBased equal");
        Assert.That(key1A.GetHashCode(), Is.Not.EqualTo(stringKey1A.GetHashCode()), "HashCode GuidBased vs StringBased");
        Assert.That(emptyGuidKey.GetHashCode(), Is.Not.EqualTo(emptyStringKey.GetHashCode()), "HashCode EmptyGuid vs EmptyString");
    }

    [Test]
    public void ImplicitConversionFromGuid()
    {
        SaveKey saveKey = _guid1;

        Assert.That(saveKey.Key, Is.EqualTo(_guid1));
        Assert.That(saveKey.StringKey.hasValue, Is.False);
    }

    [Test]
    public void ImplicitConversionFromString()
    {
        SaveKey saveKey = _stringKey1;

        Assert.That(saveKey.Key, Is.EqualTo(SerializableGuid.Empty));
        Assert.That(saveKey.StringKey.hasValue, Is.True);
        Assert.That(saveKey.StringKey.value, Is.EqualTo(_stringKey1));
    }

    [Test]
    public void ImplicitConversionToGuid()
    {
        SaveKey guidKey = new SaveKey(_guid1);
        SaveKey stringKey = new SaveKey(_stringKey1);
        SaveKey emptyStringKey = new SaveKey("");

        SerializableGuid resultGuid = guidKey;
        SerializableGuid resultGuidFromStringKey = stringKey;
        SerializableGuid resultGuidFromEmptyStringKey = emptyStringKey;

        Assert.That(resultGuid, Is.EqualTo(_guid1), "GuidKey -> Guid");
        Assert.That(resultGuidFromStringKey, Is.EqualTo(SerializableGuid.Empty), "StringKey -> Guid");
        Assert.That(resultGuidFromEmptyStringKey, Is.EqualTo(SerializableGuid.Empty), "EmptyStringKey -> Guid");
    }

     [Test]
    public void ImplicitConversionToString()
    {
        SaveKey guidKey = new SaveKey(_guid1);
        SaveKey stringKey = new SaveKey(_stringKey1);
        SaveKey emptyStringKey = new SaveKey("");
        SaveKey emptyGuidKey = SaveKey.Empty;

        string resultStringFromGuid = guidKey;
        string resultStringFromString = stringKey;
        string resultStringFromEmptyString = emptyStringKey;
        string resultStringFromEmptyGuid = emptyGuidKey;

        Assert.That(resultStringFromGuid, Is.EqualTo(_guid1Hex), "GuidKey -> string");
        Assert.That(resultStringFromString, Is.EqualTo(_stringKey1), "StringKey -> string");
        Assert.That(resultStringFromEmptyString, Is.EqualTo(""), "EmptyStringKey -> string");
        Assert.That(resultStringFromEmptyGuid, Is.EqualTo(SerializableGuid.Empty.ToHexString()), "EmptyGuidKey -> string");
    }

    [Test]
    public void ToStringReturnsCorrectRepresentation()
    {
        var guidKey = new SaveKey(_guid1);
        var stringKey = new SaveKey(_stringKey1);
        var emptyStringKey = new SaveKey("");
        var emptyGuidKey = SaveKey.Empty;

        Assert.That(guidKey.ToString(), Is.EqualTo(_guid1Hex), "ToString GuidKey");
        Assert.That(stringKey.ToString(), Is.EqualTo(_stringKey1), "ToString StringKey");
        Assert.That(emptyStringKey.ToString(), Is.EqualTo(""), "ToString Empty StringKey");
        Assert.That(emptyGuidKey.ToString(), Is.EqualTo(SerializableGuid.Empty.ToHexString()), "ToString EmptyGuidKey");
    }
}
}