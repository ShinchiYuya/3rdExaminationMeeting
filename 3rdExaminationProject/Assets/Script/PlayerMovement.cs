using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float _speed = 10f;
    [SerializeField] float _jumpForce = 15f; // �K�؂Ȓl�ɒ���
    [SerializeField] float _gravPower = 10f;
    [SerializeField] Transform player;

    public LayerMask groundLayer;
    bool isGrounded = true;

    Animator animator;
    Rigidbody _rb;

    public bool IsGrounded
    {
        get { return isGrounded; }
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        _rb.useGravity = true; // �d�͂�L���ɂ���
    }

    void Update()
    {
        Gravity();
        Move();
        Jump();
        Raycast();
    }

    void Raycast()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        // ���C���n�ʂƌ������Ă��邩�ǂ��������o
        isGrounded = Physics.Raycast(ray, out hit, 0.1f, groundLayer);
        // �f�o�b�O�p�ɉ���
        Debug.DrawRay(transform.position, Vector3.down * 0.1f, isGrounded ? Color.green : Color.red);
        isGrounded = true;
    }


    void Gravity()
    {
        float gravity = _gravPower;
        Vector3 gravityVector = new Vector3(0, -gravity, 0);
        _rb.AddForce(gravityVector, ForceMode.Acceleration);
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = Vector3.forward * v + Vector3.right * h;
        // �J�����̃��[�J�����W�n����� dir ��ϊ�����
        dir = Camera.main.transform.TransformDirection(dir);
        // �J�����͎΂߉��Ɍ����Ă���̂ŁAY ���̒l�� 0 �ɂ��āuXZ ���ʏ�̃x�N�g���v�ɂ���
        dir.y = 0;
        // �ړ��̓��͂��Ȃ����͉�]�����Ȃ��B���͂����鎞�͂��̕����ɃL�����N�^�[��������B
        if (dir != Vector3.zero) this.transform.forward = dir;
        dir = dir.normalized * _speed;
        dir.y = _rb.velocity.y;
        _rb.velocity = dir;

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

        //MoveAnim();
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space)) // Space ���g�p����1�񂾂��W�����v�ł���悤�ɂ���
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse); // ������̗͂�ǉ�
            isGrounded = false;
        }
    }

    void LateUpdate()
    {
        // �A�j���[�V�����̏���
        if (animator)
        {
            animator.SetBool("IsGrounded", isGrounded);
            Vector3 walkSpeed = _rb.velocity;
            walkSpeed.y = 0;
            animator.SetFloat("Speed", walkSpeed.magnitude);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
