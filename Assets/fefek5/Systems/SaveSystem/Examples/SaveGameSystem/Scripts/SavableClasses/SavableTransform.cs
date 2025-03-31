using fefek5.Common.Runtime.Serializable;
using fefek5.Variables.HasValueVariable.Runtime;
using fefek5.Variables.SaveDataVariable.Runtime;
using UnityEngine;

namespace fefek5.Systems.SaveSystem.Examples.SaveGameSystem.Scripts.SavableClasses
{
    public class SavableTransform : SavableMonoBehaviour
    {
        [SerializeField] private HasValue<SaveKey> _positionSaveKey = new(SaveKey.RandomKey.SetComment("Position"), true);
        [SerializeField] private HasValue<SaveKey> _rotationSaveKey = new(SaveKey.RandomKey.SetComment("Rotation"), true);
        [SerializeField] private HasValue<SaveKey> _scaleSaveKey = new(SaveKey.RandomKey.SetComment("Scale"), false);

        private SerializableVector3 CurrentPosition => Transform.position;
        private SerializableQuaternion CurrentRotation => Transform.rotation;
        private SerializableVector3 CurrentScale => Transform.localScale;
        private Transform Transform => _transform ? _transform : _transform = transform;
        
        private Transform _transform;

        // private ClassData GetCurrentData()
        // {
        //     var saveData = new ClassData();
        //
        //     if (_positionSaveKey.hasValue)
        //         saveData.SetKey(_positionSaveKey, CurrentPosition);
        //
        //     if (_rotationSaveKey.hasValue)
        //         saveData.SetKey(_rotationSaveKey, CurrentRotation);
        //
        //     if (_scaleSaveKey.hasValue)
        //         saveData.SetKey(_scaleSaveKey, CurrentScale);
        //
        //     return saveData;
        // }
        //
        // #region SaveBehaviour
        //
        // public override ClassData DefSaveData => GetCurrentData();
        // public override ClassData DataToSave => GetCurrentData();
        //
        // public override void OnLoad(ClassData savedData)
        // {
        //     if (savedData == null) return;
        //
        //     if (_positionSaveKey.hasValue) 
        //         transform.position = savedData.GetKey(_positionSaveKey, CurrentPosition);
        //
        //     if (_rotationSaveKey.hasValue) 
        //         transform.rotation = savedData.GetKey(_rotationSaveKey, CurrentRotation);
        //
        //     if (_scaleSaveKey.hasValue) 
        //         transform.localScale = savedData.GetKey(_scaleSaveKey, CurrentScale);
        // }
        //
        // #endregion
    }
}