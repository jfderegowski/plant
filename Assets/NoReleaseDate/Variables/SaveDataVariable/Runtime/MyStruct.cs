using System;

namespace NoReleaseDate.Variables.SaveDataVariable.Runtime
{
    [Serializable]
    public struct MyStruct
    {
        public int Value;
        public string String;
        public BetterSaveData BetterSaveData;
    }
}