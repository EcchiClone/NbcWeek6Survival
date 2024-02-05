using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;
    public float jumpForce;
    public LayerMask groundLayerMask; 

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot; // min과 max 사이에 위치할 카메라 상하 각도
    public float lookSensitivity;

    private Vector2 mouseDelta;

    [HideInInspector] // public인데, 가리려고 만드는 용도
    public bool canLook = true;

    private Rigidbody _rigidbody; // 언더바 붙이지 않으면 Component라고 하는 클래스 내에 구현된 rigidbody와 혼용될 여지때문에 녹색 밑줄. 현재는 사용하지 않기 때문에 상관 없지만 밑줄이 신경쓰인다면 언더바

    public static PlayerController instance; // 일단 단일플레이어여서 간편 사용을 위한 static을 이용하여 싱글톤으로.

    private void Awake()
    {
        instance = this;
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
    }
    private void FixedUpdate() // 프레임당 처리인 Update와는 다르게 일정한 시간 간격으로 호출되어, 물리 계산과 관련된 작업에 적합함.
    {
        Move();
    }
    private void LateUpdate() // 모든 처리가 끝나고 LateUpdate가 동작. 보통 카메라 작업에 많이 사용한다.
    {
        if (canLook)
        {
            CameraLook();
        }
    }
    private void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;

        dir.y = _rigidbody.velocity.y; // y값을 없애겠다고 처리한다고는 하는데 잘 모르겠음. 나중에 확인 요망
        _rigidbody.velocity = dir;

    }
    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity; // 마우스 상하 이동으로 카메라가 X축 회전
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook); // min과 max 사이에 camCurXRot 가두기
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0); // 카메라에 현재 회전치 적용

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0); // 플레이어 좌우 회전치 바로 적용
        // 마우스로 카메라 움직이기 준비 완료

        //  카메라는 localEulerAngles을 사용했는데, transform(플레이어)은 eulerAngles를 사용한 이유
        //  localEulerAngles 는 부모 오브젝트에 대한 로컬 좌표계에서의 오브젝트의 회전 각도를 나타냄.
        //  eulerAngles 는 글로벌(월드) 좌표계에서의 오브젝트의 회전 각도를 나타냄
        //  카메라의 회전은 플레이어에 종속되도록, 플레이어의 회전은 월드좌표계에 대해 절대적인 회전으로 처리하도록 한 것.
    }
    public void OnLookInput(InputAction.CallbackContext context) // 추후에 이벤트와 연결 해 줄 메서드
    {
        //  ** 개념 보충 **

        //  1. Callback
        //  Callback은 기본적으로 어떤 이벤트가 발생했을 때 시스템이나 다른 코드에 의해 자동으로 호출되는 메서드(함수)를 말함!!
        //  즉, 특정 이벤트가 발생하면 그에 반응하여 실행되도록 미리 지정해둔 함수
        //  자동 호출, 이벤트 기반 프로그래밍, 비동기적 실행 의 특징을 가짐.
        //  InputAction.CallbackContext는 입력 이벤트에 대한 정보를 제공하는 객체이며
        //  입력 시스템에서 발생하는 다양한 이벤트(예: 버튼 누름, 마우스 움직임, 조이스틱 조작 등)에 대한 세부 정보를 포함하고 있음

        //  2. Coroutine (생각난 겸 같이 정리)
        //  코루틴은 프로그램의 여러 부분이 마치 동시에 실행되는 것처럼 처리될 수 있게 해주면서도, 실제로는 한 스레드 내에서 실행 순서를 세밀하게 제어할 수 있는 방법을 제공한다.
        //  즉, 스레드보다 리소스를 적게 사용하지만 비슷한 작동인 비동기 작업을 쉽게 할 수 있게 해줌


        // mouseDelta값을 받아옴
        mouseDelta = context.ReadValue<Vector2>();
    }
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed) // 키를 누르고 있는 도중. 처음 눌렸을때는 Started, 놓을 때는 Canceled
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started) // 키 입력을 받은 순간에만 점프하도록 Started 사용
        {
            if(IsGrounded())
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
                //  ForceMode에 대해
                //  .Force : 지속적인 힘을 물체에 적용. 뉴턴의 운동 법칙에 따라 작동하며, 물체에 점진적으로 가속을 제공한다.
                //  .Impulse : 순간적으로 큰 힘을 물체에 적용.
                //  .VelocityChange : 물체의 속도를 즉시 변경하며, 이 때 적용되는 힘은 물체의 질량에 영향을 받지 않음.
                //                    즉, 물체의 속도를 직접 조정하고 싶을 때 사용. 순간적으로 물체를 정지, 반대방향으로 빠르게 가속.
                //  .Acceleration : 물체에 지속적인 가속을 적용. Force와는 다르게 질량을 무시하고 가속도만을 적용
        }
    }

    private bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (Vector3.up*0.01f), Vector3.down),// 시작위치와 방향 지정
            new Ray(transform.position + (-transform.forward * 0.2f) + (Vector3.up*0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (Vector3.up*0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (Vector3.up*0.01f), Vector3.down),
        };
        for(int i = 0; i < rays.Length; i++)
        {
            // 아래의 0.1f 는 Ray의 시작점으로부터의 거리.
            // Ray가 groundLayerMask를 감지하면 true를 리턴한다.
            // 플레이어 콜라이더는 이 조건에 전혀 관여하지 않는다. 오직 Ray가 groundLayerMask를 만나느냐 뿐
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask)) 
            {
                return true;
            }
        }
        return false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + (transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (transform.right * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.right * 0.2f), Vector3.down);
    }
    public void ToggleCursor(bool toggle)
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
