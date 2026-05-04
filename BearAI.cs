using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BearAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolRadius = 10f;
    public float waitTime = 2f;

    [Header("Spawner Settings")]
    public GameObject bearPrefab;     
    public float spawnInterval = 10f;  
    public int maxBearsNear = 3;      

    private NavMeshAgent agent;
    private Animator anim; // Tambahan untuk animasi
    private float patrolTimer;
    private Vector3 spawnPoint;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>(); // Mengambil komponen Animator
        spawnPoint = transform.position;
        SetRandomDestination();

        if (bearPrefab != null)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    void Update()
    {
        // Update animasi berdasarkan kecepatan NavMeshAgent
        if (anim != null)
        {
            float speed = agent.velocity.magnitude;
            anim.SetFloat("Speed", speed); // Pastikan nama parameter di Animator adalah "Speed"
        }

        if (!agent.isOnNavMesh) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= waitTime)
            {
                SetRandomDestination();
                patrolTimer = 0f;
            }
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true) 
        {
            yield return new WaitForSeconds(spawnInterval);
            
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 15f);
            int bearCount = 0;
            foreach (var hit in hitColliders)
            {
                if (hit.CompareTag("Enemy") || hit.name.Contains("Bear")) bearCount++;
            }

            if (bearCount < maxBearsNear)
            {
                SpawnNewBear();
            }
        }
    }

    void SpawnNewBear()
    {
        Vector3 randomPos = Random.insideUnitSphere * 3f;
        randomPos += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 5f, NavMesh.AllAreas))
        {
            Instantiate(bearPrefab, hit.position, Quaternion.identity);
        }
    }

    void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += spawnPoint;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }
}