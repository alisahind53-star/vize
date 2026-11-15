using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    [Header("Referanslar")]
    [Tooltip("Main Camera objesini buraya sürükleyin")]
    [SerializeField] private Transform playerCamera;

    [Tooltip("Main Camera altında oluşturduğunuz boş HoldPoint objesini buraya sürükleyin")]
    [SerializeField] private Transform holdPoint;

    [Tooltip("Karakterin kendi Collider/CharacterController bileşeni (nesnenin size çarpmasını önler)")]
    [SerializeField] private Collider playerCollider;

    [Header("Tutma & Fırlatma Ayarları")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private float pickupRange = 3.5f;
    [SerializeField] private float followSpeed = 18f;
    [SerializeField] private float maxHoldDistance = 5f;
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private LayerMask pickableLayer = ~0;

    private Rigidbody heldRigidbody;
    private Collider heldCollider;
    private float originalDamping;
    private RigidbodyInterpolation originalInterpolation;

    private void Awake()
    {
        if (playerCollider == null)
        {
            playerCollider = GetComponent<Collider>();
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        // F ile tut veya momentumunu koruyarak bırak (dönüp bırakınca uçar)
        if (Input.GetKeyDown(interactKey))
        {
            if (heldRigidbody == null)
            {
                TryPickup();
            }
            else
            {
                Drop(applyThrow: false);
            }
        }

        // Tutarken Sol Tık ile doğrudan ileri fırlat
        if (heldRigidbody != null && Input.GetMouseButtonDown(0))
        {
            Drop(applyThrow: true);
        }
    }

    private void FixedUpdate()
    {
        if (heldRigidbody == null || holdPoint == null) return;

        float distance = Vector3.Distance(heldRigidbody.position, holdPoint.position);
        if (distance > maxHoldDistance)
        {
            Drop(applyThrow: false);
            return;
        }

        // Fizik motoruyla hedefe çek: böylece duvarlara çarpar ve dönüş hızını (momentum) korur
        Vector3 directionToTarget = holdPoint.position - heldRigidbody.position;
        Vector3 targetVelocity = directionToTarget * followSpeed;

#if UNITY_6000_0_OR_NEWER
        heldRigidbody.linearVelocity = targetVelocity;
#else
        heldRigidbody.velocity = targetVelocity;
#endif
    }

    private void TryPickup()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickableLayer))
        {
            if (hit.collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                heldRigidbody = rb;
                heldCollider = hit.collider;

                heldRigidbody.useGravity = false;
                heldRigidbody.isKinematic = false;

                // Kamera FPS'i ile fizik FPS'i arasındaki titreşimi (kasma hissini) yok eder
                originalInterpolation = heldRigidbody.interpolation;
                heldRigidbody.interpolation = RigidbodyInterpolation.Interpolate;

#if UNITY_6000_0_OR_NEWER
                originalDamping = heldRigidbody.linearDamping;
                heldRigidbody.linearDamping = 10f;
#else
                originalDamping = heldRigidbody.drag;
                heldRigidbody.drag = 10f;
#endif

                if (playerCollider != null && heldCollider != null)
                {
                    Physics.IgnoreCollision(heldCollider, playerCollider, true);
                }
            }
        }
    }

    private void Drop(bool applyThrow)
    {
        if (heldRigidbody == null) return;

        heldRigidbody.useGravity = true;
        heldRigidbody.interpolation = originalInterpolation;

#if UNITY_6000_0_OR_NEWER
        heldRigidbody.linearDamping = originalDamping;
#else
        heldRigidbody.drag = originalDamping;
#endif

        if (playerCollider != null && heldCollider != null)
        {
            Physics.IgnoreCollision(heldCollider, playerCollider, false);
        }

        if (applyThrow && playerCamera != null)
        {
            heldRigidbody.AddForce(playerCamera.forward * throwForce, ForceMode.Impulse);
        }

        heldRigidbody = null;
        heldCollider = null;
    }
}