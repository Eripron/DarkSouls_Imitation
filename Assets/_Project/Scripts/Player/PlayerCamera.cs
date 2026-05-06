using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField]
        private Transform _transTarget;

        [SerializeField]
        [Range(1.0f, 20.0f)]
        private float _fRadius;

        [SerializeField]
        private float _fMaxPitch = 85.0f;
        [SerializeField]
        private float _fMinPitch = -85.0f;

        [SerializeField]
        private float _fSensitive;
        [SerializeField]
        private float _fMaxMouseDeltaPerFrame = 50.0f;

        [SerializeField]
        private SphereCollider _collider;

        private Transform _transform;
        private InputAction _inputMouseMove;

        private float _fYaw = 0f;
        private float _fPitch = 20f;
        private Vector3 _vCameraViewDir = Vector3.zero;


        private void Awake()
        {
            _transform = transform;

            if(!_transTarget)
                Debug.LogError("[PlayerCamera] Target Transform이 설정되지 않았습니다.");
        }

        private void Start()
        {
            _inputMouseMove = InputSystem.actions.FindAction("MouseMove");
        }

        private void LateUpdate()
        {
            if(!_transTarget) return;

            UpdateCameraRotation();
            float fCameraDistance = CalculateCameraDistance();

            Vector3 vCamearPosition = _transTarget.position + (-_vCameraViewDir * fCameraDistance);
            transform.position = Vector3.Lerp(_transform.position, vCamearPosition, 0.2f);

            transform.LookAt(_transTarget);
        }

#region << Update Camera >>

        private void UpdateCameraRotation()
        {
            Vector2 vMouseMove = Vector2.ClampMagnitude(_inputMouseMove.ReadValue<Vector2>(), _fMaxMouseDeltaPerFrame);
            Vector2 vCamRot = Time.deltaTime * _fSensitive * vMouseMove;

            float fDeltaYaw = vCamRot.x;
            float fDeltaPitch = vCamRot.y;

            _fYaw += fDeltaYaw;
            _fPitch = Mathf.Clamp(_fPitch - fDeltaPitch, _fMinPitch, _fMaxPitch);

            // 360도 회전 각도 적용
            if (_fYaw < 0.0f)
                _fYaw += 360.0f;
            else if (_fYaw >= 360.0f)
                _fYaw -= 360.0f;

            Quaternion qViewRotation = Quaternion.AngleAxis(_fYaw, Vector3.up) * Quaternion.AngleAxis(_fPitch, Vector3.right);
            _vCameraViewDir = qViewRotation * Vector3.forward;
        }

        private float CalculateCameraDistance()
        {
            float fCameraDistance = _fRadius;

            RaycastHit hitInfo;
            if (Physics.SphereCast(_transTarget.position, _collider.radius, -_vCameraViewDir, out hitInfo, _fRadius, _collider.includeLayers))
            {
                fCameraDistance = Mathf.Clamp(hitInfo.distance, 1.0f, _fRadius);
            }

            return fCameraDistance;
        }

#endregion

        public Vector3 GetPosition()
        {
            return _transform.position;
        }

        public Vector3 GetForward()
        {
            return _transform.forward;
        }

        public Vector3 GetRight()
        {
            return _transform.right;
        }
    }
}
