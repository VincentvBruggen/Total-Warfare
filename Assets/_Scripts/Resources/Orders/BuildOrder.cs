using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BuildOrder : OrderBase
{
    ConstructionUnit constructionUnit;
    Commander commander;

    private bool isBuilding = false;
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
        }
        
        agent.SetDestination(targetPosition);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
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
        // Debug.Log("Building");
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.position - targetPosition, out hit))
        {
            BuildingBase buildingBase = hit.collider.GetComponent<BuildingBase>();
            if (buildingBase != null)
            {
                // while (buildingBase.)
                // {
                //     
                // }
            }
        }
        
        yield return null;
    }
}
