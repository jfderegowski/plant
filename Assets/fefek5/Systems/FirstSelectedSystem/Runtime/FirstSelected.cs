using UnityEngine;

namespace fefek5.Systems.FirstSelectedSystem.Runtime
{
    public class FirstSelected : MonoBehaviour
    {
        [field: SerializeField] public virtual RectTransform toBeSelected { get; private set; }
        
        private void OnEnable() => 
            FirstSelectedManager.Instance.Register(toBeSelected);
        
        private void OnDisable() =>
            FirstSelectedManager.Instance.Unregister(toBeSelected);
    }
}