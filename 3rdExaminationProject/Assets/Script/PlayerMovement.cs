using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float _speed = 10f;
    [SerializeField] float _jumpForce = 15f; // �K�؂Ȓl�ɒ���
    [SerializeField] float _gravPower = 10f;
    [SerializeField] Transform _player;
    [SerializeField] int _maxJumpCount = 2;
    [SerializeField] float _coolTime = 1.0f;

    float _timeCount = 0f;
    int _jumpCount = 0;
    public LayerMask _groundLayer;
    bool _isGrounded = true;

    Animator _animator;
    Rigidbody _rb;

    public bool IsGrounded
    {
        get { return _isGrounded; }
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
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
        _isGrounded = Physics.Raycast(ray, out hit, 0.1f, _groundLayer);
        _isGrounded = true;
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
        if (dir != Vector3.zero)
        {
            this.transform.forward = dir.normalized; // �ړ������Ɋ�Â��Č�����ύX
            dir = dir.normalized * _speed;
            dir.y = _rb.velocity.y;
            _rb.velocity = dir;
        }

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
    }

    void Jump()
    {
        if (_isGrounded && Input.GetKeyDown(KeyCode.Space) && _jumpCount <= _maxJumpCount && _timeCount >= _coolTime) // Space ���g�p����1�񂾂��W�����v�ł���悤�ɂ���
        {
            _timeCount = 0;
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse); // ������̗͂�ǉ�
            _jumpCount++;
            _isGrounded = false;
            Debug.Log("Jump");
            _jumpCount = 0;
        }
        else
        {
            _timeCount += Time.deltaTime;
        }
    }


    void LateUpdate()
    {
        // �A�j���[�V�����̏���
        if (_animator)
        {
            _animator.SetBool("IsGrounded", _isGrounded);
            //Debug.Log("JumpAnim");
            Vector3 walkSpeed = _rb.velocity;
            walkSpeed.y = 0;
            _animator.SetFloat("Speed", walkSpeed.magnitude);
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            _isGrounded = true;
            _jumpCount = 0;
        }
    }
}
