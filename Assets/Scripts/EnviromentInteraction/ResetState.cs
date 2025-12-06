using UnityEngine;

public class ResetState : EnviromentInteractionState
{
    public ResetState(EnviromentInteractionContext context, 
        EnviromentInteractionStateMachine.EEnviromentInteractionState stateKey) : base(context, stateKey)
    {
        EnviromentInteractionContext _context = context;
    }

    public override void EnterState()
    {
        Debug.Log("Entering state ");
    }
    public override void ExitState(){}

    public override void UpdateState()
    {
        Debug.Log("Updating state ");
    }

    public override EnviromentInteractionStateMachine.EEnviromentInteractionState GetNextState()
    {
        return StateKey;
    }

    public override void OnTriggerEnter(Collider other){}
    public override void OnTriggerStay(Collider other){}
    public override void OnTriggerExit(Collider other){}
    
}
