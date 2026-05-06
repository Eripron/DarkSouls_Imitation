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
        private float _fSkinWidth = 0.01f;
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
            // if(_bIsGround)
            //     _fVerticalSpeed = 0.0f;
            // else
            //     _fVerticalSpeed += -_fGravity * Time.deltaTime;

            Vector2 vInputMove = _inputMove.ReadValue<Vector2>().normalized;

            if(vInputMove != Vector2.zero)
            {
                Vector3 vMoveDir = CalaulateMoveDir(vInputMove);

                UpdatePlayerPosition(vMoveDir);
                UpdatePlayerViewDirection(vMoveDir);
            }
        }

        

        /// <summary>
        /// 이동 방향으로 CapsuleCollider를 미리 계산해서 벽 등에 충돌하는지 확인 후 충돌한다면 그 만큼 내부로 이동
        /// </summary>
        /// <param name="vMovePosition"></param>
        /// <returns></returns>\
        private Vector3 CheckPlayerCollision(Vector3 vMovePosition)
        {
            // _collider 기준으로 캡슐의 두 구체 중심 계산
            Vector3 vCenterWorld = _transform.TransformPoint(_collider.center);
            Vector3 vColliderDir = _transform.TransformDirection(_colliderDirection[_collider.direction]);

            float fHalfHeight = Mathf.Max(0f, _collider.height / 2f - _collider.radius);
            Vector3 vPoint1 = vCenterWorld + vColliderDir * fHalfHeight;
            Vector3 vPoint2 = vCenterWorld - vColliderDir * fHalfHeight;

            float fDistance = vMovePosition.magnitude;
            Vector3 vDir = vMovePosition.normalized;

            if (Physics.CapsuleCast(vPoint1, vPoint2, _collider.radius, vDir, out RaycastHit hit, fDistance, _collider.includeLayers))
            {
                // skinWidth 여백을 두어 벽 표면에 완전히 닿지 않도록 함
                return vDir * Mathf.Max(0f, hit.distance - _fSkinWidth);
            }

            return vMovePosition;
        }

        /// <summary>
        /// 유저 입력에 대한 이동 방향 계산
        /// </summary>
        /// <param name="vInputMoveDir"></param>
        /// <returns></returns>
        private Vector3 CalaulateMoveDir(Vector2 vInputMoveDir)
        {
           Vector3 vMoveDir = (_camera.GetForward() * vInputMoveDir.y) + (_camera.GetRight() * vInputMoveDir.x);
            vMoveDir.y = 0.0f;
            vMoveDir.Normalize();

            return vMoveDir;
        }

        private void UpdatePlayerPosition(Vector3 vMoveDir)
        {
            Vector3 vMovePosition = vMoveDir * _fMoveSpeed * Time.deltaTime;
            vMovePosition = CheckPlayerCollision(vMovePosition);

            SetPosition(GetPosition() + vMovePosition);
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