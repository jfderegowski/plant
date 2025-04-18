using System.Collections;
using System.Collections.Generic;
using fefek5.Systems.SingletonSystem.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace fefek5.Systems.UIRebuilderSystem.Runtime
{
    /// <summary>
    /// Rebuilds UI elements that need to be rebuilt.
    /// </summary>
    public class UIRebuilder : SingletonBehaviour<UIRebuilder>
    {
        private bool _isRebuilding;
        
        private readonly HashSet<RectTransform> _rectTransforms = new();

        /// <summary>
        /// Rebuilds the given RectTransform.
        /// </summary>
        /// <param name="rectTransform">The RectTransform to rebuild.</param>
        public void Rebuild(RectTransform rectTransform)
        {
            if (!rectTransform) return;
            
            if (!_rectTransforms.Add(rectTransform)) return;

            AskForRebuild();
        }
        
        private void AskForRebuild()
        {
            if (_isRebuilding) return;

            _isRebuilding = true;
            
            StartCoroutine(ExecuteRebuild());
        }
        
        private IEnumerator ExecuteRebuild()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            foreach (var rectTransform in _rectTransforms) 
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            
            _rectTransforms.Clear();
            
            _isRebuilding = false;
        }
    }
}
