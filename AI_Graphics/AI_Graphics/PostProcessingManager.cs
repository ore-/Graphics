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
                "Lut_Aliens", "Lut_Art", "Lut_Bright", "Lut_Cold", "Lut_Comic", "Lut_DarkGreyish", "Lut_Deep",
                "Lut_Default", "Lut_Desert", "Lut_Dull", "Lut_Fog", "Lut_Greyish", "Lut_InvertMono", "Lut_Invert",
                "Lut_LimitBlue", "Lut_LimitGreen", "Lut_LimitRed", "Lut_MonoBlue", "Lut_Monochrome", "Lut_MonoGreen",
                "Lut_MonoRed", "Lut_OldPoster1", "Lut_OldPoster2", "Lut_Pale", "Lut_Posterize", "Lut_RobotSalvation",
                "Lut_Sarmo", "Lut_Sepia", "Lut_Soft", "Lut_Strong", "Lut_TimeDay", "Lut_TimeNight", "Lut_TimeSundown",
                "Lut_Vibrance_D", "Lut_Warm"
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