using System.Collections;
using UnityEngine;

public class CorruptedShard : Enemy
{
    [Header("Shard Settings")]
    [SerializeField] private float chaseDelay = 0.4f;
    [SerializeField] private float attackRange = 1.4f;
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private float stopDistance = 1.2f;   // NEW: prevents overlap

    private bool isChasing;
    private bool isAttacking;

    private int enemyLayer;
    private int playerLayer;

    protected override void Awake()
    {
        base.Awake();

        enemyLayer = LayerMask.NameToLayer("CorruptedShard");
        playerLayer = LayerMask.NameToLayer("Player");

        // Prevent physics pushback between shard and player
        Physics.IgnoreLayerCollision(enemyLayer, playerLayer, true);
    }

    protected override void EnemyBehavior()
    {
        if (target == null)
        {
            isChasing = false;
            isAttacking = false;
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        // Start chase after delay
        if (!isChasing)
        {
            StartCoroutine(StartChaseRoutine());
            return;
        }

        // Stop in front of the player
        if (distance > stopDistance)
        {
            MoveTowardsTarget(speed);
        }

        // Attack when close
        if (distance <= attackRange && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator StartChaseRoutine()
    {
        isChasing = true;
        yield return new WaitForSeconds(chaseDelay);
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        IDamageable dmg = target.GetComponent<IDamageable>();

        while (target != null &&
               Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            dmg?.Damage(damage);
            yield return new WaitForSeconds(attackInterval);
        }

        isAttacking = false;
    }

    private void OnDisable()
    {
        // Restore collision if object is disabled/destroyed
        Physics.IgnoreLayerCollision(enemyLayer, playerLayer, false);
    }
}