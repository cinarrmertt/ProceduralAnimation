using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EnviromentInteractionContext
{
    public enum EBodySide
    {
        Right,
        Left
    }
    private TwoBoneIKConstraint _leftIKConstraint; 
    private TwoBoneIKConstraint _rightIKConstraint; 
    private MultiRotationConstraint _leftMultiRotationConstraint; 
    private MultiRotationConstraint _rightMultiRotationConstraint; 
    private CharacterController _characterController;
    private Transform _rootTransform;
    
    public EnviromentInteractionContext(TwoBoneIKConstraint leftIKConstraint, 
        TwoBoneIKConstraint rightIKConstraint,
        MultiRotationConstraint leftMultiRotationConstraint,
        MultiRotationConstraint rightMultiRotationConstraint,
        CharacterController characterController,Transform rootTransform)
    {
        _leftIKConstraint = leftIKConstraint;
        _rightIKConstraint = rightIKConstraint;
        _leftMultiRotationConstraint = leftMultiRotationConstraint;
        _rightMultiRotationConstraint = rightMultiRotationConstraint;
        _characterController= characterController;        
        _rootTransform = rootTransform;
        
        characterShoulderHeight = leftIKConstraint.data.root.transform.position.y;
    }
    public TwoBoneIKConstraint LeftIKConstraint => _leftIKConstraint;
    public TwoBoneIKConstraint RighIKConstraint => _rightIKConstraint;
    public MultiRotationConstraint LeftMultiRotationConstraint => _leftMultiRotationConstraint;
    public MultiRotationConstraint RightMultiRotationConstraint => _rightMultiRotationConstraint;
    public CharacterController CharacterController => _characterController;
    public Transform RootTransform => _rootTransform;
    
    public float characterShoulderHeight { get; private set; }
    public Collider currentIntersectingCollider { get; set; }
    public TwoBoneIKConstraint currentIKConstraint { get; private set; }
    public MultiRotationConstraint currentMultiRotationConstraint { get; private set; }
    public Transform currentIKTargetTransform { get; private set; }
    public Transform currentShoulderTransform { get; private set; }
    public EBodySide currentBodySide { get; private set; }
    public Vector3 ClosestPointOnColliderFromShoulder { get; set; } = Vector3.positiveInfinity;

    public void SetCurrentSide(Vector3 positionToCheck)
    {
        
        Vector3 leftShoulder = _leftIKConstraint.data.root.transform.position;
        Vector3 rightShoulder = _rightIKConstraint.data.root.transform.position;
        
        bool isLeftCloser=Vector3.Distance(positionToCheck,leftShoulder) < Vector3.Distance(positionToCheck,rightShoulder);

        if (isLeftCloser)
        {
            Debug.Log("Left side is closer");
            currentBodySide = EBodySide.Left;
            currentIKConstraint = _leftIKConstraint;
            currentMultiRotationConstraint = _leftMultiRotationConstraint;
        }
        else
        {
            Debug.Log("Right side is closer");
            currentBodySide = EBodySide.Right;
            currentIKConstraint = _rightIKConstraint;
            currentMultiRotationConstraint = _rightMultiRotationConstraint;
        }
        currentShoulderTransform=currentIKConstraint.data.root.transform;
        currentIKTargetTransform=currentIKConstraint.data.target.transform;
    }
}
