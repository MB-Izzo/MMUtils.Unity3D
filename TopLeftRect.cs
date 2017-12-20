using System;
using UnityEngine;

namespace MM.Utils
{
    /// <summary>
    /// Rect with position defined on the bottom left corner.
    /// </summary>
    [Serializable]
    public class BottomLeftRect
    {
        [SerializeField]
        private Vector2 _origin;
        [SerializeField]
        private Vector2 _size;

        public Vector2 min { get { return _origin; } }
        public Vector2 max { get { return _origin + _size; } }
        public Vector2 center { get { return _origin + _size / 2.0f; } }
        public Vector2 size { get { return _size; } }
        public float width { get { return _size.x; } }
        public float height { get { return _size.y; } }

        public BottomLeftRect(Vector2 origin, Vector2 size)
        {
            _origin = origin;
            _size = size;
        }
    }
}