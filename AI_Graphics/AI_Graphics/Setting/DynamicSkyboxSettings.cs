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
        readonly static public string shaderName = "Skybox/Procedural";

        public float sunSize;
        public float sunsizeConvergence;
        public float atmosphereThickness;
        public float[] skyTint;
        public float[] groundTint;

        public override void Save()
        {
            Material mat = AIGraphics.Instance.SkyboxManager.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                sunSize = mat.GetFloat("_SunSize");
                sunsizeConvergence = mat.GetFloat("_SunSizeConvergence");
                atmosphereThickness = mat.GetFloat("_AtmosphereThickness");
                Color matColorA = mat.GetColor("_SkyTint");
                skyTint = new float[3] { matColorA.r, matColorA.g, matColorA.b };
                Color matColorB = mat.GetColor("_GroundColor");
                groundTint = new float[3] { matColorB.r, matColorB.g, matColorB.b };
            }
        }
        public override void Load()
        {
            Material mat = AIGraphics.Instance.SkyboxManager.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                mat.SetFloat("_SunSize", sunSize);
                mat.SetFloat("_SunSizeConvergence", sunsizeConvergence);
                mat.SetFloat("_AtmosphereThickness", atmosphereThickness);
                mat.SetColor("_SkyTint", new Color(skyTint[0], skyTint[1], skyTint[2]));
                mat.SetColor("_Col_GroundColororB", new Color(groundTint[0], groundTint[1], groundTint[2]));
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class TwoPointColorSkyboxSettings : SkyboxSettings
    {
        public float intensityA;
        public float intensityB;
        public float[] colorA;
        public float[] colorB;
        public float[] directionA;
        public float[] directionB;

        [IgnoreMember]
        readonly static public string shaderName = "SkyBox/Simple Two Colors";
        public override void Save()
        {
            Material mat = AIGraphics.Instance.SkyboxManager.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                intensityA = mat.GetFloat("_IntensityA");
                intensityB = mat.GetFloat("_IntensityB");
                Color matColorA = mat.GetColor("_ColorA");
                colorA = new float[3] { matColorA.r, matColorA.g, matColorA.b };
                Color matColorB = mat.GetColor("_ColorB");
                colorB = new float[3] { matColorB.r, matColorB.g, matColorB.b };
                Vector3 matDirectionA = mat.GetVector("_DirA");
                directionA = new float[3] { matDirectionA.x, matDirectionA.y, matDirectionA.z };
                Vector3 matDirectionB = mat.GetVector("_DirB");
                directionB = new float[3] { matDirectionB.x, matDirectionB.y, matDirectionB.z };
            }
        }
        public override void Load()
        {
            Material mat = AIGraphics.Instance.SkyboxManager.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                mat.SetFloat("_IntensityA", intensityA);
                mat.SetFloat("_IntensityB", intensityB);
                mat.SetColor("_ColorA", new Color(colorA[0], colorA[1], colorA[2]));
                mat.SetColor("_ColorB", new Color(colorB[0], colorB[1], colorB[2]));
                mat.SetVector("_DirA", new Vector3(directionA[0], directionA[1], directionA[2]));
                mat.SetVector("_DirB", new Vector3(directionB[0], directionB[1], directionB[2]));
            }
        }
    }
    public class FourPointGradientSkyboxSetting : SkyboxSettings
    {
        public float[] colorA;
        public float[] colorB;
        public float[] colorC;
        public float[] colorD;
        public float[] directionA;
        public float[] directionB;
        public float[] directionC;
        public float[] directionD;
        public float exponentA;
        public float exponentB;
        public float exponentC;
        public float exponentD;


        [IgnoreMember]
        readonly static public string shaderName = "SkyboxPlus/Gradients";
        public override void Save()
        {
            Material mat = AIGraphics.Instance.SkyboxManager.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                Color matColorA = mat.GetColor("_Color1");
                colorA = new float[3] { matColorA.r, matColorA.g, matColorA.b };
                Color matColorB = mat.GetColor("_Color2");
                colorB = new float[3] { matColorB.r, matColorB.g, matColorB.b };
                Color matColorC = mat.GetColor("_Color3");
                colorC = new float[3] { matColorC.r, matColorC.g, matColorC.b };
                Color matColorD = mat.GetColor("_Color4");
                colorC = new float[3] { matColorC.r, matColorC.g, matColorC.b };
                Vector3 matDirectionA = mat.GetVector("_Direction1");
                directionA = new float[3] { matDirectionA.x, matDirectionA.y, matDirectionA.z };
                Vector3 matDirectionB = mat.GetVector("_Direction2");
                directionA = new float[3] { matDirectionB.x, matDirectionB.y, matDirectionB.z };
                Vector3 matDirectionC = mat.GetVector("_Direction3");
                directionA = new float[3] { matDirectionC.x, matDirectionC.y, matDirectionC.z };
                Vector3 matDirectionD = mat.GetVector("_Direction4");
                directionA = new float[3] { matDirectionD.x, matDirectionD.y, matDirectionD.z };
                exponentA = mat.GetFloat("_Exponent1");
                exponentB = mat.GetFloat("_Exponent2");
                exponentC = mat.GetFloat("_Exponent3");
                exponentD = mat.GetFloat("_Exponent4");
            }
        }
        public override void Load()
        {
            Material mat = AIGraphics.Instance.SkyboxManager.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                mat.SetColor("_Color1", new Color(colorA[0], colorA[1], colorA[2]));
                mat.SetColor("_Color2", new Color(colorB[0], colorB[1], colorB[2]));
                mat.SetColor("_Color3", new Color(colorC[0], colorC[1], colorC[2]));
                mat.SetColor("_Color4", new Color(colorD[0], colorD[1], colorD[2]));
                mat.SetVector("_Direction1", new Vector3(directionA[0], directionA[1], directionA[2]));
                mat.SetVector("_Direction2", new Vector3(directionB[0], directionB[1], directionB[2]));
                mat.SetVector("_Direction3", new Vector3(directionC[0], directionC[1], directionC[2]));
                mat.SetVector("_Direction4", new Vector3(directionD[0], directionD[1], directionD[2]));
                mat.SetFloat("_Exponent1", exponentA);
                mat.SetFloat("_Exponent2", exponentB);
                mat.SetFloat("_Exponent3", exponentC);
                mat.SetFloat("_Exponent4", exponentD);
            }
        }
    }

    public class HemisphereGradientSkyboxSetting : SkyboxSettings
    {
        public float[] colorA;
        public float[] colorB;
        public float[] colorC;

        [IgnoreMember]
        readonly static public string shaderName = "SkyboxPlus/Hemisphere";

        public override void Save()
        {
            Material mat = AIGraphics.Instance.SkyboxManager.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                Color matColorA = mat.GetColor("_TopColor");
                colorA = new float[3] { matColorA.r, matColorA.g, matColorA.b };
                Color matColorB = mat.GetColor("_MiddleColor");
                colorB = new float[3] { matColorB.r, matColorB.g, matColorB.b };
                Color matColorC = mat.GetColor("_BottomColor");
                colorC = new float[3] { matColorC.r, matColorC.g, matColorC.b };
            }
        }
        public override void Load()
        {
            Material mat = AIGraphics.Instance.SkyboxManager.Skybox;
            if (mat != null && mat.shader.name == shaderName)
            {
                mat.SetColor("_TopColor", new Color(colorA[0], colorA[1], colorA[2]));
                mat.SetColor("_MiddleColor", new Color(colorB[0], colorB[1], colorB[2]));
                mat.SetColor("_BottomColor", new Color(colorC[0], colorC[1], colorC[2]));
            }
        }
    }

}
