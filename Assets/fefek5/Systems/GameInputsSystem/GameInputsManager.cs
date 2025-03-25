using System;
using fefek5.Systems.SingletonSystem.Runtime;
using NoReleaseDate.Systems.GameInputsSystem;
using UnityEngine.InputSystem;

namespace fefek5.Systems.GameInputsSystem
{
    public abstract class GameInputsManager : SingletonRawClass<GameInputsManager>
    {
        #region Events

        public event Action<InputAction> onSameActionEnable;
        public event Action<InputAction> onSameActionDisable;
    
        internal void OnSomeActionEnable(InputAction inputAction) => onSameActionEnable?.Invoke(inputAction);
        internal void OnSomeActionDisable(InputAction inputAction) => onSameActionDisable?.Invoke(inputAction);

        #endregion
    
        public GameInputsActions LeftHand => _firstPlayer ??= new GameInputsActions();
        public GameInputsActions RightPlayer => _secondPlayer ??= new GameInputsActions();
        
        private GameInputsActions _firstPlayer;
        private GameInputsActions _secondPlayer;
    }
}