using AIGraphics;
using MessagePack;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace AIGraphics.Settings
{
    [MessagePackObject(keyAsPropertyName: true)]
    internal class LightSettings
    {
        internal static float IntensityMin = 0f;
        internal static float IntensityMax = 8f;
        internal static float IntensityDefault = 1f;
        internal static float RotationXMin = -90f;
        internal static float RotationXMax = 90f;
        internal static float RotationYMin = -179.9f;
        internal static float RotationYMax = 180f;
    }
}
