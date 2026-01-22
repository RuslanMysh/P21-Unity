using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private Player player;

    private const string IS_WALKING = "IsWalking";
    private const string IS_SPRINTING = "IsSprinting";
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetBool(IS_WALKING, false);
        animator.SetBool(IS_SPRINTING, false);
    }

    private void Update()
    {
        animator.SetBool(IS_WALKING, player.IsWalking);
        animator.SetBool(IS_SPRINTING, player.IsSprinting);
    }
}
