using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchOrb : Enemy
{
    [Header("Glitch Orb settings")]
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargeWindupTime;
    [SerializeField] private float chargeDuration;
    [SerializeField] private float chargeCooldown;
    [SerializeField] private float chargeStopDistance;

    private bool hasDealtChargeDamage;
    private bool isCharging;
    private float lastChargeTime;
    private Vector3 chargeDirection;
    private Vector3 lastKnownTargetPos;
    private IDamageable cachedTargetDamageable;
    private MeshTrail meshTrail;    

    private int enemyLayer;
    private int playerLayer;

    protected override void Awake()
    {
        base.Awake();

        enemyLayer = LayerMask.NameToLayer("GlitchOrb");
        playerLayer = LayerMask.NameToLayer("Player");
        meshTrail = GetComponent<MeshTrail>();
    }

    protected override void EnemyBehavior()
    {
        if (target == null || isCharging)
            return;

        float distance = Vector3.Distance(transform.position, target.position);
    
        if (distance <= detectionRadius && Time.time >= lastChargeTime + chargeCooldown)
        {
            StartCoroutine(Charge());
        }
    }

    private IEnumerator Charge()
    {
        isCharging = true;
        hasDealtChargeDamage = false;
        lastChargeTime = Time.time;

        
        Physics.IgnoreLayerCollision(enemyLayer, playerLayer, true);
        rb.isKinematic = true;
        cachedTargetDamageable = target.GetComponent<IDamageable>();

        // wind up
        yield return new WaitForSeconds(chargeWindupTime);

        meshTrail.StartTrailCoroutine(chargeDuration);
        
        lastKnownTargetPos = target.position;
        chargeDirection = (lastKnownTargetPos - transform.position).normalized;
        
        float timer = 0f;
        float hitDistance = 1.1f;

        while (timer < chargeDuration)
        {
            if (!hasDealtChargeDamage && Vector3.Distance(rb.position, target.position) <= hitDistance)
            {
                cachedTargetDamageable?.Damage(damage);
                hasDealtChargeDamage = true;
            }

            rb.MovePosition(rb.position + chargeDirection * chargeSpeed * Time.fixedDeltaTime);

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        Physics.IgnoreLayerCollision(enemyLayer, playerLayer, false);
        
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        
        isCharging = false;
    }

    private void OnDisable()
    {
        Physics.IgnoreLayerCollision(enemyLayer, playerLayer, false);
    }
}
