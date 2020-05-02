using AIGraphics.Textures;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGraphics.Settings
{
    public class SkyboxSettings {
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
        readonly static public string shaderName = "SkyBox/Simple Two Colors";
    }
}
