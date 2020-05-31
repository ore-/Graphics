using UnityEngine;

// Utility scripts for the post processing stack
// https://github.com/keijiro/PostProcessingUtilities
namespace Graphics
{
    public class FocusPuller : MonoBehaviour
    {
        private Graphics _parent;
        private float _velocity;
        private Vector3 lastDoFPoint;
        private bool initialFrame;
        private GameObject _ftgo;
        private Transform _target;
        public LayerMask hitLayer;
        public float maxDistance;

        public Transform FocusTarget
        {
            get
            {
                if (!_target)
                {
                    _ftgo = new GameObject("FocusTarget");
                    _target = _ftgo.transform;
                }
                return _target;
            }
            set => _target = value;
        }

        [SerializeField] private float _speed = 6f;
        public float speed
        {
            get => _speed;
            set => _speed = Mathf.Max(0.01f, value);
        }

        public void init(Graphics parent)
        {
            _parent = parent;
            hitLayer = _parent.CameraSettings.MainCamera.cullingMask;
            maxDistance = _parent.CameraSettings.MainCamera.farClipPlane;
        }

        private void Awake()
        {
            Physics.queriesHitBackfaces = true;

        }

        private void OnEnable()
        {
            initialFrame = true;
        }

        private void OnValidate()
        {
            speed = _speed;
        }

        private void LateUpdate()
        {
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
            float d1 = _parent.PostProcessingSettings.FocalDistance;//_controller.depthOfField.focusDistance;

            //Debug.DrawLine(transform.position, _target.position, Color.green);

            // Calculate the depth of the focus point.
            float d2 = Vector3.Dot((FocusTarget?.position ?? new Vector3(Screen.width / 2, Screen.height / 2)) - transform.position, transform.forward);
            if (d2 < 0.1f)
            {
                d2 = 0.1f;
            }

            // Damped-spring interpolation.
            float dt = Time.deltaTime;
            float n1 = _velocity - (d1 - d2) * speed * speed * dt;
            float n2 = 1 + speed * dt;
            _velocity = n1 / (n2 * n2);
            float d = d1 + _velocity * dt;

            // Apply the result.
            _parent.PostProcessingSettings.FocalDistance = d;
        }

        private void Focus(Vector3 PointOnScreen)
        {
            // our ray
            //var ray = transform.GetComponent<Camera>().ScreenPointToRay(PointOnScreen);
            Ray ray = _parent.CameraSettings.MainCamera.ScreenPointToRay(PointOnScreen);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitLayer))
            {
                //Debug.DrawLine(ray.origin, hit.point);
                // do we have a new point?					
                if (lastDoFPoint == hit.point)
                {
                    return;
                }

                FocusTarget.position = hit.point;
                // asign the last hit
                lastDoFPoint = hit.point;
            }
        }
    }
}
