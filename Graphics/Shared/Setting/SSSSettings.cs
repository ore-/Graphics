using System.Diagnostics.CodeAnalysis;
using MessagePack;
using UnityEngine;

namespace Graphics.Settings
{
    [MessagePackObject(true)]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class SSSSettings
    {
        public float BlurSize = 0.1f; // Small Values by default.
        public float[] Color = {1f, 0f, 0f}; // red by default.
        public bool Debug = false;
        public bool DebugDistance = false;
        public float DepthTest;
        public bool Dither; // disabled by default.
        public float DitherIntensity;
        public float DitherScale;
        public float DownscaleFactor = 1; // Default Resolution by default.
        public bool EdgeDitherNoise;
        public bool Enabled; // disabled by default.
        public bool FixPixelLeaks;
        public float FixPixelLeaksNormal;
        public int LayerBitMask;
        public int MaxDistance = 10000;
        public float NormalTest;
        public int ProcessIterations = 1;
        public float ProfileColorTest;
        public bool ProfilePerObject;
        public float ProfileRadiusTest;
        public bool ProfileTest;
        public int ShaderIterations = 1;
        public SSS.ToggleTexture ToggleTexture;

        public void SaveParameters()
        {
            SSS instance = SSSManager.SSSInstance;
            if (ReferenceEquals(null, instance)) return;

            Enabled = instance.Enabled;
            ProfilePerObject = instance.ProfilePerObject;
            Color = new[] {instance.sssColor.r, instance.sssColor.g, instance.sssColor.b};
            BlurSize = instance.ScatteringRadius;
            ProcessIterations = instance.ScatteringIterations;
            ShaderIterations = instance.ShaderIterations;
            DownscaleFactor = instance.Downsampling;
            MaxDistance = instance.maxDistance;
            LayerBitMask = instance.SSS_Layer;

            Dither = instance.Dither;
            DitherIntensity = instance.DitherIntensity;
            DitherScale = instance.DitherScale;

            ToggleTexture = instance.toggleTexture;
            DepthTest = instance.DepthTest;
            NormalTest = instance.NormalTest;
            EdgeDitherNoise = instance.DitherEdgeTest;
            FixPixelLeaks = instance.FixPixelLeaks;
            FixPixelLeaksNormal = instance.EdgeOffset;
            ProfileTest = instance.UseProfileTest;
            ProfileColorTest = instance.ProfileColorTest;
            ProfileRadiusTest = instance.ProfileRadiusTest;
        }

        private void RescueWithHelicopter()
        {
            BlurSize = Mathf.Clamp(BlurSize, 0, 100);
            ProcessIterations = Mathf.Clamp(ProcessIterations, 1, 100);
            ShaderIterations = Mathf.Clamp(ShaderIterations, 1, 100);
            DownscaleFactor = Mathf.Clamp(DownscaleFactor, 0.5f, 100);
        }

        public void LoadParameters()
        {
            SSS instance = SSSManager.SSSInstance;
            if (ReferenceEquals(null, instance)) return;
            RescueWithHelicopter();
            if (!Enabled) Graphics.Instance.SSSManager.Destroy();
            else Graphics.Instance.SSSManager.Start();

            instance.Enabled = Enabled;
            instance.ProfilePerObject = ProfilePerObject;
            instance.sssColor = ReferenceEquals(Color, null) ? UnityEngine.Color.white : new Color(Color[0], Color[1], Color[2]);
            instance.ScatteringRadius = BlurSize;
            instance.ScatteringIterations = ProcessIterations;
            instance.ShaderIterations = ShaderIterations;
            instance.Downsampling = DownscaleFactor;
            instance.maxDistance = MaxDistance;
            instance.SSS_Layer = LayerBitMask;

            instance.Dither = Dither;
            instance.DitherIntensity = DitherIntensity;
            instance.DitherScale = DitherScale;

            instance.toggleTexture = ToggleTexture;
            instance.DepthTest = DepthTest;
            instance.NormalTest = NormalTest;
            instance.DitherEdgeTest = EdgeDitherNoise;
            instance.FixPixelLeaks = FixPixelLeaks;
            instance.EdgeOffset = FixPixelLeaksNormal;
            instance.UseProfileTest = ProfileTest;
            instance.ProfileColorTest = ProfileColorTest;
            instance.ProfileRadiusTest = ProfileRadiusTest;
        }
    }
}