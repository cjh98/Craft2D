using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    float speed;

    public float walkSpeed;
    public float sprintSpeed;

    public Animator animator;

    public bool isSprinting = false;

    //float movingThreshold = 0.1f;

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public Direction direction = Direction.Down;

    void Start()
    {

    }

    void Update()
    {

    }
    
    public void Move(Vector3 velocity)
    {
        speed = isSprinting ? sprintSpeed : walkSpeed;

        animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
        animator.SetFloat("Vertical", Input.GetAxis("Vertical"));

        if (Input.GetKey(KeyCode.D))
        {
            animator.SetBool("WasMovingRight", true);
            animator.SetBool("WasMovingLeft", false);
            animator.SetBool("WasMovingUp", false);
            animator.SetBool("WasMovingDown", false);

            direction = Direction.Right;
        }

        if (Input.GetKey(KeyCode.A))
        {
            animator.SetBool("WasMovingRight", false);
            animator.SetBool("WasMovingLeft", true);
            animator.SetBool("WasMovingUp", false);
            animator.SetBool("WasMovingDown", false);

            direction = Direction.Left;
        }

        if (Input.GetKey(KeyCode.W))
        {
            animator.SetBool("WasMovingRight", false);
            animator.SetBool("WasMovingLeft", false);
            animator.SetBool("WasMovingUp", true);
            animator.SetBool("WasMovingDown", false);

            direction = Direction.Up;
        }

        if (Input.GetKey(KeyCode.S))
        {
            animator.SetBool("WasMovingRight", false);
            animator.SetBool("WasMovingLeft", false);
            animator.SetBool("WasMovingUp", false);
            animator.SetBool("WasMovingDown", true);

            direction = Direction.Down;
        }

        transform.position += velocity * speed * Time.deltaTime;
    }
}
