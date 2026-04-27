using UnityEngine;

namespace ThirdPerson.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class Move : MonoBehaviour
    {
        [SerializeField] protected float moveSpeed = 5f;
        protected Rigidbody body;
        protected Vector3 currentInput;

        protected virtual void Awake()
        {
            body = GetComponent<Rigidbody>();
            
            // Sangat penting untuk 3D agar karakter tidak menggelinding
            body.constraints = RigidbodyConstraints.FreezeRotation; 
        }
    }
}