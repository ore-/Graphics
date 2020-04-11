using System.Collections.Generic;
using UnityEngine;

namespace AIGraphics
{

    internal class PostProcessingManager : MonoBehaviour
    {
        internal static Setting.TextureSetting lensDirtTexture = new Setting.TextureSetting();
        // hard coding ftw
        internal static Setting.InternalTextureSetting lutTexture = new Setting.InternalTextureSetting(
            "studio/lut/00.unity3d",
            new List<string>() {
                "Lut_Default", "Lut_TimeDay", "Lut_TimeSundown", "Lut_TimeNight", "Lut_Warm", "Lut_Cold", 
                "Lut_Dull", "Lut_Pale", "Lut_Soft", "Lut_Strong", "Lut_Deep", "Lut_Bright", "Lut_Sepia", "Lut_Monochrome", 
                "Lut_MonoRed", "Lut_MonoBlue", "Lut_MonoGreen", "Lut_LimitRed", "Lut_LimitBlue", "Lut_LimitGreen", 
                "Lut_InvertMono", "Lut_Invert", "Lut_Sarmo", "Lut_Posterize", 
                "Lut_OldPoster1", "Lut_OldPoster2", "Lut_RobotSalvation", "Lut_Greyish", "Lut_DarkGreyish", 
                "Lut_Art", "Lut_Comic", "Lut_Aliens", "Lut_Fog", "Lut_Desert", "Lut_Vibrance_D"
            }
        );

        internal string LensDirtTexturesPath
        {
            get => lensDirtTexture.lookupPath;
            set
            {
                lensDirtTexture.lookupPath = value;
                lensDirtTexture.LoadTextures(this);
            }
        }
    }
}