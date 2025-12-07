using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    private PlayerLocomotionMap _playerLocomotionMap;
    private PlayerState _playerState;
    private CharacterController _characterController;
    public Camera _playerCamera;
    
    // YENİ: Kameranın odaklanacağı nokta (Örn: Kafanın olduğu yer veya omuz)
    [Tooltip("Kameranın takip edeceği obje. Player'ın içinde boş bir GameObject oluşturup buraya ata.")]
    public Transform cameraTarget; 

    [Header("TPS Camera Settings")]
    [SerializeField] private float lookSenseH = 2f;
    [SerializeField] private float lookSenseV = 2f;
    [SerializeField] private float lookLimitV = 70f; // Aşağı/Yukarı bakma limiti
    [SerializeField] private float distanceFromTarget = 3.0f; // Kameranın karakterden uzaklığı
    [SerializeField] private float minCameraDistance = 0.5f; // Kamera duvara sıkışırsa ne kadar yaklaşabilir
    [SerializeField] private LayerMask cameraCollisionLayers; // Kameranın çarpacağı layerlar (Duvar vs.)
    
    private float _currentX;
    private float _currentY;

    [Header("Movement Settings")]
    [SerializeField] private float runAcceleration = 25f; // Hızlanma değerlerini biraz artırdım, TPS daha atik hissettirmeli
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float sprintAcceleration = 35f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float drag = 5f; // Drag değerini artırdım, kayma hissini azaltmak için
    [SerializeField] private float movingThreshold = 0.1f;
    [SerializeField] private float rotationSmoothTime = 0.1f; // Karakterin dönüş yumuşaklığı
    
    private float _rotationVelocity; // SmoothDamp için yardımcı değişken

    [Header("Gravity Settings")]
    [SerializeField] private float gravity = 25f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float inAirAcceleration = 10f;
    [SerializeField] private LayerMask groundLayers;
    private float verticalVelocity = 0f;
    private float antiBump;
    private float stepOffset;

    private void Awake()
    {
        _playerLocomotionMap = GetComponent<PlayerLocomotionMap>();
        _characterController = GetComponent<CharacterController>();
        _playerState = GetComponent<PlayerState>();

        antiBump = sprintSpeed;
        stepOffset = _characterController.stepOffset;

        // Mouse imlecini gizle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Eğer editörden atanmadıysa target olarak kendisini seçsin (Fallback)
        if (cameraTarget == null) cameraTarget = transform;
    }

    private void Update()
    {
        UpdateMovementState();
        HandleVerticalMovement();
        HandleLateralMovement();
    }

    private void LateUpdate()
    {
        HandleCameraRotation();
    }

    // YENİ: TPS Kamera Mantığı
    private void HandleCameraRotation()
    {
        if (_playerCamera == null || cameraTarget == null) return;

        // Inputları al
        _currentX += _playerLocomotionMap._lookInput.x * lookSenseH;
        _currentY -= _playerLocomotionMap._lookInput.y * lookSenseV;
        
        // Açıyı kısıtla (Takla atmaması için)
        _currentY = Mathf.Clamp(_currentY, -lookLimitV, lookLimitV);

        // Rotasyonu hesapla
        Vector3 direction = new Vector3(0, 0, -distanceFromTarget);
        Quaternion rotation = Quaternion.Euler(_currentY, _currentX, 0);
        
        // YENİ: Kamera Çarpışma Kontrolü (Duvarların içinden geçmemesi için)
        Vector3 desiredCameraPos = cameraTarget.position + rotation * direction;
        RaycastHit hit;
        // Target'tan kameranın olması gereken yere ışın atıyoruz
        if (Physics.Linecast(cameraTarget.position, desiredCameraPos, out hit, cameraCollisionLayers))
        {
            // Eğer bir şeye çarparsa, mesafeyi çarpılan noktaya çekiyoruz (biraz pay bırakarak)
            float distanceToHit = Vector3.Distance(cameraTarget.position, hit.point);
            // Çok fazla yaklaşmaması için clamp
            float clampedDistance = Mathf.Clamp(distanceToHit * 0.9f, minCameraDistance, distanceFromTarget); 
            desiredCameraPos = cameraTarget.position + rotation * new Vector3(0, 0, -clampedDistance);
        }

        // Kamerayı konumlandır
        _playerCamera.transform.position = desiredCameraPos;
        _playerCamera.transform.LookAt(cameraTarget.position);
    }

    private void UpdateMovementState()
    {
        bool isMovementInput = _playerLocomotionMap._moveInput != Vector2.zero;
        bool isMovingLaterally = IsMovingLaterally();
        bool isMovingBackward = _playerLocomotionMap._moveInput.y < 0;
        bool isSprinting = _playerLocomotionMap._sprintToggleOn && isMovementInput && !isMovingBackward;
        bool isGrounded = IsGrounded();

        StateType lateralState = isSprinting ? StateType.Sprinting : 
            isMovingLaterally || isMovementInput ? StateType.Running : StateType.Idling;
        
        _playerState.SetPlayerMovementState(lateralState);

        if (!isGrounded && _characterController.velocity.y >= 0)
        {
            _playerState.SetPlayerMovementState(StateType.Jumping);
            _characterController.stepOffset = 0;
        }
        else if(!isGrounded && _characterController.velocity.y < 0)
        {
            _playerState.SetPlayerMovementState(StateType.Falling);
            _characterController.stepOffset = 0;
        }
        else
        {
            _characterController.stepOffset = stepOffset;
        }
    }

    void HandleVerticalMovement()
    {
        bool isGrounded = _playerState.InGroundedState();

        verticalVelocity -= gravity * Time.deltaTime;
        
        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -antiBump;

        if (_playerLocomotionMap._jumpPressed && isGrounded)
        {
            verticalVelocity += antiBump + Mathf.Sqrt(jumpHeight * 3f * gravity);
        }
    }

   void HandleLateralMovement()
    {
        bool isSprinting = _playerState.currentStat == StateType.Sprinting;
        bool isGrounded = _playerState.InGroundedState();

        // Inputları al
        Vector2 input = _playerLocomotionMap._moveInput;
        
        // Hız belirle
        float targetSpeed = !isGrounded ? sprintSpeed : isSprinting ? sprintSpeed : runSpeed;
        if (input == Vector2.zero) targetSpeed = 0;

        float lateralAcceleration = !isGrounded ? inAirAcceleration : isSprinting ? sprintAcceleration : runAcceleration;

        // --- DEĞİŞİKLİK BURADA (STRAFE LOGIC) ---
        
        // 1. Karakterin yönünü kameranın baktığı yöne çevir (Sadece Y ekseni)
        if (_playerCamera != null)
        {
            float cameraYaw = _playerCamera.transform.eulerAngles.y;
            // Karakteri kameranın baktığı yöne döndür (Yumuşak geçiş için SmoothDamp kullanılabilir ama Shooter'da net dönüş iyidir)
            transform.rotation = Quaternion.Euler(0, cameraYaw, 0);
        }

        // 2. Hareket yönünü Kameraya göre hesapla
        // Kamera yönüne göre ileri ve sağ vektörlerini al
        Vector3 cameraForward = transform.forward;
        Vector3 cameraRight = transform.right;
        
        // Input.y (W/S) ileri/geri, Input.x (A/D) sağ/sol strafe
        Vector3 movementDirection = (cameraForward * input.y + cameraRight * input.x).normalized;

        // --- DEĞİŞİKLİK BİTTİ ---

        // Mevcut Hız Hesaplamaları (Aynen Kalıyor)
        Vector3 currentLateralVelocity = new Vector3(_characterController.velocity.x, 0, _characterController.velocity.z);
        Vector3 targetVelocityVector = movementDirection * targetSpeed;
        
        Vector3 newLateralVelocity = Vector3.MoveTowards(currentLateralVelocity, targetVelocityVector, lateralAcceleration * Time.deltaTime);

        Vector3 finalVelocity = newLateralVelocity;
        finalVelocity.y = verticalVelocity;

        finalVelocity = !isGrounded ? HandleSteepWalls(finalVelocity) : finalVelocity;
        
        _characterController.Move(finalVelocity * Time.deltaTime);
    }
    
    private Vector3 HandleSteepWalls(Vector3 velocity)
    {
        Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(_characterController, groundLayers);
        float angle = Vector3.Angle(normal, Vector3.up);
        bool validAngle = angle <= _characterController.slopeLimit;

        if (!validAngle && verticalVelocity < 0f)
            velocity = Vector3.ProjectOnPlane(velocity, normal);

        return velocity;
    }
    
    bool IsMovingLaterally()
    {
        Vector3 lateralVelocity = new Vector3(_characterController.velocity.x, 0, _characterController.velocity.z);
        return lateralVelocity.magnitude > movingThreshold;
    }

    bool IsGrounded()
    {
        return _playerState.InGroundedState() ? IsGroundedWhileGrounded() : IsGroundedWhileAirborne();
    }

    private bool IsGroundedWhileGrounded()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, 
            transform.position.y - _characterController.radius, 
            transform.position.z);

        return Physics.CheckSphere(spherePosition, _characterController.radius, groundLayers, QueryTriggerInteraction.Ignore);
    }

    private bool IsGroundedWhileAirborne()
    {
        // Not: Burada SphereCast normal kontrolü yapmışsın, aynen bıraktım.
        // Genellikle CharacterController.isGrounded daha stabildir ama senin utils kütüphanene bağlı.
        return _characterController.isGrounded;
    }
}