using UnityEngine;

public class RiseState : EnviromentInteractionState
{
    private float _elapsedTime = 0;
    private float _lerpDuration = 5;
    private float _riseWeight = 1f;
    Quaternion _expectedHandRotation;
    private float _maxDistance = 0.5f;
    protected LayerMask _interactableLayerMask=LayerMask.GetMask("Interactable");
    float _rotationSpeed = 500f;
    private float _touchDistanceThreshold = .05f;
    private float _touchTimeThreshold = 1;
    
    public RiseState(EnviromentInteractionContext context, 
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
        CalculateExpectedHandRotation();
        
        Context.InteractionPointYOffset = Mathf.Lerp(Context.InteractionPointYOffset,
            Context.ClosestPointOnColliderFromShoulder.y, _elapsedTime / _lerpDuration);

        Context.currentIKConstraint.weight =
            Mathf.Lerp(Context.currentIKConstraint.weight, _riseWeight, _elapsedTime / _lerpDuration);

        Context.currentMultiRotationConstraint.weight = Mathf.Lerp(Context.currentMultiRotationConstraint.weight,
            _riseWeight, _elapsedTime / _lerpDuration);

        Context.currentIKTargetTransform.rotation = Quaternion.RotateTowards(Context.currentIKTargetTransform.rotation,
            _expectedHandRotation, _rotationSpeed * Time.deltaTime);
        
        _elapsedTime += Time.deltaTime;
    }

    void CalculateExpectedHandRotation()
    {
        Vector3 startPos = Context.currentShoulderTransform.position;
        Vector3 endPos = Context.ClosestPointOnColliderFromShoulder;
        Vector3 direction = (endPos - startPos).normalized;

        RaycastHit hit;
        if (Physics.Raycast(startPos,direction,out hit,_maxDistance,_interactableLayerMask))
        {
            Vector3 surfaceNormal = hit.normal;
            Vector3 targetForward = -surfaceNormal;
            _expectedHandRotation = Quaternion.LookRotation(targetForward,Vector3.up);
        }
    }

    public override EnviromentInteractionStateMachine.EEnviromentInteractionState GetNextState()
    {
        if (CheckShouldReset())
        {
            return EnviromentInteractionStateMachine.EEnviromentInteractionState.Reset;
        }
        
        if (Vector3.Distance(Context.currentIKTargetTransform.position , Context.ClosestPointOnColliderFromShoulder) < 
            _touchDistanceThreshold && _elapsedTime >= _touchTimeThreshold)
        {
            return EnviromentInteractionStateMachine.EEnviromentInteractionState.Touch;
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
