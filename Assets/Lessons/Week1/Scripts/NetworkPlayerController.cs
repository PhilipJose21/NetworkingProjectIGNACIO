using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float groundedGravity = -2f;
    [SerializeField] float jumpHeight = 1.5f;
    public NetworkVariable<int> points = new NetworkVariable<int>(0);
    
    private CharacterController characterController;
    private float verticalVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool jumpPressed = Input.GetButtonDown("Jump");
        Vector2 inputDirection = new Vector2(horizontalInput, verticalInput);
        if (IsServer)
        {
            MovePlayer(inputDirection, jumpPressed);
        }
        else
        {
            MovePlayerRPC(inputDirection, jumpPressed);
        }
    }
    [Rpc(SendTo.Server)]

    private void MovePlayerRPC(Vector2 inputDirection, bool jumpPressed)
    {
        MovePlayer(inputDirection, jumpPressed);
    }
    private void MovePlayer(Vector2 movementInput, bool jumpPressed)
    {
        if (characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = groundedGravity;
            if (jumpPressed)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        Vector3 horizontalMovement = moveDirection * moveSpeed;
        Vector3 verticalMovement = Vector3.up * verticalVelocity;
        Vector3 finalMovement = horizontalMovement + verticalMovement;
        characterController.Move(finalMovement * Time.deltaTime);
    }

    public void AddPointDirect()
    {
        points.Value += 1;
    }
    [ServerRpc]
    public void AddPointServerRpc()
    {
        points.Value++;
        Debug.Log($"Player {OwnerClientId} scored! Total points: {points.Value}");
    }

    // Helper to read points on any side
    public int GetPoints()
    {
        return points.Value;
    }
}
