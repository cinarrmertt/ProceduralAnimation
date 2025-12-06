using UnityEngine;

public class RiseState : EnviromentInteractionState
{
    public RiseState(EnviromentInteractionContext context, 
        EnviromentInteractionStateMachine.EEnviromentInteractionState stateKey) : base(context, stateKey)
    {
        EnviromentInteractionContext _context = context;
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
