using System.Collections;
using KKAPI;
using Studio;
using UnityEngine;

// Utility scripts for the post processing stack
// https://github.com/keijiro/PostProcessingUtilities
namespace Graphics
{
    public class FocusPuller : MonoBehaviour
    {
        internal static float MinSpeed = 1f;
        internal static float MaxSpeed = 12f;

        private GameObject _customTarget;
        private GameObject _focusTargetGameObject;
        private float _speed = 6f;
        private Transform _target;
        private float _velocity;

        internal float MaxDistance { get; set; }
        internal Vector3 TargetPosition => null != _target ? _target.position : Vector3.zero;

        internal float Speed
        {
            get => _speed;
            set => _speed = Mathf.Max(MinSpeed, value);
        }

        private void Awake()
        {
            _focusTargetGameObject = new GameObject("FocusTarget");
            _target = _focusTargetGameObject.transform;
            MaxDistance = Graphics.Instance.CameraSettings.MainCamera.farClipPlane;
        }

        private void OnEnable()
        {
            StartCoroutine(FindTarget());
        }

        private void OnPreRender()
        {
            if (_target == null)
            {
                return;
            }

            Focus();

            // Retrieve the current value.
            float d1 = Graphics.Instance.PostProcessingSettings.FocalDistance;

            // Calculate the depth of the focus point.
            float d2 = Vector3.Dot(_target.position - transform.position, transform.forward);
            if (d2 < 0.1f)
            {
                d2 = 0.1f;
            }

            // Damped-spring interpolation.
            float dt = Time.deltaTime;
            float n1 = _velocity - ((d1 - d2) * Speed * Speed * dt);
            float n2 = 1 + (Speed * dt);
            _velocity = n1 / (n2 * n2);
            float d = d1 + (_velocity * dt);

            // Apply the result.
            Graphics.Instance.PostProcessingSettings.FocalDistance = d;
        }

        private void OnValidate()
        {
            Speed = _speed;
        }

        private IEnumerator FindTarget()
        {
            while (isActiveAndEnabled)
            {
                yield return new WaitForSecondsRealtime(.5f);
                _customTarget = null;
                foreach (ObjectCtrlInfo info in Singleton<Studio.Studio>.Instance.dicObjectCtrl.Values)
                {
                    if (!(info is OCIFolder folder))
                    {
                        continue;
                    }

                    if (folder.name.Equals("DOF Target") && !ReferenceEquals(folder.objectItem, null) && folder.objectItem.activeInHierarchy)
                    {
                        _customTarget = folder.objectItem;
                    }
                }
            }
        }

        private void Focus()
        {
            if (KoikatuAPI.GetCurrentGameMode() == GameMode.Studio)
            {
                // ReSharper disable once MergeConditionalExpression
                _target.position = _customTarget != null ? _customTarget.transform.position : Singleton<Studio.Studio>.Instance.cameraCtrl.targetPos;
            }
        }
    }
}