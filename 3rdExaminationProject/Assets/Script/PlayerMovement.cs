using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float _speed = 10f;
    [SerializeField] float _jumpForce = 15f; // 適切な値に調整
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
        _rb.useGravity = true; // 重力を有効にする
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
        // レイが地面と交差しているかどうかを検出
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
        // カメラのローカル座標系を基準に dir を変換する
        dir = Camera.main.transform.TransformDirection(dir);
        // カメラは斜め下に向いているので、Y 軸の値を 0 にして「XZ 平面上のベクトル」にする
        dir.y = 0;
        // 移動の入力がない時は回転させない。入力がある時はその方向にキャラクターを向ける。
        if (dir != Vector3.zero)
        {
            this.transform.forward = dir.normalized; // 移動方向に基づいて向きを変更
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

        moveDirection.Normalize(); // すべての入力を正規化してから速度を適用
        moveDirection.y = _rb.velocity.y;
        moveDirection.x *= _speed;
        moveDirection.z *= _speed;
        _rb.velocity = moveDirection;
    }

    void Jump()
    {
        if (_isGrounded && Input.GetKeyDown(KeyCode.Space) && _jumpCount <= _maxJumpCount && _timeCount >= _coolTime) // Space を使用して1回だけジャンプできるようにする
        {
            _timeCount = 0;
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse); // 上向きの力を追加
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
        // アニメーションの処理
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
