using UnityEngine;

public class SearchState : EnviromentInteractionState
{
    public SearchState(EnviromentInteractionContext context, 
        EnviromentInteractionStateMachine.EEnviromentInteractionState eState) : base(context, eState)
    {
        EnviromentInteractionContext Context = context;
    }
    
    public override void EnterState(){}
    public override void ExitState(){}
    public override void UpdateState(){}

    public override EnviromentInteractionStateMachine.EEnviromentInteractionState GetNextState()
    {
        return StateKey;
    }

    public override void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        StartIKTargetPositionTracking(other);
    }

    public override void OnTriggerStay(Collider other)
    {
        UpdateIKTargetPosition(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        StopIKTargetPositionTracking(other);
    }
}
