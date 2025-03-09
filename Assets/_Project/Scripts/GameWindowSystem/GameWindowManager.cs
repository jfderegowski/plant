using System;
using UnityEngine;
using NoReleaseDate.Systems.SingletonSystem.Runtime;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using Object = UnityEngine.Object;

namespace Scripts.GameWindowSystem
{
    public class GameWindowManager : SingletonBehaviour<GameWindowManager>
    {
// #if UNITY_EDITOR
//         private GameWindowController _currentGameWindowController;
// #endif
//
//         protected override void Awake()
//         {
//             base.Awake();
//
//             switch (Application.platform)
//             {
// #if UNITY_EDITOR
//                 case RuntimePlatform.WindowsEditor:
//                     _currentGameWindowController = new WindowsEditor();
//                     break;
//                 case RuntimePlatform.OSXEditor:
//                     break;
// #endif
//                 case RuntimePlatform.WindowsPlayer:
//                     break;
//                 case RuntimePlatform.OSXPlayer:
//                     break;
//                 default:
//                     Debug.Log($"Platform not supported: {Application.platform}");
//                     break;
//             }
//
//             _currentGameWindowController?.Initialize();
//         }
//
// #if UNITY_EDITOR
//         protected override void OnDestroy()
//         {
//             base.OnDestroy();
//             _currentGameWindowController?.DeInitialize();
//         }
// #endif
//
//         private abstract class GameWindowController
//         {
//             internal abstract void Initialize();
//             internal abstract void DeInitialize();
//         }
//
//         private class WindowsEditor : GameWindowController
//         {
//             internal override void Initialize()
//             {
//                 foreach (var gameView in GetOpenGameViews())
//                 {
//                     Debug.Log($"GameView resolution: {GetGameViewResolution(gameView)}");
//                 }
//             }
//
//             internal override void DeInitialize()
//             {
//                 throw new System.NotImplementedException();
//             }
//             
//             public static Object[] GetOpenGameViews() => 
//                 Resources.FindObjectsOfTypeAll(System.Type.GetType("UnityEditor.GameView,UnityEditor"));
//             
//             public static Vector2 GetGameViewResolution(Object gameView)
//             {
//                 if (gameView == null)
//                 {
//                     Debug.LogError("GameView object is null.");
//                     return Vector2.zero;
//                 }
//
//                 // Get the GameView type
//                 Type gameViewType = gameView.GetType();
//
//                 // Get the "currentGameViewSize" property
//                 PropertyInfo currentSizeProperty = gameViewType.GetProperty("currentGameViewSize", BindingFlags.Instance | BindingFlags.NonPublic);
//                 if (currentSizeProperty == null)
//                 {
//                     Debug.LogError("Could not find 'currentGameViewSize' property in GameView.");
//                     return Vector2.zero;
//                 }
//
//                 // Get the value of the "currentGameViewSize" property
//                 object currentSize = currentSizeProperty.GetValue(gameView);
//                 if (currentSize == null)
//                 {
//                     Debug.LogError("'currentGameViewSize' property is null.");
//                     return Vector2.zero;
//                 }
//
//                 // Get the type of GameViewSize
//                 Type gameViewSizeType = currentSize.GetType();
//
//                 // Get the "width" and "height" properties
//                 PropertyInfo widthProperty = gameViewSizeType.GetProperty("width", BindingFlags.Instance | BindingFlags.Public);
//                 PropertyInfo heightProperty = gameViewSizeType.GetProperty("height", BindingFlags.Instance | BindingFlags.Public);
//
//                 if (widthProperty == null || heightProperty == null)
//                 {
//                     Debug.LogError("Could not find 'width' or 'height' properties in GameViewSize.");
//                     return Vector2.zero;
//                 }
//
//                 // Get the resolution as width and height
//                 int width = (int)widthProperty.GetValue(currentSize);
//                 int height = (int)heightProperty.GetValue(currentSize);
//
//                 return new Vector2(width, height);
//             }
//         }
    }
}
