using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MouseMovement : MonoBehaviour
{
    private Player player;
    private Rigidbody2D rb;
    [SerializeField] private PlayerMovement playerRef;

    [SerializeField] private float mouseSpeed = 3.0f;
    [SerializeField] private float acceleration = 6.0f;
    [SerializeField] private float jumpHeight = 3.0f;
    [SerializeField] private float followDistance = 2.0f;
    [SerializeField] private GameObject feet;

    [SerializeField] private float burrowDistance = 7.5f; //burrow if the player is further than this ditance
    [SerializeField] private float emergeOffset = 0.4f; //0.39178 is the exact value
    [SerializeField] private List<Transform> passedNodes = new List<Transform>();
    [SerializeField] private Transform currentPassedNode = null; //last node player touched

    private Vector2 lastPosition;
    private float positionDifference;
    private float stuckThreshold = 3.0f;
    private float stuckTime = 0.0f;
    private float mouseFallenThreshold = 30.0f;
    private bool isCrushed = false;

    private bool spawned = true;
    private float horizontalDistance; 
    private float verticalDistance;

    private bool canMove = true;
    private bool startedMoving = false;
    private MouseAnimations animations;
    private bool isBurrowing = false;
    private bool isEmerging = false;
    private bool isTeleporting = false;
    private float stillTime = 0.0f;
    [SerializeField] private float timeToIdleAnim = 10.0f;

    void Start()
    {
        player = FindObjectOfType<Player>();
        playerRef = FindObjectOfType<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();

        // Configure Rigidbody settings
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true;

        animations = GetComponent<MouseAnimations>();
    }

    void Update()
    {
        if (!IsGrounded() && MouseAnimations.currentAnimation != "Jumping" && MouseAnimations.currentAnimation != "Landing")
            animations.ChangeAnimation("Falling");

        CheckAnimations();
    }

    void FixedUpdate()
    {
        if (spawned)
        {
            horizontalDistance = Mathf.Abs(player.transform.position.x - transform.position.x);
            verticalDistance = Mathf.Abs(player.transform.position.y - transform.position.y);

            if (canMove)
                MoveMouse();
            StuckCheck();
            MurderedCheck();
        }
    }

    private void MoveMouse()
    {
        if (!player) return;

        float direction = Mathf.Sign(player.transform.position.x - transform.position.x);

        // Stop if there's a ledge ahead
        if (IsLedgeAhead(direction))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            BurrowCheck();
            return;
        }

        if (horizontalDistance < followDistance * 0.95f)
        {
            Decelerate();
            return;
        }

        // Move towards the player
        float targetSpeed = direction * mouseSpeed;
        rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(targetSpeed, rb.velocity.y), acceleration * Time.deltaTime);

        transform.localScale = new Vector3(direction > 0 ? 1 : -1, 1, 1);

        // Handle jumping/burrowing
        if (IsGrounded() && playerRef.Grounded && IsObstacleAhead(direction))
        {
            if (verticalDistance > 0.5f && verticalDistance <= 2.0f)
            {
                animations.ChangeAnimation("Jumping");
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            }
            else if (verticalDistance > 2.0f)
                BurrowCheck();
        }
    }

    public void CheckAnimations()
    {
        if (MouseAnimations.currentAnimation == "Idle"
            || MouseAnimations.currentAnimation == "Jumping"
            || MouseAnimations.currentAnimation == "Landing"
            || MouseAnimations.currentAnimation == "Burrowing"
            || MouseAnimations.currentAnimation == "Emerging"
            || MouseAnimations.currentAnimation == "Downing"
            || MouseAnimations.currentAnimation == "Upping")
            return;

        if (MouseAnimations.currentAnimation == "Falling")
        {
            if (IsGrounded())
                animations.ChangeAnimation("Landing");
            return;
        }

        if (rb.velocity.magnitude >= 0.3f)
        {
            if (!startedMoving)
            {
                startedMoving = true;
                animations.ChangeAnimation("Downing");
                StartCoroutine(WaitForAnimation("Downing"));
            }
            else
                animations.ChangeAnimation("Running");
        }
        else
        {
            if (spawned)
            {
                stillTime += Time.deltaTime;

                if (stillTime > timeToIdleAnim)
                {
                    if (startedMoving)
                        animations.ChangeAnimation("Upping");
                    else
                    {
                        animations.ChangeAnimation("Idle");
                        StartCoroutine(WaitForAnimation("Idle"));
                        stillTime = 0.0f;
                    }
                }
                else
                {
                    if (startedMoving)
                        animations.ChangeAnimation("Upping");
                    else
                        animations.ChangeAnimation("Still");
                }
            }

            startedMoving = false;
        }
    }

    private IEnumerator WaitForAnimation(string nameOfAnimation)
    {
        canMove = false;
        rb.velocity = new Vector2(0, rb.velocity.y);

        yield return new WaitForSeconds(animations.GetAnimationLength(nameOfAnimation));

        canMove = true;
    }

    private bool IsGrounded()
    {
        return feet && feet.GetComponent<MouseFeet>()?.isGrounded == true;
    }

    private void Decelerate()
    {
        float deceleration = 5.0f;
        rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(0, rb.velocity.y), deceleration * Time.deltaTime);
    }

    private bool IsLedgeAhead(float direction)
    {
        Vector2 frontCheck = (Vector2)transform.position + new Vector2(direction * 0.7f, 0);
        RaycastHit2D groundCheck = Physics2D.Raycast(frontCheck, Vector2.down, 2.5f, LayerMask.GetMask("Environment"));

        Debug.DrawRay(frontCheck, Vector2.down * 2.0f, Color.red); // Debugging

        return groundCheck.collider == null; // No ground means it's a ledge
    }

    private bool IsObstacleAhead(float direction)
    {
        return Physics2D.Raycast(transform.position, Vector2.right * direction, 0.6f, LayerMask.GetMask("Environment"));
    }

    private void StuckCheck()
    {
        positionDifference = Vector2.Distance(transform.position, lastPosition);

        if (positionDifference < 0.5f && (horizontalDistance > burrowDistance + 1.0f || verticalDistance > burrowDistance + 1.0f)) //stays still for too long
        {
            stuckTime += Time.fixedDeltaTime;
            //Debug.Log("Stuck");

            if (stuckTime > stuckThreshold)
            {
                StartCoroutine(Burrow());
            }
        }
        else
            stuckTime = 0.0f;

        lastPosition = transform.position;
    }

    private void MurderedCheck()
    {
        if (verticalDistance > mouseFallenThreshold)
            StartCoroutine(Emerge());
    }

    private void BurrowCheck()
    {
        if (spawned && (horizontalDistance > burrowDistance || verticalDistance > burrowDistance))
        {
            StartCoroutine(Burrow());
        }
    }

    private IEnumerator Burrow()
    {
        if (currentPassedNode != null && !isTeleporting) // Prevent animation interruptions
        {
            isBurrowing = true;
            isTeleporting = true;
            canMove = false; // Stop movement

            animations.ChangeAnimation("Burrowing");
            yield return StartCoroutine(WaitForAnimation("Burrowing"));

            isBurrowing = false;
            StartCoroutine(Emerge()); // Proceed to emerge
        }
    }

    private IEnumerator Emerge()
    {
        if (currentPassedNode != null && !isEmerging) // Ensure we don't trigger twice
        {
            isEmerging = true;
            yield return new WaitForSeconds(0.5f); // Optional small delay before emerging

            animations.ChangeAnimation("Emerging");
            transform.position = new Vector2(currentPassedNode.position.x, currentPassedNode.position.y + emergeOffset);
            Debug.Log("Burrowed to " + currentPassedNode.name);

            yield return StartCoroutine(WaitForAnimation("Emerging")); // Wait for animation to finish

            isEmerging = false;
            isTeleporting = false; // Now allow movement and other animations
            canMove = true;
        }
    }

    public void PassedNode(Transform node)
    {
        if (!passedNodes.Contains(node))
        {
            passedNodes.Add(node);
        }

        currentPassedNode = node;
    }

    public void SetCrushed(bool status)
    {
        isCrushed = status;

        if (isCrushed)
        {
            transform.position = new Vector2(0, 0);
            StartCoroutine(Emerge());
        }
    }
}

