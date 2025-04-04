using UnityEngine;

public class Commander : BaseUnit
{
    [Header("Attack attributes")]
    public float damage;
    public float attackRange;
    
    [Header("Build attributes")]
    public GameObject building;
    public float buildRange;
    public float buildSpeed;
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }
}
