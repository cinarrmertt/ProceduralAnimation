using UnityEngine;

public abstract class EnviromentInteractionState : BaseState<EnviromentInteractionStateMachine.
    EEnviromentInteractionState>
{
   protected EnviromentInteractionContext _context;
   
   public EnviromentInteractionState(EnviromentInteractionContext context,
       EnviromentInteractionStateMachine.EEnviromentInteractionState stateKey) : base(stateKey)
   {
      _context = context;
   }
}
