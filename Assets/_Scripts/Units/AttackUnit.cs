using System.Collections;
using Photon.Pun;
using UnityEngine;

public class AttackUnit : BaseUnit
{
    [Header("Attack attributes")]
    public float damage;
    public float attackRange;
    public float attackSpeed;

    [SerializeField]
    private Transform shootPoint;
    [SerializeField] GameObject projectilePrefab;
    
    private Transform target;
    private bool isAttacking = false;

    private Coroutine currentSequence;

    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();

        if (isAttacking)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler((target.position - transform.position).normalized), turnSpeed * Time.deltaTime);
        }
        
        CheckForEnemy();
    }
    protected override void OnDeath()
    {
        // VFX for death

        if (currentSequence != null)
        {
            StopCoroutine(currentSequence);
        }
        base.OnDeath();
    }

    void CheckForEnemy()
    {
        // Check for enemies in the area
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask(new string[]
        {
            "Units",
            "Buildings",
        }));
        foreach (Collider hitCollider in hitColliders)
        {
            if (!hitCollider.GetComponent<PhotonView>().IsMine)
            {
                // Attack the enemy
                currentSequence = StartCoroutine(Attack(hitCollider.GetComponent<IDestroyable>()));
            }
        }
    }
    IEnumerator Attack(IDestroyable destroyable)
    {
        isAttacking = true;
        while (true)
        {
            GameObject go = PhotonNetwork.Instantiate(projectilePrefab.name, shootPoint.forward, Quaternion.Euler(shootPoint.forward));
            go.GetComponent<ProjectileScript>().damage = damage;
            if (target.GetComponent<BaseUnit>().health <= 0)
            {
                isAttacking = false;
                StopCoroutine(currentSequence);
            }
            yield return new WaitForSeconds(attackSpeed);
        }
    }
}
