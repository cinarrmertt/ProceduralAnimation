using UnityEngine;

public class ApproachState : EnviromentInteractionState
{
    private float _elapsedTime = 0;
    private float _lerpDuration = 5;
    private float _approachDuration = 2f;
    private float _approachWeight = .5f;
    private float _rotationSpeed = 500f;
    private float _approachRotationWeight = .75f;
    private float _riseDistanceThreshold = .5f;
    public ApproachState(EnviromentInteractionContext context, 
        EnviromentInteractionStateMachine.EEnviromentInteractionState eState) : base(context, eState)
    {
        EnviromentInteractionContext Context = context;
    }

    public override void EnterState()
    {
        Debug.Log("Enter Approach State");
        _elapsedTime = 0;
    }
    public override void ExitState(){}

    public override void UpdateState()
    {
        Quaternion expectedGroundRotation=Quaternion.LookRotation(-Vector3.up,
            Context.RootTransform.forward);
        _elapsedTime += Time.deltaTime;

        Context.currentIKTargetTransform.rotation =
            Quaternion.RotateTowards(Context.currentIKTargetTransform.rotation,
                expectedGroundRotation, _rotationSpeed * Time.deltaTime);
        
        Context.currentMultiRotationConstraint.weight = Mathf.Lerp(Context.currentMultiRotationConstraint.weight, _approachRotationWeight,
            _elapsedTime / _lerpDuration);
        
        Context.currentIKConstraint.weight = Mathf.Lerp(Context.currentIKConstraint.weight, _approachWeight,
            _elapsedTime / _lerpDuration);
        
    }

    public override EnviromentInteractionStateMachine.EEnviromentInteractionState GetNextState()
    {
        bool isOverStateLifeDuration = _elapsedTime >= _approachDuration;
        if (isOverStateLifeDuration ||  CheckShouldReset())
        {
            return EnviromentInteractionStateMachine.EEnviromentInteractionState.Reset;
        }
         
        bool isWithinArmsReach = Vector3.Distance(Context.ClosestPointOnColliderFromShoulder,
            Context.currentShoulderTransform.position) < _riseDistanceThreshold;

        bool isClosestPointOnColliderReal = Context.ClosestPointOnColliderFromShoulder != Vector3.positiveInfinity;

        if (isClosestPointOnColliderReal && isWithinArmsReach)
        {
            return EnviromentInteractionStateMachine.EEnviromentInteractionState.Rise;
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
