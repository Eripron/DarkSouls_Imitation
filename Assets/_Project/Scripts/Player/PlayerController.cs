using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private PlayerCamera _camera;

        [SerializeField]
        [Range(0.1f, 10.0f)]
        private float _fMoveSpeed;
        
        [SerializeField]
        [Range(0.1f, 1.0f)]
        private float _fRotSpeed;

        [SerializeField]
        [Range(1.0f, 10.0f)]
        private float _fGravity;

        // CapsuleCollider.direction: 0=X, 1=Y, 2=Z (Unity 공식 문서 기준)
        [SerializeField] 
        private float _fSkinWidth = 0.08f;
        private Vector3[] _colliderDirection = { Vector3.right, Vector3.up, Vector3.forward };

        private Transform _transform;
        private InputAction _inputMove;
        private CapsuleCollider _collider;

        private float _fVerticalSpeed;  // 수직 속도
        private bool _bIsGround;


        private void Awake()
        {
            _transform = transform;
            _inputMove = InputSystem.actions.FindAction("Move");
            _collider = GetComponent<CapsuleCollider>();
        }

        private void Update()
        {
            Vector2 vInputMove = _inputMove.ReadValue<Vector2>().normalized;

            if(vInputMove != Vector2.zero)
            {
                Vector3 vMoveDir = CalaulateMoveDir(vInputMove);

                UpdatePlayerPosition(vMoveDir);
                UpdatePlayerViewDirection(vMoveDir);
            }
        }

        /// <summary>
        /// 유저 입력에 대한 이동 방향 계산
        /// </summary>
        /// <param name="vInputMove"></param>
        /// <returns></returns>
        private Vector3 CalaulateMoveDir(Vector2 vInputMove)
        {
            Vector3 vMoveDir = (_camera.GetForward() * vInputMove.y) + (_camera.GetRight() * vInputMove.x);
            vMoveDir.y = 0.0f;
            vMoveDir.Normalize();

            return vMoveDir;
        }
        
        private void UpdatePlayerPosition(Vector3 vMoveDir)
        {
            // 이동 위치
            Vector3 vMovePosition = vMoveDir * _fMoveSpeed * Time.deltaTime;
            vMovePosition = CheckPlayerCollision(vMovePosition);

            SetPosition(GetPosition() + vMovePosition);
        }

        /// <summary>
        /// 벽과 충돌 처리 및 최종 이동 위치 계산
        /// </summary>
        /// <param name="vMovePosition"></param>
        /// <returns></returns>\
        private Vector3 CheckPlayerCollision(Vector3 vMovePosition)
        {
            // _collider 기준으로 캡슐의 두 구체 중심 계산
            Vector3 vColliderCenterWorld = _transform.TransformPoint(_collider.center);
            Vector3 vColliderDir = _transform.TransformDirection(_colliderDirection[_collider.direction]);

            float fHalfHeight = Mathf.Max(0f, _collider.height / 2f - _collider.radius);
            Vector3 vWorldPoint1 = vColliderCenterWorld + vColliderDir * fHalfHeight;
            Vector3 vWorldPoint2 = vColliderCenterWorld - vColliderDir * fHalfHeight;

            Vector3 vMoveDir = vMovePosition.normalized;    // 이동 방향
            float fMoveDistance = vMovePosition.magnitude;  // 이동 거리

            if (Physics.CapsuleCast(vWorldPoint1, vWorldPoint2, _collider.radius, vMoveDir, 
                out RaycastHit hit, fMoveDistance, _collider.includeLayers))
            {
                // 벽까지 이동 가능한 거리 계산 (skinWidth 여유)
                float fMovable = Mathf.Max(0f, hit.distance - _fSkinWidth);

                // 남은 이동 거리를 벽면에 투영해서 슬라이딩
                float fRemaining = fMoveDistance - fMovable;
                Vector3 vSlide = Vector3.ProjectOnPlane(vMoveDir, hit.normal) * fRemaining;

                return fMovable * vMoveDir + vSlide;
            }

            return vMovePosition;
        }

        /// <summary>
        /// 플레이어 캐릭터의 방향을 계산 및 적용
        /// </summary>
        /// <param name="vMoveDir"></param>
        private void UpdatePlayerViewDirection(Vector3 vMoveDir)
        {
            Quaternion qCurrent = _transform.rotation;
            Quaternion qLook = Quaternion.LookRotation(vMoveDir);
            _transform.rotation = Quaternion.Slerp(qCurrent, qLook, _fRotSpeed);
        }

        #region <<set, get>>

        private void SetPosition(Vector3 vPosition)
        {
            _transform.position = vPosition;   
        }

        private Vector3 GetPosition()
        {
            return _transform.position;
        }

        #endregion

    }
}