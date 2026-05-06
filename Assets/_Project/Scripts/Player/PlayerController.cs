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
        /// 
        /// </summary>
        /// <param name="vMovePosition"></param>
        /// <returns></returns>
        private Vector3 CheckCollider(Vector3 vMovePosition)
        {

            return Vector3.zero;
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
            Vector3 vMove = vMoveDir * _fMoveSpeed * Time.deltaTime;

            SetPosition(GetPosition() + vMove);
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