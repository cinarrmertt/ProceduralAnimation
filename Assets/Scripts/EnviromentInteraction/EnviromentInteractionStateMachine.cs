using System;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Animations.Rigging;

public class EnviromentInteractionStateMachine : StateManager<EnviromentInteractionStateMachine.
   EEnviromentInteractionState>
{
   public enum EEnviromentInteractionState
   {
      Search,
      Approach,
      Rise,
      Touch,
      Reset,
   }
   
   private EnviromentInteractionContext _context;
   
   [SerializeField] private TwoBoneIKConstraint _leftIKConstraint;
   [SerializeField] private TwoBoneIKConstraint _rightIKConstraint;
   [SerializeField] private MultiRotationConstraint _leftMultiRotationConstraint;
   [SerializeField] private MultiRotationConstraint _rightMultiRotationConstraint;
   [SerializeField] private CharacterController _characterController;

   private void OnDrawGizmos()
   {
      Gizmos.color = Color.red;

      if (_context != null && _context.ClosestPointOnColliderFromShoulder != null)
      {
         Gizmos.DrawSphere(_context.ClosestPointOnColliderFromShoulder, 0.03f);
      }
   }

   private void Awake()
   {
      ValidateConstraints();

      _context = new EnviromentInteractionContext(_leftIKConstraint, _rightIKConstraint, 
         _leftMultiRotationConstraint,_rightMultiRotationConstraint, _characterController,transform.root);
      
      ConstructEnviromentDetectCollider();
      InitializeStates();
   }

   private void ValidateConstraints()
   {
      Assert.IsNotNull(_leftIKConstraint, "Left Hand IK Constraint is not assigned.");
      Assert.IsNotNull(_rightIKConstraint, "Right Hand IK Constraint is not assigned.");
      Assert.IsNotNull(_leftMultiRotationConstraint, "Left Multi Rotation Constraint is not assigned.");
      Assert.IsNotNull(_rightMultiRotationConstraint, "Right Multi Rotation Constraint is not assigned.");
      Assert.IsNotNull(_characterController, "Character Controller is not assigned.");
   }

   void InitializeStates()
   {
      states.Add(EEnviromentInteractionState.Reset,new ResetState(_context,EEnviromentInteractionState.Reset));
      states.Add(EEnviromentInteractionState.Search,new SearchState(_context,EEnviromentInteractionState.Search));
      states.Add(EEnviromentInteractionState.Approach,new ApproachState(_context,EEnviromentInteractionState.Approach));
      states.Add(EEnviromentInteractionState.Rise,new RiseState(_context,EEnviromentInteractionState.Rise));
      states.Add(EEnviromentInteractionState.Touch,new TouchState(_context,EEnviromentInteractionState.Touch));
      currentState = states[EEnviromentInteractionState.Reset];
   }

   private void ConstructEnviromentDetectCollider()
   {
      float wingspan = _characterController.height;
      
      BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
      boxCollider.size = new Vector3(wingspan, wingspan, wingspan);
      boxCollider.center = new Vector3(_characterController.center.x,
         _characterController.center.y + (.25f*wingspan),
         _characterController.center.z + (.5f*wingspan));
      boxCollider.isTrigger = true;
   }
}
