using System.Collections.Generic;
using UnityEngine;

namespace Graphics
{
    //https://www.gamasutra.com/blogs/CharlesCordingley/20141124/230710/Unity3D_Culling_Mask_Tip.php
    internal static class CullingMaskExtensions
    {
        private static List<string> _layers;
        internal static List<string> Layers { 
            get
            { 
                if (_layers == null)
                {
                    List<string> layers = new List<string>();
                    for (int i = 0; i <= 31; i++)
                    {
                        var layerN = LayerMask.LayerToName(i);
                        if (layerN.Length > 0)
                            layers.Add(layerN);
                    }
                    _layers = layers;
                }
                return _layers;
            }
        }

        internal static int LayerCullingShow(int cullingMask, int layerMask)
        {
            cullingMask |= layerMask;
            return cullingMask;
        }

        internal static int LayerCullingShow(int cullingMask, string layer)
        {
            return LayerCullingShow(cullingMask, 1 << LayerMask.NameToLayer(layer));
        }

        internal static int LayerCullingHide(int cullingMask, int layerMask)
        {
            cullingMask &= ~layerMask;
            return cullingMask;
        }

        internal static int LayerCullingHide(int cullingMask, string layer)
        {
            return LayerCullingHide(cullingMask, 1 << LayerMask.NameToLayer(layer));
        }

        internal static int LayerCullingToggle(int cullingMask, int layerMask)
        {
            cullingMask ^= layerMask;
            return cullingMask;
        }

        internal static int LayerCullingToggle(int cullingMask, string layer)
        {
            return LayerCullingToggle(cullingMask, 1 << LayerMask.NameToLayer(layer));
        }

        internal static bool LayerCullingIncludes(int cullingMask, int layerMask)
        {
            return (cullingMask & layerMask) > 0;
        }

        internal static bool LayerCullingIncludes(int cullingMask, string layer)
        {
            return LayerCullingIncludes(cullingMask, 1 << LayerMask.NameToLayer(layer));
        }

        internal static int LayerCullingToggle(int cullingMask, int layerMask, bool isOn)
        {
            bool included = LayerCullingIncludes(cullingMask, layerMask);
            if (isOn && !included)
            {
                return LayerCullingShow(cullingMask, layerMask);
            }
            else if (!isOn && included)
            {
                return LayerCullingHide(cullingMask, layerMask);
            }
            return cullingMask;
        }

        internal static int LayerCullingToggle(int cullingMask, string layer, bool isOn)
        {
            return LayerCullingToggle(cullingMask, 1 << LayerMask.NameToLayer(layer), isOn);
        }
    }
}
