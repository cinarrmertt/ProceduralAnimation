using UnityEngine;

public class TouchState : EnviromentInteractionState
{
    private float _elapsedTime = 0;
    private float _resetThreshold = 0.5f;
    
    public TouchState(EnviromentInteractionContext context, 
        EnviromentInteractionStateMachine.EEnviromentInteractionState eState) : base(context, eState)
    {
        EnviromentInteractionContext Context = context;
    }

    public override void EnterState()
    {
        _elapsedTime = 0;
    }
    public override void ExitState(){}

    public override void UpdateState()
    {
        _elapsedTime += Time.deltaTime;
    }

    public override EnviromentInteractionStateMachine.EEnviromentInteractionState GetNextState()
    {
        if (_elapsedTime>_resetThreshold || CheckShouldReset())
        {
            return EnviromentInteractionStateMachine.EEnviromentInteractionState.Reset;
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
