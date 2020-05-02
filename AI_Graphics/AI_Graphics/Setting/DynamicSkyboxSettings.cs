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
    public abstract class SkyboxSettings {
        readonly static public string shaderName;

        virtual public void Save() { }
        virtual public void Load() { }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class ProceduralSkyboxSettings : SkyboxSettings
    {
        readonly static public string shaderName = "Skybox/Procedural";

        internal float sunSize;
        internal float sunsizeConvergence;
        internal float atmosphereThickness;
        internal float[] skyTint;
        internal float[] groundTint;
        internal float exposure;

        override public void Save()
        {
        }
        override public void Load()
        {
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class TwoPointColorSkyboxSettings : SkyboxSettings
    {
        internal float intensityA;
        internal float intensityB;
        internal float[] colorA;
        internal float[] colorB;
        internal float[] directionA;
        internal float[] directionB;

        readonly static public string shaderName = "SkyBox/Simple Two Colors";
        override public void Save()
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
        override public void Load()
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
}
