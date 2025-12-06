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
   
   [SerializeField] private TwoBoneIKConstraint _leftHandIKConstraint;
   [SerializeField] private TwoBoneIKConstraint _rightHandIKConstraint;
   [SerializeField] private MultiRotationConstraint _leftMultiRotationConstraint;
   [SerializeField] private MultiRotationConstraint _rightMultiRotationConstraint;
   [SerializeField] private CharacterController _characterController;

   private void Awake()
   {
      ValidateConstraints();

      _context = new EnviromentInteractionContext(_leftHandIKConstraint, _rightHandIKConstraint, 
         _leftMultiRotationConstraint,_rightMultiRotationConstraint, _characterController);
      
      InitializeStates();
   }

   private void ValidateConstraints()
   {
      Assert.IsNotNull(_leftHandIKConstraint, "Left Hand IK Constraint is not assigned.");
      Assert.IsNotNull(_rightHandIKConstraint, "Right Hand IK Constraint is not assigned.");
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
}
