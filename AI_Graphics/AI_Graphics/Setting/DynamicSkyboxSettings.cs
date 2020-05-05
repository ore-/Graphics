using AIGraphics.Textures;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AIGraphics.Settings
{
    [Union(0, typeof(ProceduralSkyboxSettings))]
    [Union(1, typeof(TwoPointColorSkyboxSettings))]
    [Union(2, typeof(FourPointGradientSkyboxSetting))]
    [Union(3, typeof(HemisphereGradientSkyboxSetting))]
    public abstract class SkyboxSettings
    {
        virtual public void Save() { }
        virtual public void Load() { }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class ProceduralSkyboxSettings : SkyboxSettings
    {
        [IgnoreMember]
        public static readonly string shaderName = "Skybox/Procedural";

        public enum SunDisk
        {
            None,
            Simple,
            HighQuality,
        }

        public SunDisk sunDisk;
        public float sunSize;
        public float sunsizeConvergence;
        public float atmosphereThickness;
        public Color skyTint;
        public Color groundTint;
        public float exposure;

        public override void Save()
        {
            Material mat = AIGraphics.Instance?.SkyboxManager?.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                sunDisk = (ProceduralSkyboxSettings.SunDisk)mat.GetInt("_SunDisk");
                sunSize = mat.GetFloat("_SunSize");
                sunsizeConvergence = mat.GetFloat("_SunSizeConvergence");
                atmosphereThickness = mat.GetFloat("_AtmosphereThickness");
                skyTint = mat.GetColor("_SkyTint");
                groundTint = mat.GetColor("_GroundColor");
                exposure = mat.GetFloat("_Exposure");
            }
        }
        public override void Load()
        {
            Material mat = AIGraphics.Instance?.SkyboxManager?.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                mat.SetInt("_SunDisk", (int)sunDisk);
                mat.SetFloat("_SunSize", sunSize);
                mat.SetFloat("_SunSizeConvergence", sunsizeConvergence);
                mat.SetFloat("_AtmosphereThickness", atmosphereThickness);
                mat.SetColor("_SkyTint", skyTint);
                mat.SetColor("_GroundColor", groundTint);
                mat.SetFloat("_Exposure", exposure);
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class TwoPointColorSkyboxSettings : SkyboxSettings
    {
        public float intensityA;
        public float intensityB;
        public Color colorA = new Color();
        public Color colorB = new Color();
        public Vector4 directionA = new Vector4();
        public Vector4 directionB = new Vector4();

        [IgnoreMember]
        public static readonly string shaderName = "SkyBox/Simple Two Colors";
        public override void Save()
        {
            Material mat = AIGraphics.Instance?.SkyboxManager?.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                intensityA = mat.GetFloat("_IntensityA");
                intensityB = mat.GetFloat("_IntensityB");
                colorA = mat.GetColor("_ColorA");
                colorB = mat.GetColor("_ColorB");
                directionA = mat.GetVector("_DirA");
                directionB = mat.GetVector("_DirB");
            }
        }
        public override void Load()
        {
            Material mat = AIGraphics.Instance?.SkyboxManager?.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                mat.SetFloat("_IntensityA", intensityA);
                mat.SetFloat("_IntensityB", intensityB);
                mat.SetColor("_ColorA", colorA);
                mat.SetColor("_ColorB", colorB);
                mat.SetVector("_DirA", directionA);
                mat.SetVector("_DirB", directionB);
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class FourPointGradientSkyboxSetting : SkyboxSettings
    {
        public Color colorA = new Color();
        public Color colorB = new Color();
        public Color colorC = new Color();
        public Color colorD = new Color();
        public Vector3 directionA = new Vector3();
        public Vector3 directionB = new Vector3();
        public Vector3 directionC = new Vector3();
        public Vector3 directionD = new Vector3();
        public float exponentA;
        public float exponentB;
        public float exponentC;
        public float exponentD;


        [IgnoreMember]
        public static readonly string shaderName = "SkyboxPlus/Gradients";
        public override void Save()
        {
            Material mat = AIGraphics.Instance?.SkyboxManager?.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                colorA = mat.GetColor("_Color1");
                colorB = mat.GetColor("_Color2");
                colorC = mat.GetColor("_Color3");
                colorD = mat.GetColor("_Color4");
                directionA = mat.GetVector("_Direction1");
                directionB = mat.GetVector("_Direction2");
                directionC = mat.GetVector("_Direction3");
                directionD = mat.GetVector("_Direction4");
                exponentA = mat.GetFloat("_Exponent1");
                exponentB = mat.GetFloat("_Exponent2");
                exponentC = mat.GetFloat("_Exponent3");
                exponentD = mat.GetFloat("_Exponent4");
            }
        }
        public override void Load()
        {
            Material mat = AIGraphics.Instance?.SkyboxManager?.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                mat.SetColor("_Color1", colorA);
                mat.SetColor("_Color2", colorB);
                mat.SetColor("_Color3", colorC);
                mat.SetColor("_Color4", colorD);
                mat.SetVector("_Direction1", directionA);
                mat.SetVector("_Direction2", directionB);
                mat.SetVector("_Direction3", directionC);
                mat.SetVector("_Direction4", directionD);
                mat.SetFloat("_Exponent1", exponentA);
                mat.SetFloat("_Exponent2", exponentB);
                mat.SetFloat("_Exponent3", exponentC);
                mat.SetFloat("_Exponent4", exponentD);
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class HemisphereGradientSkyboxSetting : SkyboxSettings
    {
        public Color colorA = new Color();
        public Color colorB = new Color();
        public Color colorC = new Color();

        [IgnoreMember]
        public static readonly string shaderName = "SkyboxPlus/Hemisphere";

        public override void Save()
        {
            Material mat = AIGraphics.Instance?.SkyboxManager?.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                colorA = mat.GetColor("_TopColor");
                colorB = mat.GetColor("_MiddleColor");
                colorC = mat.GetColor("_BottomColor");
            }
        }
        public override void Load()
        {
            Material mat = AIGraphics.Instance?.SkyboxManager?.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                mat.SetColor("_TopColor", colorA);
                mat.SetColor("_MiddleColor", colorB);
                mat.SetColor("_BottomColor", colorC);
            }
        }
    }

}
