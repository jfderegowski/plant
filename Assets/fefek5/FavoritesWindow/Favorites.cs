using System.Collections.Generic;
using fefek5.Systems.SingletonSystem.Runtime;
using UnityEngine;

namespace fefek5.FavoritesWindow
{
    public class Favorites : SingletonObject<Favorites>
    {
        [SerializeField] private List<Object> _favorites;
    }
}