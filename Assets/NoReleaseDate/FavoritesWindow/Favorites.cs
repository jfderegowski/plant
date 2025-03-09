using System.Collections.Generic;
using NoReleaseDate.Systems.SingletonSystem.Runtime;
using UnityEngine;

namespace NoReleaseDate.FavoritesWindow
{
    public class Favorites : SingletonObject<Favorites>
    {
        [SerializeField] private List<Object> _favorites;
    }
}