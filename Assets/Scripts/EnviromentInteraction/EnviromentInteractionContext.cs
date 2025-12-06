using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EnviromentInteractionContext
{ 
    private TwoBoneIKConstraint _leftHandIKConstraint; 
    private TwoBoneIKConstraint _rightHandIKConstraint; 
    private MultiRotationConstraint _leftMultiRotationConstraint; 
    private MultiRotationConstraint _rightMultiRotationConstraint; 
    private CharacterController _characterController;
    
    public EnviromentInteractionContext(TwoBoneIKConstraint leftHandIKConstraint, 
        TwoBoneIKConstraint rightHandIKConstraint,
        MultiRotationConstraint leftMultiRotationConstraint,
        MultiRotationConstraint rightMultiRotationConstraint,
        CharacterController characterController)
    {
        _leftHandIKConstraint = leftHandIKConstraint;
        _rightHandIKConstraint = rightHandIKConstraint;
        _leftMultiRotationConstraint = _leftMultiRotationConstraint;
        _rightMultiRotationConstraint = _rightMultiRotationConstraint;
        _characterController=_characterController;        
    }
    public TwoBoneIKConstraint LeftHandIKConstraint => _leftHandIKConstraint;
    public TwoBoneIKConstraint RightHandIKConstraint => _rightHandIKConstraint;
    public MultiRotationConstraint LeftMultiRotationConstraint => _leftMultiRotationConstraint;
    public MultiRotationConstraint RightMultiRotationConstraint => _rightMultiRotationConstraint;
    public CharacterController CharacterController => _characterController;
}
