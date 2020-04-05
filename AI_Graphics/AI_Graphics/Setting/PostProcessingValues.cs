using MessagePack;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// These are for packing post processing parameters.
namespace AIGraphics.Settings {
    [MessagePackObject(keyAsPropertyName: true)]
    public struct FloatValue {
        public float value;
        public bool overrideState;
        public FloatValue(float value, bool overrideState) {
            this.value = value;
            this.overrideState = overrideState;
        }
        public FloatValue(FloatParameter parameter) {
            this.value = parameter.value;
            this.overrideState = parameter.overrideState;
        }
        public void Fill(FloatParameter parameter) {
            parameter.value = this.value;
            parameter.overrideState = this.overrideState;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct IntValue {
        public int value;
        public bool overrideState;
        public IntValue(int value, bool overrideState) {
            this.value = value;
            this.overrideState = overrideState;
        }
        public IntValue(IntParameter parameter) {
            this.value = parameter.value;
            this.overrideState = parameter.overrideState;
        }
        public void Fill(IntParameter parameter) {
            parameter.value = this.value;
            parameter.overrideState = this.overrideState;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct BoolValue {
        public bool value;
        public bool overrideState;
        public BoolValue(bool value, bool overrideState) {
            this.value = value;
            this.overrideState = overrideState;
        }
        public BoolValue(BoolParameter parameter) {
            this.value = parameter.value;
            this.overrideState = parameter.overrideState;
        }
        public void Fill(BoolParameter parameter) {
            parameter.value = this.value;
            parameter.overrideState = this.overrideState;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct Vector2Value {
        public float[] value;
        public bool overrideState;
        public Vector2Value(float[] value, bool overrideState) {
            this.value = value;
            this.overrideState = overrideState;
        }
        public Vector2Value(Vector2Parameter parameter) {
            this.value = new float[2] { parameter.value[0], parameter.value[1] };
            this.overrideState = parameter.overrideState;
        }
        public void Fill(Vector2Parameter parameter) {
            parameter.value = new Vector2(this.value[0], this.value[1]);
            parameter.overrideState = this.overrideState;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct Vector3Value {
        public float[] value;
        public bool overrideState;
        public Vector3Value(float[] value, bool overrideState) {
            this.value = value;
            this.overrideState = overrideState;
        }
        public Vector3Value(Vector3Parameter parameter) {
            this.value = new float[3] { parameter.value[0], parameter.value[1], parameter.value[2] };
            this.overrideState = parameter.overrideState;
        }
        public void Fill(Vector3Parameter parameter) {
            parameter.value = new Vector3(this.value[0], this.value[1], this.value[2]);
            parameter.overrideState = this.overrideState;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct Vector4Value {
        public float[] value;
        public bool overrideState;
        public Vector4Value(float[] value, bool overrideState) {
            this.value = value;
            this.overrideState = overrideState;
        }
        public Vector4Value(Vector4Parameter parameter) {
            this.value = new float[4] { parameter.value[0], parameter.value[1], parameter.value[2], parameter.value[3] };
            this.overrideState = parameter.overrideState;
        }
        public void Fill(Vector4Parameter parameter) {
            parameter.value = new Vector4(this.value[0], this.value[1], this.value[2], this.value[3]);
            parameter.overrideState = this.overrideState;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct ColorValue {
        public float[] value;
        public bool overrideState;
        public ColorValue(float[] value, bool overrideState) {
            this.value = value;
            this.overrideState = overrideState;
        }
        public ColorValue(ColorParameter parameter) {
            this.value = new float[3] { parameter.value[0], parameter.value[1], parameter.value[2] };
            this.overrideState = parameter.overrideState;
        }
        public void Fill(ColorParameter parameter) {
            parameter.value = new Color(this.value[0], this.value[1], this.value[2]);
            parameter.overrideState = this.overrideState;
        }
    }

    // Unique Parameters for Post Processing


    [MessagePackObject(keyAsPropertyName: true)]
    public struct EyeAdaptationValue {
        public EyeAdaptation value;
        public bool overrideState;
        public EyeAdaptationValue(EyeAdaptation value, bool overrideState) {
            this.value = value;
            this.overrideState = overrideState;
        }
        public EyeAdaptationValue(EyeAdaptationParameter parameter) {
            this.value = parameter.value;
            this.overrideState = parameter.overrideState;
        }
        public void Fill(EyeAdaptationParameter parameter) {
            parameter.value = this.value;
            parameter.overrideState = this.overrideState;
        }
    }
    [MessagePackObject(keyAsPropertyName: true)]
    public struct AmbientOcclusionModeValue {
        public AmbientOcclusionMode value;
        public bool overrideState;
        public AmbientOcclusionModeValue(AmbientOcclusionMode value, bool overrideState) {
            this.value = value;
            this.overrideState = overrideState;
        }
        public AmbientOcclusionModeValue(AmbientOcclusionModeParameter parameter) {
            this.value = parameter.value;
            this.overrideState = parameter.overrideState;
        }
        public void Fill(AmbientOcclusionModeParameter parameter) {
            parameter.value = this.value;
            parameter.overrideState = this.overrideState;
        }
    }
    [MessagePackObject(keyAsPropertyName: true)]
    public struct AmbientOcclusionQualityValue {
        public AmbientOcclusionQuality value;
        public bool overrideState;
        public AmbientOcclusionQualityValue(AmbientOcclusionQuality value, bool overrideState) {
            this.value = value;
            this.overrideState = overrideState;
        }
        public AmbientOcclusionQualityValue(AmbientOcclusionQualityParameter parameter) {
            this.value = parameter.value;
            this.overrideState = parameter.overrideState;
        }
        public void Fill(AmbientOcclusionQualityParameter parameter) {
            parameter.value = this.value;
            parameter.overrideState = this.overrideState;
        }
    }
    [MessagePackObject(keyAsPropertyName: true)]
    public struct GradingModeValue {
        public GradingMode value;
        public bool overrideState;
        public GradingModeValue(GradingMode value, bool overrideState) {
            this.value = value;
            this.overrideState = overrideState;
        }
        public GradingModeValue(GradingModeParameter parameter) {
            this.value = parameter.value;
            this.overrideState = parameter.overrideState;
        }
        public void Fill(GradingModeParameter parameter) {
            parameter.value = this.value;
            parameter.overrideState = this.overrideState;
        }
    }
    [MessagePackObject(keyAsPropertyName: true)]
    public struct TonemapperValue {
        public Tonemapper value;
        public bool overrideState;
        public TonemapperValue(Tonemapper value, bool overrideState) {
            this.value = value;
            this.overrideState = overrideState;
        }
        public TonemapperValue(TonemapperParameter parameter) {
            this.value = parameter.value;
            this.overrideState = parameter.overrideState;
        }
        public void Fill(TonemapperParameter parameter) {
            parameter.value = this.value;
            parameter.overrideState = this.overrideState;
        }
    }
    [MessagePackObject(keyAsPropertyName: true)]
    public struct KernelSizeValue {
        public KernelSize value;
        public bool overrideState;
        public KernelSizeValue(KernelSize value, bool overrideState) {
            this.value = value;
            this.overrideState = overrideState;
        }
        public KernelSizeValue(KernelSizeParameter parameter) {
            this.value = parameter.value;
            this.overrideState = parameter.overrideState;
        }
        public void Fill(KernelSizeParameter parameter) {
            parameter.value = this.value;
            parameter.overrideState = this.overrideState;
        }
    }
    [MessagePackObject(keyAsPropertyName: true)]
    public struct ScreenSpaceReflectionPresetValue {
        public ScreenSpaceReflectionPreset value;
        public bool overrideState;
        public ScreenSpaceReflectionPresetValue(ScreenSpaceReflectionPreset value, bool overrideState) {
            this.value = value;
            this.overrideState = overrideState;
        }
        public ScreenSpaceReflectionPresetValue(ScreenSpaceReflectionPresetParameter parameter) {
            this.value = parameter.value;
            this.overrideState = parameter.overrideState;
        }
        public void Fill(ScreenSpaceReflectionPresetParameter parameter) {
            parameter.value = this.value;
            parameter.overrideState = this.overrideState;
        }
    }
    [MessagePackObject(keyAsPropertyName: true)]
    public struct ScreenSpaceReflectionResolutionValue {
        public ScreenSpaceReflectionResolution value;
        public bool overrideState;
        public ScreenSpaceReflectionResolutionValue(ScreenSpaceReflectionResolution value, bool overrideState) {
            this.value = value;
            this.overrideState = overrideState;
        }
        public ScreenSpaceReflectionResolutionValue(ScreenSpaceReflectionResolutionParameter parameter) {
            this.value = parameter.value;
            this.overrideState = parameter.overrideState;
        }
        public void Fill(ScreenSpaceReflectionResolutionParameter parameter) {
            parameter.value = this.value;
            parameter.overrideState = this.overrideState;
        }
    }
    [MessagePackObject(keyAsPropertyName: true)]
    public struct VignetteModeValue {
        public VignetteMode value;
        public bool overrideState;
        public VignetteModeValue(VignetteMode value, bool overrideState) {
            this.value = value;
            this.overrideState = overrideState;
        }
        public VignetteModeValue(VignetteModeParameter parameter) {
            this.value = parameter.value;
            this.overrideState = parameter.overrideState;
        }
        public void Fill(VignetteModeParameter parameter) {
            parameter.value = this.value;
            parameter.overrideState = this.overrideState;
        }
    }

}
