using UnityEngine;

public class ResetState : EnviromentInteractionState
{
    public ResetState(EnviromentInteractionContext context, 
        EnviromentInteractionStateMachine.EEnviromentInteractionState eState) : base(context, eState)
    {
        EnviromentInteractionContext Context = context;
    }

    public override void EnterState()
    {
    }
    public override void ExitState(){}

    public override void UpdateState()
    {
    }

    public override EnviromentInteractionStateMachine.EEnviromentInteractionState GetNextState()
    {
        return EnviromentInteractionStateMachine.EEnviromentInteractionState.Search;
    }

    public override void OnTriggerEnter(Collider other){}
    public override void OnTriggerStay(Collider other){}
    public override void OnTriggerExit(Collider other){}
    
}
