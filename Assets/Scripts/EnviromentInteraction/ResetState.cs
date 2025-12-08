using UnityEngine;

public class ResetState : EnviromentInteractionState
{
    private float _elapsedTime = 0;
    private float _resetDuration = 2.0f;
    private float _lerpDuration = 10f;
    private float _rotationSpeed = 500;
    public ResetState(EnviromentInteractionContext context, 
        EnviromentInteractionStateMachine.EEnviromentInteractionState eState) : base(context, eState)
    {
        EnviromentInteractionContext Context = context;
    }

    public override void EnterState()
    {
        _elapsedTime = 0;
        Context.ClosestPointOnColliderFromShoulder = Vector3.positiveInfinity;
        Context.currentIntersectingCollider = null;

    }
    public override void ExitState(){}

    public override void UpdateState()
    {
        _elapsedTime += Time.deltaTime;
        
        Context.InteractionPointYOffset = Mathf.Lerp(Context.InteractionPointYOffset,
            Context.ColliderCenterY, _elapsedTime / _lerpDuration);
        Context.currentIKConstraint.weight=Mathf.Lerp(Context.currentIKConstraint.weight,
            0,_elapsedTime / _lerpDuration);
        Context.currentMultiRotationConstraint.weight =Mathf.Lerp(Context.currentMultiRotationConstraint.weight,
            0,_elapsedTime / _lerpDuration);

        Context.currentIKTargetTransform.localPosition = Vector3.Lerp(Context.currentIKTargetTransform.localPosition,
            Context.currentOriginalTargetPosition, _elapsedTime / _lerpDuration);
        Context.currentIKTargetTransform.rotation = Quaternion.RotateTowards(Context.currentIKTargetTransform.rotation,
            Context.OriginalTargetRotation, _rotationSpeed * Time.deltaTime);
    }

    public override EnviromentInteractionStateMachine.EEnviromentInteractionState GetNextState()
    {
        bool isMoving = Context.CharacterController.velocity.magnitude > 0.1f;
        if(_elapsedTime >= _resetDuration && isMoving)
        {
            return EnviromentInteractionStateMachine.EEnviromentInteractionState.Search;
        }
        
        return StateKey;

    }

    public override void OnTriggerEnter(Collider other){}
    public override void OnTriggerStay(Collider other){}
    public override void OnTriggerExit(Collider other){}
    
}
