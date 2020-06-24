using UnityEngine;

// Utility scripts for the post processing stack
// https://github.com/keijiro/PostProcessingUtilities
namespace Graphics
{
    public class FocusPuller : MonoBehaviour
    {
        private float _velocity;
        private float _speed = 6f;
        private Vector3 lastDoFPoint;
        private bool initialFrame;
        private GameObject _ftgo;
        private Transform _target;
#if DEBUG
        private LineDrawer lineDrawer;
#endif

        internal LayerMask HitLayer { get; set; }
        internal float MaxDistance { get; set; }
        internal Vector3 TargetFocusPoint { get; private set; }
        internal Vector3 TargetPosition { get => null != _target ? _target.position : Vector3.zero; }
        internal Vector3 TransformPosition { get => null != _target ? transform.position : Vector3.zero; }
        internal Vector3 HitPoint { get; private set; }
        internal Vector3 RayOrigin { get; private set; }
        internal float Speed
        {
            get => _speed;
            set => _speed = Mathf.Max(MinSpeed, value);
        }
        internal static float MinSpeed = 1f;
        internal static float MaxSpeed = 12f;
        internal static string ColliderLayer = "H_MetaBallB";
        internal static string DefaultLayer = "Default";

        private void Awake()
        {
            _ftgo = new GameObject("FocusTarget");
            _target = _ftgo.transform;
            HitLayer = CullingMaskExtensions.LayerCullingShow(Graphics.Instance.CameraSettings.MainCamera.cullingMask, ColliderLayer);
            HitLayer = CullingMaskExtensions.LayerCullingHide(HitLayer, DefaultLayer);
            MaxDistance = Graphics.Instance.CameraSettings.MainCamera.farClipPlane;
#if DEBUG
            lineDrawer = new LineDrawer();
#endif
        }

        private void OnEnable()
        {
            initialFrame = true;
        }

        private void OnValidate()
        {
            Speed = _speed;
        }

        private void OnPreRender()
        {
            if (_target == null) return;

            if (initialFrame)
            {
                initialFrame = false;
                Focus(new Vector3(Screen.width / 2, Screen.height / 2));
            }
            else
            {
                Focus(Input.mousePosition);
            }

            // Retrieve the current value.
            float d1 = Graphics.Instance.PostProcessingSettings.FocalDistance;
#if DEBUG
            lineDrawer.DrawLineInGameView(transform.position, _target.position, Color.green);
#endif
            // Calculate the depth of the focus point.
            float d2 = Vector3.Dot(_target.position - transform.position, transform.forward);
            if (d2 < 0.1f)
            {
                d2 = 0.1f;
            }

            // Damped-spring interpolation.
            float dt = Time.deltaTime;
            float n1 = _velocity - (d1 - d2) * Speed * Speed * dt;
            float n2 = 1 + Speed * dt;
            _velocity = n1 / (n2 * n2);
            float d = d1 + _velocity * dt;

            // Apply the result.
            Graphics.Instance.PostProcessingSettings.FocalDistance = d;
        }

        private void Focus(Vector3 PointOnScreen)
        {
            TargetFocusPoint = PointOnScreen;
            // our ray
            var ray = transform.GetComponent<Camera>().ScreenPointToRay(PointOnScreen);
            RayOrigin = ray.origin;
#if DEBUG
            lineDrawer.DrawLineInGameView(ray.origin, ray.direction * 100, Color.yellow);
#endif
            if (Physics.Raycast(ray, out RaycastHit hit, MaxDistance, HitLayer))
            {
                HitPoint = hit.point;
#if DEBUG
                lineDrawer.DrawLineInGameView(ray.origin, hit.point, Color.green);
#endif
                // do we have a new point?					
                if (lastDoFPoint == hit.point)
                {
                    return;
                }

                _target.position = hit.point;
                // asign the last hit
                lastDoFPoint = hit.point;
            }
            /*
             * debug - Do any of the rays hit?
             * RaycastHit[] results = new RaycastHit[10];            
             
            if (0 < Physics.RaycastNonAlloc(ray, results, MaxDistance))
            {
                foreach (var result in results)
                {
                    // Check for null since some array spots might be
                    if (result.collider != null)
                    {
                        Graphics.Instance.Log.Log(BepInEx.Logging.LogLevel.All, "hit " + result.collider.gameObject.name);
                    }
                }
            }
            else
            {
                Graphics.Instance.Log.Log(BepInEx.Logging.LogLevel.All, "didn't hit");
            }
            */
        }
#if DEBUG
        public struct LineDrawer
        {
            private LineRenderer lineRenderer;
            private float lineSize;

            public LineDrawer(float lineSize = 0.2f)
            {
                GameObject lineObj = new GameObject("LineObj");
                lineRenderer = lineObj.AddComponent<LineRenderer>();
                //Particles/Additive
                lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

                this.lineSize = lineSize;
            }

            private void init(float lineSize = 0.2f)
            {
                if (lineRenderer == null)
                {
                    GameObject lineObj = new GameObject("LineObj");
                    lineRenderer = lineObj.AddComponent<LineRenderer>();
                    //Particles/Additive
                    lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

                    this.lineSize = lineSize;
                }
            }

            //Draws lines through the provided vertices
            public void DrawLineInGameView(Vector3 start, Vector3 end, Color color)
            {
                if (lineRenderer == null)
                {
                    init(0.2f);
                }

                //Set color
                lineRenderer.startColor = color;
                lineRenderer.endColor = color;

                //Set width
                lineRenderer.startWidth = lineSize;
                lineRenderer.endWidth = lineSize;

                //Set line count which is 2
                lineRenderer.positionCount = 2;

                //Set the postion of both two lines
                lineRenderer.SetPosition(0, start);
                lineRenderer.SetPosition(1, end);
            }

            public void Destroy()
            {
                if (lineRenderer != null)
                {
                    UnityEngine.Object.Destroy(lineRenderer.gameObject);
                }
            }
        }
#endif
    }
}