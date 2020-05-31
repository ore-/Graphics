using Studio;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Graphics
{
    internal class LightManager
    {
        private List<LightObject> allLights;
        internal List<LightObject> DirectionalLights { get; private set; }
        internal List<LightObject> PointLights { get; private set; }
        internal List<LightObject> SpotLights { get; private set; }
        internal bool UseAlloyLight { get; set; }
        internal LightObject SelectedLight { get; set; }

        private Graphics _parent;
        internal LightManager(Graphics parent)
        {
            _parent = parent;
            DirectionalLights = new List<LightObject>();
            PointLights = new List<LightObject>();
            SpotLights = new List<LightObject>();
            UseAlloyLight = true;
        }

        internal void Light()
        {
            allLights = LightObject.GetLights();
            DirectionalLights.Clear();
            PointLights.Clear();
            SpotLights.Clear();
            for (int i = 0; i < allLights.Count; i++)
            {
                if (allLights[i].type == LightType.Spot)
                {
                    //if (l.cookie == null)
                    //    l.cookie = DefaultSpotCookie;
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
                    allLights[i].light.GetOrAddComponent<AlloyAreaLight>().UpdateBinding();
                }
            }
        }

        internal class LightObject
        {
            private Light _light;
            private OCILight _ociLight;

            internal LightObject(Light light)
            {
                _light = light;
            }

            internal LightObject(OCILight ociLight)
            {
                _ociLight = ociLight;
            }

            internal static List<LightObject> GetLights()
            {
                if (Graphics.Instance.IsStudio())
                {
                    List<OCILight> ociLights = Singleton<Studio.Studio>.Instance.dicObjectCtrl.Values.OfType<OCILight>().ToList();
                    List<LightObject> lightObjects = ociLights.Select(light => new LightObject(light)).ToList();
                    //filter for non studio lights, including light from custom maps
                    //all lights
                    List<Light> lights = GameObject.FindObjectsOfType<Light>().ToList();
                    //all studio lights
                    List<Light> lightsFromOCI = ociLights.Select(light => light.light).ToList();
                    //all light except studio lights
                    lights = lights.Where(light => !lightsFromOCI.Contains(light)).ToList();
                    //instantiate these lights
                    List<LightObject> additionalLightObjects = lights.Select(light => new LightObject(light)).ToList();
                    //full list of studio and "non-studio" lights
                    return lightObjects.Concat(additionalLightObjects).ToList();
                }
                else
                {
                    List<Light> lights = GameObject.FindObjectsOfType<Light>().ToList();
                    List<LightObject> lightObjects = lights.Select(light => new LightObject(light)).ToList();
                    return lightObjects;
                }
            }

            private bool IsStudioLight { get => Graphics.Instance.IsStudio() && null == _light && null != _ociLight; }

            internal Light light
            {
                get
                {
                    if (IsStudioLight)
                    {
                        try
                        {
                            return _ociLight?.light;
                        }
                        catch
                        {
                            return null;
                        }
                    }
                    else
                        return _light;
                }
            }

            internal LightType type
            {
                get => light.type;
            }

            internal bool enabled
            {
                get => null == light ? false : light.enabled;
                set
                {
                    if (value == enabled) return;

                    if (IsStudioLight)
                    {
                        _ociLight.SetEnable(value);
                        UnityEngine.UI.Toggle toggle = LightControl()?.GetFieldValue<UnityEngine.UI.Toggle>("toggleVisible");
                        if (null != toggle)
                        {
                            toggle.isOn = value;
                            LightControl()?.SetFieldValue<UnityEngine.UI.Toggle>("toggleVisible", toggle);
                        }
                    }
                    else
                        _light.enabled = value;
                }
            }

            internal Color color
            {
                get => light.color;
                set
                {
                    if (IsStudioLight)
                    {
                        _ociLight.SetColor(value);
                        LightControl()?.CallMethod("OnValueChangeColor", new object[] { value });
                    }
                    else
                        _light.color = value;
                }
            }
            internal float intensity
            {
                get => light.intensity;
                set
                {
                    if (IsStudioLight)
                    {
                        _ociLight.SetIntensity(value);
                        LightControl()?.CallMethod("OnEndEditIntensity", new object[] { value.ToString() });
                    }
                    else
                        _light.intensity = value;
                }
            }

            internal LightShadows shadows
            {
                get => light.shadows;
                set
                {
                    if (value == shadows) return;

                    if (IsStudioLight)
                    {
                        _ociLight.SetShadow(LightShadows.Soft == value);
                        UnityEngine.UI.Toggle toggle = LightControl()?.GetFieldValue<UnityEngine.UI.Toggle>("toggleShadow");
                        if (null != toggle)
                        {
                            toggle.isOn = LightShadows.Soft == value;
                            LightControl()?.SetFieldValue<UnityEngine.UI.Toggle>("toggleShadow", toggle);
                        }
                    }
                    else
                        _light.shadows = value;
                }
            }

            internal float range
            {
                get => light.range;
                set
                {
                    if (IsStudioLight)
                    {
                        _ociLight.SetRange(value);
                        LightControl()?.CallMethod("OnEndEditRange", new object[] { value.ToString() });
                    }
                    else
                        _light.range = value;
                }
            }

            internal float spotAngle
            {
                get => light.spotAngle;
                set
                {
                    if (IsStudioLight)
                    {
                        _ociLight.SetSpotAngle(value);
                        LightControl()?.CallMethod("OnEndEditSpotAngle", new object[] { value.ToString() });
                    }
                    else
                        _light.spotAngle = value;
                }
            }

            internal Vector3 rotation
            {
                get
                {
                    if (IsStudioLight)
                    {
                        return _ociLight.guideObject.changeAmount.rot;
                    }
                    else
                    {
                        Vector3 rot = _light.transform.eulerAngles;
                        rot.x = Mathf.DeltaAngle(0f, rot.x);
                        if (rot.x > 180f)
                        {
                            rot.x -= 360f;
                        }
                        rot.y = Mathf.DeltaAngle(0f, rot.y);
                        if (rot.y > 180f)
                        {
                            rot.y -= 360f;
                        }
                        return rot;
                    }
                }
                set
                {
                    if (IsStudioLight)
                    {
                        List<GuideCommand.EqualsInfo> list = new List<GuideCommand.EqualsInfo>();
                        Vector3 rot2 = _ociLight.guideObject.changeAmount.rot;
                        _ociLight.guideObject.changeAmount.rot = value;
                        list.Add(new GuideCommand.EqualsInfo
                        {
                            dicKey = _ociLight.guideObject.dicKey,
                            oldValue = rot2,
                            newValue = value
                        });
                    }
                    else
                    {
                        _light.transform.eulerAngles = value;
                    }
                }
            }

            private Studio.MPLightCtrl LightControl()
            {
                Studio.MPLightCtrl[] controls = GameObject.FindObjectsOfType<Studio.MPLightCtrl>();
                return 1 != controls.Length || !ReferenceEquals(controls[0].ociLight, _ociLight) ? null : controls[0];
            }
        }
    }
}