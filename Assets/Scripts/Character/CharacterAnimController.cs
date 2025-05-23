using UnityEngine;
using Mirror;
public class CharacterAnimController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float locomotionBlendSpeed = 1f;

    private FirstPersonController firstPersonController;
    private NetworkFirstPersonController networkFirstPersonController;
    private PlayerState playerState;
    private CharacterController characterController;

    private static int inputXhash = Animator.StringToHash("inputX");
    private static int inputYhash = Animator.StringToHash("inputY");
    private static int isCrouchHash = Animator.StringToHash("IsCrouch");
    private static int isFallingHash = Animator.StringToHash("IsFalling");
    private static int isJumpingHash = Animator.StringToHash("IsJumping");
    private static int isRunningHash = Animator.StringToHash("IsRunning");
    
    //private static int drawSwordHash = Animator.StringToHash("DrawSword");
    //private static int sheathSwordHash = Animator.StringToHash("SheathSword");
    //private static int attackSword = Animator.StringToHash("AttackSword");


    
    private Vector3 currentBlendInput = Vector3.zero;

    private void Awake()
    {
        firstPersonController = GetComponent<FirstPersonController>();
        networkFirstPersonController = GetComponent<NetworkFirstPersonController>();
        playerState = GetComponent<PlayerState>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        bool isCrouching = playerState.CurrentPlayerMovementState == PlayerMovementState.Crouching;
        bool isRunning = playerState.CurrentPlayerMovementState == PlayerMovementState.Running;
        bool isJumping = playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping;
        bool isFalling = playerState.CurrentPlayerMovementState == PlayerMovementState.Falling;
        bool isGrounded = characterController.isGrounded;
        bool isSprinting = playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
        Vector2 inputTarget;
        if (GetComponent<NetworkIdentity>() != null)
        {
            inputTarget = networkFirstPersonController.PlayerInput;
        }
        else
        {
            inputTarget = firstPersonController.PlayerInput;
        }
        
        currentBlendInput = Vector3.Lerp(currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);
        
        animator.SetBool(isCrouchHash, isCrouching);
        animator.SetBool(isFallingHash, isFalling);
        animator.SetBool(isRunningHash, isRunning);
        //animator.SetBool(isRunningHash, isSprinting);
        //animator.SetBool(, isGrounded);
        animator.SetBool(isJumpingHash, isJumping);

        if (!isSprinting && isRunning)
        {
            animator.SetFloat(inputXhash, currentBlendInput.x * 0.5f);
        }
        animator.SetFloat(inputXhash, currentBlendInput.x);
        animator.SetFloat(inputYhash, currentBlendInput.y);
        
    }

    public void SetTriggers(PlayerTrigger trigger)
    {
        animator.SetTrigger(trigger.ToString());
    }

    public Animator GetAnimator()
    {
        return animator;
    }
    
    public enum PlayerTrigger
    {
        DrawSword,
        SheathSword,
        AttackSword
    }

    
}