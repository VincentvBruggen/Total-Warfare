using System;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Check if Variable is Null", story: "[Variable] is Null", category: "Variable Conditions", id: "13d776912d6e61dca7fa1dc3f29c5d1b")]
public partial class CheckIfVariableIsNullCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Variables> Variable;

    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
        if (Variable.Value == null)
        {
            this.IsTrue();
        }
    }

    public override void OnEnd()
    {
    }
}
