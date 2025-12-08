using UnityEngine;

public class SearchState : EnviromentInteractionState
{
    public float _approachDistanceThreshold = 2;
    public SearchState(EnviromentInteractionContext context, 
        EnviromentInteractionStateMachine.EEnviromentInteractionState eState) : base(context, eState)
    {
        EnviromentInteractionContext Context = context;
    }

    public override void EnterState()
    {
        Debug.Log("Entered Search State");
    }
    public override void ExitState(){}
    public override void UpdateState(){}

    public override EnviromentInteractionStateMachine.EEnviromentInteractionState GetNextState()
    {
        if (CheckShouldReset())
        {
            return EnviromentInteractionStateMachine.EEnviromentInteractionState.Reset;
        }
        
        bool isCloseToTarget=Vector3.Distance(Context.ClosestPointOnColliderFromShoulder,
            Context.RootTransform.position) < _approachDistanceThreshold;

        bool isClosesPointOnColliderValid = Context.ClosestPointOnColliderFromShoulder != Vector3.positiveInfinity;

        if (isClosesPointOnColliderValid && isCloseToTarget)
        {
            return EnviromentInteractionStateMachine.EEnviromentInteractionState.Approach;
        }
        return StateKey;
    }

    public override void OnTriggerEnter(Collider other)
    {
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
