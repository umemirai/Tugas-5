using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BearAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolRadius = 10f;
    public float waitTime = 2f;

    [Header("Detection Settings")]
    public float detectionRange = 8f;      // Jarak beruang mulai mengejar
    public float stopChasingRange = 12f;   // Jarak beruang berhenti mengejar
    public Transform target;               // Referensi ke Player

    [Header("Spawner Settings")]
    public GameObject bearPrefab;     
    public float spawnInterval = 10f;  
    public int maxBearsNear = 3;      

    private NavMeshAgent agent;
    private Animator anim; 
    private float patrolTimer;
    private Vector3 spawnPoint;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>(); 
        spawnPoint = transform.position;

        // Mencari Player secara otomatis jika belum di-assign di Inspector
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
        }

        SetRandomDestination();

        if (bearPrefab != null)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    void Update()
    {
        UpdateAnimation();

        if (!agent.isOnNavMesh) return;

        float distanceToPlayer = target != null ? Vector3.Distance(transform.position, target.position) : Mathf.Infinity;

        // Logika Perpindahan State
        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }
        else if (distanceToPlayer > stopChasingRange)
        {
            isChasing = false;
        }

        // Eksekusi State
        if (isChasing && target != null)
        {
            ChasePlayer();
        }
        else
        {
            PatrolLogic();
        }
    }

    void UpdateAnimation()
    {
        if (anim != null)
        {
            // Menggunakan velocity.magnitude agar animasi sinkron dengan kecepatan gerak AI
            float speed = agent.velocity.magnitude;
            anim.SetFloat("Speed", speed); 
        }
    }

    void PatrolLogic()
    {
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

    void ChasePlayer()
    {
        // Beruang akan mencari jalan terpintar menuju lokasi player
        agent.SetDestination(target.position);
    }

    // --- SISANYA ADALAH LOGIKA SPAWNER KAMU YANG SEBELUMNYA ---

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