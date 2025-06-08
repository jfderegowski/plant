using System;
using System.Collections.Generic;
using fefek5.Systems.ResourcesScriptableObjectSystem.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace fefek5.FavoritesWindow
{
    [Serializable]
    public struct Favorite
    {
        [TableList]
        public List<Object> Objects;
    }
    
    public class Favorites : ResourcesScriptableObject<Favorites>
    {
        [SerializeField, TableList] private List<Favorite> _favorites;
    }
}