using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float _speed = 10f;
    [SerializeField] float _jumpForce = 10f; // 適切な値に調整
    [SerializeField] float _gravPower = 20f;

    bool isGrounded = true;
    Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = true; // 重力を有効にする
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
        moveDirection.Normalize(); // すべての入力を正規化してから速度を適用
        moveDirection.y = _rb.velocity.y;
        moveDirection.x *= _speed;
        moveDirection.z *= _speed;
        _rb.velocity = moveDirection;
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space)) // GetKeyDown を使用して1回だけジャンプできるようにする
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse); // 上向きの力を追加
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
