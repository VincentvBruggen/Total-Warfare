using System;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Compare with null", story: "[Variable] is [Comparison] Null", category: "Variable Conditions", id: "213d4fb2c88b94e33ca1ed71e0a3a933")]
public partial class CompareWithNullCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Variables> Variable;
    [Comparison(comparisonType: ComparisonType.Boolean)]
    [SerializeReference] public BlackboardVariable<ConditionOperator> Comparison;

    public override bool IsTrue()
    {
        if (this.Comparison.Value == ConditionOperator.Equal)
        {
            if (this.Variable.Value == null)
            {
                return true;
            }
        }
        else if (this.Comparison.Value == ConditionOperator.NotEqual)
        {
            if (this.Variable.Value != null)
            {
                return true;
            }
        }
        return false;
    }

    public override void OnStart()
    {
        Debug.Log("OnStart");
    }


    public override void OnEnd()
    {
    }
}
