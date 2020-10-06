using System;
using System.Collections.Generic;
using System.Text;
using KKAPI;

namespace Graphics
{
    public enum PresetDefaultType
    {
        MAIN_GAME,
        MAKER,
        VR_GAME,
        STUDIO
    }

    static class PresetDefaultTypeUtils
    {
        internal static PresetDefaultType ForGameMode(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.MainGame:
                    return PresetDefaultType.MAIN_GAME;
                case GameMode.Maker:
                    return PresetDefaultType.MAKER;
                case GameMode.Studio:
                    return PresetDefaultType.STUDIO;
                case GameMode.Unknown:   // Until KKAPI updates this is needed -- the VR preset is also a decent fallback - likely to be a more conservative default anyway
                    return PresetDefaultType.VR_GAME;
                default:
                    return PresetDefaultType.VR_GAME;
            }
        }
    }
}
