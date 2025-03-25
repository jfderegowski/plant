using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace fefek5.Systems.GameInputsSystem
{
    public static class GameInputsManagerExtensions
    {
        private static readonly Dictionary<InputAction, Action<InputAction>> _onEnableCallbacks = new();
        private static readonly Dictionary<InputAction, Action<InputAction>> _onDisableCallbacks = new();
    
        public static void EnableWithCallback(this InputAction inputAction)
        {
            if (inputAction.enabled) return;
        
            inputAction.Enable();
        
            if (_onEnableCallbacks.TryGetValue(inputAction, out var onEnableCallback))
                onEnableCallback?.Invoke(inputAction);

            GameInputsManager.Instance.OnSomeActionEnable(inputAction);
        }
    
        public static void DisableWithCallback(this InputAction inputAction)
        {
            if (!inputAction.enabled) return;
        
            inputAction.Disable();
        
            if (_onDisableCallbacks.TryGetValue(inputAction, out var onDisableCallback))
                onDisableCallback?.Invoke(inputAction);
        
            GameInputsManager.Instance.OnSomeActionDisable(inputAction);
        }
    
        public static void OnEnable(this InputAction inputAction, Action<InputAction> callback) => 
            _onEnableCallbacks[inputAction] += callback;
    
        public static void OnDisable(this InputAction inputAction, Action<InputAction> callback) =>
            _onDisableCallbacks[inputAction] += callback;
    }
}