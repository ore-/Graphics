namespace Graphics.Settings
{
    internal class LightSettings
    {
        internal static float IntensityMin = 0f;
        internal static float IntensityMax = 8f;
        internal static float IntensityDefault = 1f;
        internal static float RotationXMin = -90f;
        internal static float RotationXMax = 90f;
        internal static float RotationYMin = -179.9f;
        internal static float RotationYMax = 180f;

        internal enum LightType
        {
            Directional,
            Point,
            Spot
        }
    }
}