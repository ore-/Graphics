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
        public static Color FloatArrayToColor(float[] array)
        {
            return array != null ? new Color(array[0], array[1], array[2]) : Color.white;
        }
        public static Vector3 FloatArrayToVector(float[] array)
        {
            return array != null ? new Vector3(array[0], array[1], array[2]) : Vector3.zero;
        }
        virtual public void Save() { }
        virtual public void Load() { }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class ProceduralSkyboxSettings : SkyboxSettings
    {
        [IgnoreMember]
        public static readonly string shaderName = "Skybox/Procedural";

        public float sunSize;
        public float sunsizeConvergence;
        public float atmosphereThickness;
        public float[] skyTint = new float[3];
        public float[] groundTint = new float[3];

        public override void Save()
        {
            Material mat = AIGraphics.Instance?.SkyboxManager?.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                sunSize = mat.GetFloat("_SunSize");
                sunsizeConvergence = mat.GetFloat("_SunSizeConvergence");
                atmosphereThickness = mat.GetFloat("_AtmosphereThickness");
                for (int i = 0; i < 3; i++)
                {
                    skyTint[i] = mat.GetColor("_SkyTint")[i];
                    groundTint[i] = mat.GetColor("_GroundColor")[i];
                }
            }
        }
        public override void Load()
        {
            Material mat = AIGraphics.Instance?.SkyboxManager?.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                mat.SetFloat("_SunSize", sunSize);
                mat.SetFloat("_SunSizeConvergence", sunsizeConvergence);
                mat.SetFloat("_AtmosphereThickness", atmosphereThickness);
                mat.SetColor("_SkyTint", FloatArrayToColor(skyTint));
                mat.SetColor("_GroundColor", FloatArrayToColor(groundTint));
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class TwoPointColorSkyboxSettings : SkyboxSettings
    {
        public float intensityA;
        public float intensityB;
        public float[] colorA = new float[3];
        public float[] colorB = new float[3];
        public float[] directionA = new float[3];
        public float[] directionB = new float[3];

        [IgnoreMember]
        public static readonly string shaderName = "SkyBox/Simple Two Colors";
        public override void Save()
        {
            Material mat = AIGraphics.Instance?.SkyboxManager?.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                intensityA = mat.GetFloat("_IntensityA");
                intensityB = mat.GetFloat("_IntensityB");
                for (int i = 0; i < 3; i++)
                {
                    colorA[i] = mat.GetColor("_ColorA")[i];
                    colorB[i] = mat.GetColor("_ColorB")[i];
                    directionA[i] = mat.GetVector("_DirA")[i];
                    directionB[i] = mat.GetVector("_DirB")[i];
                }
            }
        }
        public override void Load()
        {
            Material mat = AIGraphics.Instance?.SkyboxManager?.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                mat.SetFloat("_IntensityA", intensityA);
                mat.SetFloat("_IntensityB", intensityB);
                mat.SetColor("_ColorA", FloatArrayToColor(colorA));
                mat.SetColor("_ColorB", FloatArrayToColor(colorB));
                mat.SetVector("_DirA", FloatArrayToVector(directionA));
                mat.SetVector("_DirB", FloatArrayToVector(directionB));
            }
        }
    }
    public class FourPointGradientSkyboxSetting : SkyboxSettings
    {
        public float[] colorA = new float[3];
        public float[] colorB = new float[3];
        public float[] colorC = new float[3];
        public float[] colorD = new float[3];
        public float[] directionA = new float[3];
        public float[] directionB = new float[3];
        public float[] directionC = new float[3];
        public float[] directionD = new float[3];
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
                for (int i = 0; i < 3; i++)
                {
                    colorA[i] = mat.GetColor("_Color1")[i];
                    colorB[i] = mat.GetColor("_Color2")[i];
                    colorC[i] = mat.GetColor("_Color3")[i];
                    colorD[i] = mat.GetColor("_Color4")[i];
                    directionA[i] = mat.GetVector("_Direction1")[i];
                    directionB[i] = mat.GetVector("_Direction2")[i];
                    directionC[i] = mat.GetVector("_Direction3")[i];
                    directionD[i] = mat.GetVector("_Direction4")[i];
                }
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
                mat.SetColor("_Color1", FloatArrayToColor(colorA));
                mat.SetColor("_Color2", FloatArrayToColor(colorB));
                mat.SetColor("_Color3", FloatArrayToColor(colorC));
                mat.SetColor("_Color4", FloatArrayToColor(colorD));
                mat.SetVector("_Direction1", FloatArrayToVector(directionA));
                mat.SetVector("_Direction2", FloatArrayToVector(directionB));
                mat.SetVector("_Direction3", FloatArrayToVector(directionC));
                mat.SetVector("_Direction4", FloatArrayToVector(directionD));
                mat.SetFloat("_Exponent1", exponentA);
                mat.SetFloat("_Exponent2", exponentB);
                mat.SetFloat("_Exponent3", exponentC);
                mat.SetFloat("_Exponent4", exponentD);
            }
        }
    }

    public class HemisphereGradientSkyboxSetting : SkyboxSettings
    {
        public float[] colorA = new float[3];
        public float[] colorB = new float[3];
        public float[] colorC = new float[3];

        [IgnoreMember]
        public static readonly string shaderName = "SkyboxPlus/Hemisphere";

        public override void Save()
        {
            Material mat = AIGraphics.Instance?.SkyboxManager?.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                for (int i = 0; i < 3; i++)
                {
                    colorA[i] = mat.GetColor("_TopColor")[i];
                    colorB[i] = mat.GetColor("_MiddleColor")[i];
                    colorC[i] = mat.GetColor("_BottomColor")[i];
                }
            }
        }
        public override void Load()
        {
            Material mat = AIGraphics.Instance?.SkyboxManager?.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                mat.SetColor("_TopColor", FloatArrayToColor(colorA));
                mat.SetColor("_MiddleColor", FloatArrayToColor(colorB));
                mat.SetColor("_BottomColor", FloatArrayToColor(colorC));
            }
        }
    }

}
