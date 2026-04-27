using UnityEngine;
using UnityEngine.InputSystem;

namespace ThirdPerson.Movement
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerMovement : Move 
    {
        private Animator _anim;
        private Vector2 mouseInput; 

        protected override void Awake()
        {
            base.Awake(); 
            _anim = GetComponentInChildren<Animator>();
        }

        private void OnMove(InputValue value)
        {
            Vector2 input = value.Get<Vector2>();
            currentInput = new Vector3(input.x, 0f, input.y); 
        }

        // --- ALARM 1: CEK INPUT MOUSE ---
        private void OnLook(InputValue value)
        {
            mouseInput = value.Get<Vector2>();
            // Kalau ini muncul di Console, berarti mouse kamu nyambung!
            Debug.Log("1. MOUSE MASUK: " + mouseInput);
        }

        private void FixedUpdate()
        {
         if (Camera.main != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(mouseInput);
                
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f))
                {
                    // Cari arah dari karakter ke titik laser
                    Vector3 aimDirection = hitInfo.point - transform.position;
                    aimDirection.y = 0f; // Kunci sumbu Y biar nggak nunduk/dangak

                    // Kalau jarak kursor cukup jauh, baru berputar
                    if (aimDirection.magnitude > 0.5f)
                    {
                        // Putar menggunakan sistem Rigidbody (MoveRotation) agar mulus dan tak bentrok
                        Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
                        body.MoveRotation(targetRotation);
                    }
                }
            }

            // --- URUSAN JALAN ---
            if (currentInput.magnitude > 0.1f)
            {
                Vector3 velocity = currentInput * moveSpeed;
                velocity.y = body.velocity.y; 
                body.velocity = velocity;
                if (_anim != null) _anim.SetFloat("Speed", 5f, 0.1f, Time.fixedDeltaTime);
            }
            else
            {
                body.velocity = new Vector3(0f, body.velocity.y, 0f);
                if (_anim != null) _anim.SetFloat("Speed", 0f, 0.1f, Time.fixedDeltaTime);
            }
    }
    }
}