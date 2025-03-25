using System;
using UnityEngine;

namespace fefek5.Systems.DetectorSystem
{
    [Serializable]
    public struct DetectedArgs<T>
    {
        public T detected;
        public RaycastHit hit;
        
        public DetectedArgs(T detected, RaycastHit hit)
        {
            this.detected = detected;
            this.hit = hit;
        }
    }
}