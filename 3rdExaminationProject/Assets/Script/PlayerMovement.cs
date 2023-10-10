using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float _speed = 10f;
    [SerializeField] float _jumpForce = 10f; // �K�؂Ȓl�ɒ���
    [SerializeField] float _gravPower = 20f;

    bool isGrounded = true;
    Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = true; // �d�͂�L���ɂ���
    }

    void Update()
    {
        Gravity();
        Move();
        Jump();
        Raycast();
    }

    void Gravity()
    {
        float gravity = _gravPower;
        Vector3 gravityVector = new Vector3(0, -gravity, 0);
        _rb.AddForce(gravityVector, ForceMode.Acceleration);
    }

    void Move()
    {
        Vector3 moveDirection = _rb.velocity;

        if (Input.GetKey(KeyCode.W))
            moveDirection += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            moveDirection += Vector3.back;
        if (Input.GetKey(KeyCode.A))
            moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            moveDirection += Vector3.right;
        moveDirection.Normalize(); // ���ׂĂ̓��͂𐳋K�����Ă��瑬�x��K�p
        moveDirection.y = _rb.velocity.y;
        moveDirection.x *= _speed;
        moveDirection.z *= _speed;
        _rb.velocity = moveDirection;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = Vector3.forward * v + Vector3.right * h;
        // �J�����̃��[�J�����W�n����� dir ��ϊ�����
        dir = Camera.main.transform.TransformDirection(dir);
        // �J�����͎΂߉��Ɍ����Ă���̂ŁAY ���̒l�� 0 �ɂ��āuXZ ���ʏ�̃x�N�g���v�ɂ���
        dir.y = 0;
        // �ړ��̓��͂��Ȃ����͉�]�����Ȃ��B���͂����鎞�͂��̕����ɃL�����N�^�[��������B
        if (dir != Vector3.zero) this.transform.forward = dir;
        _rb.velocity = dir.normalized * _speed;
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space)) // GetKeyDown ���g�p����1�񂾂��W�����v�ł���悤�ɂ���
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse); // ������̗͂�ǉ�
            isGrounded = false;
        }
    }

    void Raycast()
    {
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = Vector3.down;
        float rayLength = .5f;

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
