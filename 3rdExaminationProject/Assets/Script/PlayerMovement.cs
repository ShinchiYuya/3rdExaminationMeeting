using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float _speed = 10f;
    [SerializeField] float _jumpForce = 15f; // 適切な値に調整
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
        // カメラのローカル座標系を基準に dir を変換する
        dir = Camera.main.transform.TransformDirection(dir);
        // カメラは斜め下に向いているので、Y 軸の値を 0 にして「XZ 平面上のベクトル」にする
        dir.y = 0;
        // 移動の入力がない時は回転させない。入力がある時はその方向にキャラクターを向ける。
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

        moveDirection.Normalize(); // すべての入力を正規化してから速度を適用
        moveDirection.y = _rb.velocity.y;
        moveDirection.x *= _speed;
        moveDirection.z *= _speed;
        _rb.velocity = moveDirection;

        //MoveAnim();
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && jumpCount <= maxJumpCount && timer >= coolTime) // Space を使用して1回だけジャンプできるようにする
        {
            timer = 0;
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse); // 上向きの力を追加
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
        // アニメーションの処理
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
