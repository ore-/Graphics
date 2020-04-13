using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AIGraphics
{
    internal class LightManager
    {
        private Light[] allLights;
        internal List<Light> DirectionalLights { get; private set; }
        internal List<Light> PointLights { get; private set; }
        internal List<Light> SpotLights { get; private set; }
        internal bool UseAlloyLight { get; set; }
        private AIGraphics _parent;
        internal LightManager(AIGraphics parent)
        {
            _parent = parent;
            DirectionalLights = new List<Light>();
            PointLights = new List<Light>();
            SpotLights = new List<Light>();
            UseAlloyLight = true;
        }

        internal void Light()
        {
            allLights = GameObject.FindObjectsOfType<Light>();
            DirectionalLights.Clear();
            PointLights.Clear();
            SpotLights.Clear();
            for (int i = 0; i < allLights.Length; i++)
            {
                if (allLights[i].type == LightType.Spot)
                {
                    /*
                    if (l.cookie == null)
                        l.cookie = DefaultSpotCookie;
                    */
                    SpotLights.Add(allLights[i]);
                }
                else if (allLights[i].type == LightType.Point)
                {
                    PointLights.Add(allLights[i]);
                }
                else if (allLights[i].type == LightType.Directional)
                {
                    DirectionalLights.Add(allLights[i]);
                }
                if (UseAlloyLight)
                {
                    allLights[i].GetOrAddComponent<AlloyAreaLight>().UpdateBinding();
                }
            }
        }
    }
}
