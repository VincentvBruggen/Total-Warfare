using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Commander : BaseUnit
{
    [Header("Attack attributes")]
    public float damage;
    public float attackRange;
    public float attackSpeed;
    [SerializeField] Transform shootPoint;
    [SerializeField] GameObject projectilePrefab;
    
    private Transform target;
    
    [Header("Build attributes")]
    public GameObject building;
    public float buildRange;
    public float buildSpeed;

    private Coroutine currentSequence;

    private bool isAttacking;
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

    void CheckForEnemy()
    {
        if (isAttacking)
        {
            return;
        }
        // Check for enemies in the area
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask(new string[]
        {
            "Units",
            "Buildings",
        }));
        foreach (Collider hitCollider in hitColliders)
        {
            if (!hitCollider.GetComponent<PhotonView>().IsMine && hitCollider.GetComponent<IDestroyable>() != null)
            {
                target = hitCollider.transform;
                // Attack the enemy
                currentSequence = StartCoroutine(Attack(hitCollider.GetComponent<IDestroyable>()));
                return;
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
