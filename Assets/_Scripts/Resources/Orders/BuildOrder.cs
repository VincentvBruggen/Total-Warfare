using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.Behavior;

public class BuildOrder : OrderBase
{
    ConstructionUnit constructionUnit;
    Commander commander;

    private float buildSpeed = 0;
    private float buildRange = 0;

    private bool isBuilding = false;
    
    protected override void Awake()
    {
        orderState = UnitState.Build;
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        constructionUnit = baseUnit.GetComponent<ConstructionUnit>();
        if(constructionUnit == null)
        {
            commander = baseUnit.GetComponent<Commander>();
            buildSpeed = commander.buildSpeed;
            buildRange = commander.buildRange;
        }
        else
        {
            buildSpeed = constructionUnit.buildSpeed;
            buildRange = constructionUnit.buildRange;
        }
        
        agent.SetDestination(targetPosition);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        isBuilding = false;
        agent.isStopped = false;
    }

    protected override void Update()
    {

        if (commander == null && !isBuilding)
        {
            if (Vector3.Distance(transform.position, targetPosition) < constructionUnit.buildRange)
            {
                agent.destination = transform.position;

                StartCoroutine(Build());
            }
        }
        if(constructionUnit == null && !isBuilding)
        {
            if (Vector3.Distance(transform.position, targetPosition) < commander.buildRange)
            {
                agent.destination = transform.position;

                StartCoroutine(Build());
            }
        }
        
        base.Update();
    }

    private IEnumerator Build()
    {
        isBuilding = true;
        
        Debug.DrawRay(transform.position, targetPosition-transform.position, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, targetPosition - transform.position, out hit, buildRange + 0.75f))
        {
            BuildingBase buildingBase = hit.transform.parent.GetComponent<BuildingBase>();
            if (buildingBase != null)
            {
                Debug.Log("Building: " + buildingBase.name);
                while (buildingBase.buildProgress < buildingBase.metalCost)
                {
                    buildingBase.buildProgress += buildSpeed * Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                status = Node.Status.Success;
            }
        }
        
        yield return null;
    }
}
