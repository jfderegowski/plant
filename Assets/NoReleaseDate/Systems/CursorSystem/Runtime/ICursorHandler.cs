﻿namespace NoReleaseDate.Plugins.CursorSystem.Runtime
{
    /// <summary>
    /// Interface for cursor handlers.
    /// </summary>
    public interface ICursorHandler
    {
        /// <summary>
        /// The cursor settings.
        /// </summary>
        public CursorSettings CursorSettings { get; }
    }
}