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
    public LayerMask groundLayerMask; // ���� �뵵��...

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot; // min�� max ���̿� ��ġ�� ī�޶� ���� ����
    public float lookSensitivity;

    private Vector2 mouseDelta;

    [HideInInspector] // public�ε�, �������� ����� �뵵
    public bool canLook = true;

    private Rigidbody _rigidbody; // ����� ������ ������ Component��� �ϴ� Ŭ���� ���� ������ rigidbody�� ȥ��� ���������� ��� ����. ����� ������� �ʱ� ������ ��� ������ ������ �Ű澲�δٸ� �����

    public static PlayerController instance; // �ϴ� �����÷��̾�� ���� ����� ���� static�� �̿��Ͽ� �̱�������.

    private void Awake()
    {
        instance = this;
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
    }
    private void FixedUpdate() // �����Ӵ� ó���� Update�ʹ� �ٸ��� ������ �ð� �������� ȣ��Ǿ�, ���� ���� ���õ� �۾��� ������.
    {
        
    }
    private void LateUpdate() // ��� ó���� ������ LateUpdate�� ����. ���� ī�޶� �۾��� ���� ����Ѵ�.
    {
        if (canLook)
        {
            CameraLook();
        }
    }
    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity; // ���콺 ���� �̵����� ī�޶� X�� ȸ��
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook); // min�� max ���̿� camCurXRot ���α�
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0); // ī�޶� ���� ȸ��ġ ����

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0); // �÷��̾� �¿� ȸ��ġ �ٷ� ����
        // ���콺�� ī�޶� �����̱� �غ� �Ϸ�

        //  ī�޶�� localEulerAngles�� ����ߴµ�, transform(�÷��̾�)�� eulerAngles�� ����� ����
        //  localEulerAngles �� �θ� ������Ʈ�� ���� ���� ��ǥ�迡���� ������Ʈ�� ȸ�� ������ ��Ÿ��.
        //  eulerAngles �� �۷ι�(����) ��ǥ�迡���� ������Ʈ�� ȸ�� ������ ��Ÿ��
        //  ī�޶��� ȸ���� �÷��̾ ���ӵǵ���, �÷��̾��� ȸ���� ������ǥ�迡 ���� �������� ȸ������ ó���ϵ��� �� ��.
    }
    public void OnLookInput(InputAction.CallbackContext context) // ���Ŀ� �̺�Ʈ�� ���� �� �� �޼���
    {
        //  ** ���� ���� **

        //  1. Callback
        //  Callback�� �⺻������ � �̺�Ʈ�� �߻����� �� �ý����̳� �ٸ� �ڵ忡 ���� �ڵ����� ȣ��Ǵ� �޼���(�Լ�)�� ����!!
        //  ��, Ư�� �̺�Ʈ�� �߻��ϸ� �׿� �����Ͽ� ����ǵ��� �̸� �����ص� �Լ�
        //  �ڵ� ȣ��, �̺�Ʈ ��� ���α׷���, �񵿱��� ���� �� Ư¡�� ����.
        //  InputAction.CallbackContext�� �Է� �̺�Ʈ�� ���� ������ �����ϴ� ��ü�̸�
        //  �Է� �ý��ۿ��� �߻��ϴ� �پ��� �̺�Ʈ(��: ��ư ����, ���콺 ������, ���̽�ƽ ���� ��)�� ���� ���� ������ �����ϰ� ����

        //  2. Coroutine (������ �� ���� ����)
        //  �ڷ�ƾ�� ���α׷��� ���� �κ��� ��ġ ���ÿ� ����Ǵ� ��ó�� ó���� �� �ְ� ���ָ鼭��, �����δ� �� ������ ������ ���� ������ �����ϰ� ������ �� �ִ� ����� �����Ѵ�.
        //  ��, �����庸�� ���ҽ��� ���� ��������� ����� �۵��� �񵿱� �۾��� ���� �� �� �ְ� ����


        // mouseDelta���� �޾ƿ�
        mouseDelta = context.ReadValue<Vector2>();

        //  
    }

}
