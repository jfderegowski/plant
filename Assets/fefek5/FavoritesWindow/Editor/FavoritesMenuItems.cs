using UnityEditor;

namespace NoReleaseDate.FavoritesWindow.Editor
{
    public static class FavoritesMenuItems
    {
        [MenuItem("No Release Date/Open Favorites Window")]
        private static void OpenFavoritesWindow() => EditorUtility.OpenPropertyEditor(Favorites.Instance);
    }
}