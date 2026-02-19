using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    public event EventHandler OnPickedSomething;

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField]
    private GameInput gameInput;

    [SerializeField]
    private Transform kitchenObjectPlace;

    [SerializeField]
    private float speed = 10f;

    [SerializeField]
    private float sprintModifier = 1.5f;

    [SerializeField]
    private float rotationSpeed = 10f;

    [SerializeField]
    private LayerMask counterMask;

    private Vector3 lastInteractionDir;

    private BaseCounter selectedCounter;

    private KitchenObject kitchenObject;

    public bool IsWalking { get; private set; }
    public bool IsSprinting { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("На уровне больше одного игрока! Синглтон сломался");
        }

        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
        gameInput.OnSprintStarted += GameInput_OnSprintStarted;
        gameInput.OnSprintCancelled += GameInput_OnSprintCancelled;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnSprintStarted(object sender, EventArgs e)
    {
        IsSprinting = true;
    }

    private void GameInput_OnSprintCancelled(object sender, EventArgs e)
    {
        IsSprinting = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractionDir = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractionDir,
            out RaycastHit raycastHit, interactDistance, counterMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                // Has ClearCounter
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float playerRadius = 0.7f;
        float playerHeight = 2f;
        float moveDist = speed * Time.deltaTime;

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
            playerRadius, moveDir, moveDist);

        IsWalking = moveDir != Vector3.zero;

        //if (!IsWalking) { IsSprinting = false; }

        if (!canMove)
        {
            //пробуем двинуться по X
            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f);
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
            playerRadius, moveDirX, moveDist);

            if (canMove)
            {
                //можем двигаться только по X
                moveDirX = moveDirX.normalized;
                moveDir = moveDirX;
            }
            else
            {
                //по X нельзя => пробуем по Z
                Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z);
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
                playerRadius, moveDirZ, moveDist);

                if (canMove)
                {
                    //можно двигаться только по Z
                    moveDirZ = moveDirZ.normalized;
                    moveDir = moveDirZ;
                }
                else
                {
                    //нельзя двигаться никуда
                }
            }
        }

        if (canMove)
        {
            float speedModifier;
            speedModifier = IsSprinting ? sprintModifier : 1;
            transform.position += speed * speedModifier * moveDir * Time.deltaTime;
        }

        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotationSpeed * Time.deltaTime);
    }
    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }
    private void SetSelectedCounter(BaseCounter baseCounter)
    {
        selectedCounter = baseCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = this.selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectPlace;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}