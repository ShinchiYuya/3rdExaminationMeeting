using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float _speed = 10f;
    [SerializeField] float _jumpForce = 15f; // �K�؂Ȓl�ɒ���
    [SerializeField] float _gravPower = 10f;
    [SerializeField] Transform player;
    [SerializeField] int maxJumpCount = 2;
    [SerializeField] float coolTime = 1.0f;
    
    float timer= 0f;
    int jumpCount = 0;
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
        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && jumpCount <= maxJumpCount && timer >= coolTime) // Space ���g�p����1�񂾂��W�����v�ł���悤�ɂ���
        {
            timer = 0;
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse); // ������̗͂�ǉ�
            jumpCount++;
            isGrounded = false;
            Debug.Log("1");
            jumpCount = 0;
        }
        else
        {
            timer += Time.deltaTime;
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

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }
}
