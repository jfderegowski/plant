using System;
using System.Collections.Generic;
using fefek5.Systems.SingletonSystem.Runtime;
using UnityEngine;

namespace fefek5.Systems.CursorSystem.Runtime
{
    /// <summary>
    /// Cursor manager class. It manages the cursor settings and the cursor handlers.
    /// </summary>
    public class CursorManager : SingletonRawClass<CursorManager>
    {
        /// <summary>
        /// Event that is called when a cursor handler is registered.
        /// ICursorHandler is the registered cursor handler.
        /// </summary>
        public event Action<ICursorHandler> onRegister;

        /// <summary>
        /// Event that is called when a cursor handler is unregistered.
        /// ICursorHandler is the unregistered cursor handler.
        /// </summary>
        public event Action<ICursorHandler> onUnregister;

        /// <summary>
        /// The current cursor handler.
        /// </summary>
        public ICursorHandler CurrentCursorHandler => CursorHandlers.Count > 0 ? CursorHandlers[0] : null;

        /// <summary>
        /// The list of cursor handlers.
        /// </summary>
        public List<ICursorHandler> CursorHandlers { get; } = new();

        /// <summary>
        /// The current cursor settings.
        /// </summary>
        public CursorSettings CurrentCursorSettings => CurrentCursorHandler?.CursorSettings;

        protected override void OnDeinit()
        {
            base.OnDeinit();
            
            UnregisterAll();
        }

        /// <summary>
        /// Register a cursor handler.
        /// </summary>
        /// <param name="cursorHandler">The cursor handler to register.</param>
        public void Register(ICursorHandler cursorHandler)
        {
            CursorHandlers.Insert(0, cursorHandler);

            ActiveCursorHandler(cursorHandler);

            onRegister?.Invoke(cursorHandler);
        }

        /// <summary>
        /// Unregister a cursor handler.
        /// </summary>
        /// <param name="cursorHandler">The cursor handler to unregister.</param>
        public void Unregister(ICursorHandler cursorHandler)
        {
            CursorHandlers.Remove(cursorHandler);

            ActiveCursorHandler(CurrentCursorHandler);

            onUnregister?.Invoke(cursorHandler);
        }

        private void UnregisterAll()
        {
            for (var i = 0; i < CursorHandlers.Count; i++)
                Unregister(CursorHandlers[i]);
        }

        private void ActiveCursorHandler(ICursorHandler cursorHandler)
        {
            if (cursorHandler == null) return;

            if (CurrentCursorHandler != cursorHandler)
            {
                CursorHandlers.Remove(cursorHandler);
                CursorHandlers.Insert(0, cursorHandler);
            }

            SetCursorSettings(cursorHandler.CursorSettings);
        }

        private static void SetCursorSettings(CursorSettings cursorSettings)
        {
            Cursor.visible = cursorSettings.IsVisible;
            Cursor.lockState = cursorSettings.LockMode;
            Cursor.SetCursor(cursorSettings.Texture, cursorSettings.Hotspot, cursorSettings.CursorMode);
        }
    }
}