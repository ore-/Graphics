using MessagePack;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// These are for packing post processing parameters.
namespace Graphics.Settings
{
    // Well I didn't want to do this but MessagePack actually creates new class so few things should be moved out.
    public static class SettingValues
    {
        public static PostProcessProfile profile;
        public static PostProcessProfile defaultProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct FloatValue
    {
        public float value;
        public bool overrideState;
        public FloatValue(float value, bool overrideState)
        {
            this.value = value;
            this.overrideState = overrideState;
        }
        public FloatValue(FloatParameter parameter)
        {
            value = parameter.value;
            overrideState = parameter.overrideState;
        }
        public void Fill(FloatParameter parameter)
        {
            parameter.value = value;
            parameter.overrideState = overrideState;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct IntValue
    {
        public int value;
        public bool overrideState;
        public IntValue(int value, bool overrideState)
        {
            this.value = value;
            this.overrideState = overrideState;
        }
        public IntValue(IntParameter parameter)
        {
            value = parameter.value;
            overrideState = parameter.overrideState;
        }
        public void Fill(IntParameter parameter)
        {
            parameter.value = value;
            parameter.overrideState = overrideState;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct BoolValue
    {
        public bool value;
        public bool overrideState;
        public BoolValue(bool value, bool overrideState)
        {
            this.value = value;
            this.overrideState = overrideState;
        }
        public BoolValue(BoolParameter parameter)
        {
            value = parameter.value;
            overrideState = parameter.overrideState;
        }
        public void Fill(BoolParameter parameter)
        {
            parameter.value = value;
            parameter.overrideState = overrideState;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct Vector2Value
    {
        public float[] value;
        public bool overrideState;
        public Vector2Value(float[] value, bool overrideState)
        {
            this.value = value;
            this.overrideState = overrideState;
        }
        public Vector2Value(Vector2Parameter parameter)
        {
            value = new float[2] { parameter.value[0], parameter.value[1] };
            overrideState = parameter.overrideState;
        }
        public void Fill(Vector2Parameter parameter)
        {
            parameter.value = new Vector2(value[0], value[1]);
            parameter.overrideState = overrideState;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct Vector3Value
    {
        public float[] value;
        public bool overrideState;
        public Vector3Value(float[] value, bool overrideState)
        {
            this.value = value;
            this.overrideState = overrideState;
        }
        public Vector3Value(Vector3Parameter parameter)
        {
            value = new float[3] { parameter.value[0], parameter.value[1], parameter.value[2] };
            overrideState = parameter.overrideState;
        }
        public void Fill(Vector3Parameter parameter)
        {
            parameter.value = new Vector3(value[0], value[1], value[2]);
            parameter.overrideState = overrideState;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct Vector4Value
    {
        public float[] value;
        public bool overrideState;
        public Vector4Value(float[] value, bool overrideState)
        {
            this.value = value;
            this.overrideState = overrideState;
        }
        public Vector4Value(Vector4Parameter parameter)
        {
            value = new float[4] { parameter.value[0], parameter.value[1], parameter.value[2], parameter.value[3] };
            overrideState = parameter.overrideState;
        }
        public void Fill(Vector4Parameter parameter)
        {
            parameter.value = new Vector4(value[0], value[1], value[2], value[3]);
            parameter.overrideState = overrideState;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct ColorValue
    {
        public float[] value;
        public bool overrideState;
        public ColorValue(float[] value, bool overrideState)
        {
            this.value = value;
            this.overrideState = overrideState;
        }
        public ColorValue(ColorParameter parameter)
        {
            value = new float[3] { parameter.value[0], parameter.value[1], parameter.value[2] };
            overrideState = parameter.overrideState;
        }
        public void Fill(ColorParameter parameter)
        {
            parameter.value = new Color(value[0], value[1], value[2]);
            parameter.overrideState = overrideState;
        }
    }

    // Unique Parameters for Post Processing


    [MessagePackObject(keyAsPropertyName: true)]
    public struct EyeAdaptationValue
    {
        public EyeAdaptation value;
        public bool overrideState;
        public EyeAdaptationValue(EyeAdaptation value, bool overrideState)
        {
            this.value = value;
            this.overrideState = overrideState;
        }
        public EyeAdaptationValue(EyeAdaptationParameter parameter)
        {
            value = parameter.value;
            overrideState = parameter.overrideState;
        }
        public void Fill(EyeAdaptationParameter parameter)
        {
            parameter.value = value;
            parameter.overrideState = overrideState;
        }
    }
    [MessagePackObject(keyAsPropertyName: true)]
    public struct AmbientOcclusionModeValue
    {
        public AmbientOcclusionMode value;
        public bool overrideState;
        public AmbientOcclusionModeValue(AmbientOcclusionMode value, bool overrideState)
        {
            this.value = value;
            this.overrideState = overrideState;
        }
        public AmbientOcclusionModeValue(AmbientOcclusionModeParameter parameter)
        {
            value = parameter.value;
            overrideState = parameter.overrideState;
        }
        public void Fill(AmbientOcclusionModeParameter parameter)
        {
            parameter.value = value;
            parameter.overrideState = overrideState;
        }
    }
    [MessagePackObject(keyAsPropertyName: true)]
    public struct AmbientOcclusionQualityValue
    {
        public AmbientOcclusionQuality value;
        public bool overrideState;
        public AmbientOcclusionQualityValue(AmbientOcclusionQuality value, bool overrideState)
        {
            this.value = value;
            this.overrideState = overrideState;
        }
        public AmbientOcclusionQualityValue(AmbientOcclusionQualityParameter parameter)
        {
            value = parameter.value;
            overrideState = parameter.overrideState;
        }
        public void Fill(AmbientOcclusionQualityParameter parameter)
        {
            parameter.value = value;
            parameter.overrideState = overrideState;
        }
    }
    [MessagePackObject(keyAsPropertyName: true)]
    public struct GradingModeValue
    {
        public GradingMode value;
        public bool overrideState;
        public GradingModeValue(GradingMode value, bool overrideState)
        {
            this.value = value;
            this.overrideState = overrideState;
        }
        public GradingModeValue(GradingModeParameter parameter)
        {
            value = parameter.value;
            overrideState = parameter.overrideState;
        }
        public void Fill(GradingModeParameter parameter)
        {
            parameter.value = value;
            parameter.overrideState = overrideState;
        }
    }
    [MessagePackObject(keyAsPropertyName: true)]
    public struct TonemapperValue
    {
        public Tonemapper value;
        public bool overrideState;
        public TonemapperValue(Tonemapper value, bool overrideState)
        {
            this.value = value;
            this.overrideState = overrideState;
        }
        public TonemapperValue(TonemapperParameter parameter)
        {
            value = parameter.value;
            overrideState = parameter.overrideState;
        }
        public void Fill(TonemapperParameter parameter)
        {
            parameter.value = value;
            parameter.overrideState = overrideState;
        }
    }
    [MessagePackObject(keyAsPropertyName: true)]
    public struct KernelSizeValue
    {
        public KernelSize value;
        public bool overrideState;
        public KernelSizeValue(KernelSize value, bool overrideState)
        {
            this.value = value;
            this.overrideState = overrideState;
        }
        public KernelSizeValue(KernelSizeParameter parameter)
        {
            value = parameter.value;
            overrideState = parameter.overrideState;
        }
        public void Fill(KernelSizeParameter parameter)
        {
            parameter.value = value;
            parameter.overrideState = overrideState;
        }
    }
    [MessagePackObject(keyAsPropertyName: true)]
    public struct ScreenSpaceReflectionPresetValue
    {
        public ScreenSpaceReflectionPreset value;
        public bool overrideState;
        public ScreenSpaceReflectionPresetValue(ScreenSpaceReflectionPreset value, bool overrideState)
        {
            this.value = value;
            this.overrideState = overrideState;
        }
        public ScreenSpaceReflectionPresetValue(ScreenSpaceReflectionPresetParameter parameter)
        {
            value = parameter.value;
            overrideState = parameter.overrideState;
        }
        public void Fill(ScreenSpaceReflectionPresetParameter parameter)
        {
            parameter.value = value;
            parameter.overrideState = overrideState;
        }
    }
    [MessagePackObject(keyAsPropertyName: true)]
    public struct ScreenSpaceReflectionResolutionValue
    {
        public ScreenSpaceReflectionResolution value;
        public bool overrideState;
        public ScreenSpaceReflectionResolutionValue(ScreenSpaceReflectionResolution value, bool overrideState)
        {
            this.value = value;
            this.overrideState = overrideState;
        }
        public ScreenSpaceReflectionResolutionValue(ScreenSpaceReflectionResolutionParameter parameter)
        {
            value = parameter.value;
            overrideState = parameter.overrideState;
        }
        public void Fill(ScreenSpaceReflectionResolutionParameter parameter)
        {
            parameter.value = value;
            parameter.overrideState = overrideState;
        }
    }
    [MessagePackObject(keyAsPropertyName: true)]
    public struct VignetteModeValue
    {
        public VignetteMode value;
        public bool overrideState;
        public VignetteModeValue(VignetteMode value, bool overrideState)
        {
            this.value = value;
            this.overrideState = overrideState;
        }
        public VignetteModeValue(VignetteModeParameter parameter)
        {
            value = parameter.value;
            overrideState = parameter.overrideState;
        }
        public void Fill(VignetteModeParameter parameter)
        {
            parameter.value = value;
            parameter.overrideState = overrideState;
        }
    }

}
