using UnityEngine;

public class TouchState : EnviromentInteractionState
{
    public TouchState(EnviromentInteractionContext context, 
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

    public override void OnTriggerEnter(Collider other){}
    public override void OnTriggerStay(Collider other){}
    public override void OnTriggerExit(Collider other){}
}
