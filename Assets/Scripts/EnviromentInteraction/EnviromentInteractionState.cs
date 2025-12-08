using UnityEngine;

public abstract class EnviromentInteractionState : BaseState<EnviromentInteractionStateMachine.
    EEnviromentInteractionState>
{
   protected EnviromentInteractionContext Context;
   private float _movingAwayOffset = .005f;
   bool _shouldReset;
   
   public EnviromentInteractionState(EnviromentInteractionContext context,
       EnviromentInteractionStateMachine.EEnviromentInteractionState stateKey) : base(stateKey)
   {
      Context = context;
   }

   protected bool CheckShouldReset()
   {
       if (_shouldReset)
       {
           Context.LowestDistance = Mathf.Infinity;
           _shouldReset = false;
           return true;
       }
       
       bool isPlayerStopped = Context.CharacterController.velocity == Vector3.zero;
       bool isMovingAway = CheckIsMovingAway();
       bool isBadAngle = CheckIsBadAngle();
       bool isPlayerJumping = Mathf.Round(Context.CharacterController.velocity.y) >= 1;

       if (isPlayerStopped || isMovingAway || isBadAngle || isPlayerJumping)
       {
           Context.LowestDistance = Mathf.Infinity;
           return true;
       }
       return false;
   }

   protected bool CheckIsBadAngle()
   {
       if (Context.currentIntersectingCollider == null)
       {
           return false;
       }

       Vector3 targetDirection = Context.ClosestPointOnColliderFromShoulder - Context.currentShoulderTransform.position;
       Vector3 shoulderDirection = Context.currentBodySide == EnviromentInteractionContext.EBodySide.Right ? 
           Context.RootTransform.right : -Context.RootTransform.right;
       
       float dotProduct = Vector3.Dot(shoulderDirection, targetDirection.normalized);
       bool isBadAngle = dotProduct < 0;
       
       return isBadAngle;
   }

   protected bool CheckIsMovingAway()
   {
       float currentDistanceToTarget=Vector3.Distance(Context.RootTransform.position, Context.ClosestPointOnColliderFromShoulder);

       bool isSearchingForNewInteraction = Context.currentIntersectingCollider == null;
       if (isSearchingForNewInteraction)
       {
           return false;
       }
           
       bool isGettingCloserToTarget = currentDistanceToTarget <= Context.LowestDistance;

       if (isGettingCloserToTarget)
       {
           Context.LowestDistance = currentDistanceToTarget;
           return false;
       }

       bool isMovingAwayFromTarget = currentDistanceToTarget > Context.LowestDistance + _movingAwayOffset;

       if (isMovingAwayFromTarget)
       {
           Context.LowestDistance = Mathf.Infinity;
           return true;
       }
       return false;
   }
   
   private Vector3 GetClosestPointOnCollider(Collider intersectingCollider, Vector3 positionToCheck)
   {
      return intersectingCollider.ClosestPoint(positionToCheck);
   }

   protected void StartIKTargetPositionTracking(Collider intersectingCollider)
   {
       if (intersectingCollider.gameObject.layer == LayerMask.NameToLayer("Interactable") &&
           Context.currentIntersectingCollider ==null)
       {
           Context.currentIntersectingCollider = intersectingCollider;
           Vector3 closestPointFromRoot=GetClosestPointOnCollider(intersectingCollider, Context.RootTransform.position);
           Context.SetCurrentSide(closestPointFromRoot); 
           
           SetIKTargetPosition();
       }
   }

   protected void UpdateIKTargetPosition(Collider intersectingCollider)
   {
       if (intersectingCollider == Context.currentIntersectingCollider)
       {
           SetIKTargetPosition();
       }
   }

   protected void StopIKTargetPositionTracking(Collider intersectingCollider)
   {
       if (intersectingCollider == Context.currentIntersectingCollider)
       {
           Context.currentIntersectingCollider = null;
           Context.ClosestPointOnColliderFromShoulder = Vector3.positiveInfinity;
           _shouldReset = true;
       }
   }
   
   private void SetIKTargetPosition()
   {
       Context.ClosestPointOnColliderFromShoulder = GetClosestPointOnCollider(Context.currentIntersectingCollider,
           new Vector3(Context.currentShoulderTransform.position.x,
               Context.characterShoulderHeight,
               Context.currentShoulderTransform.position.z));

       Vector3 rayDirection = Context.currentShoulderTransform.position - Context.ClosestPointOnColliderFromShoulder;
       Vector3 rayDirectionNormalized = rayDirection.normalized;
       float offsetDistance = 0.05f;
       Vector3 offset = rayDirectionNormalized * offsetDistance;
       
       Vector3 offsetPosition= Context.ClosestPointOnColliderFromShoulder + offset;
       Context.currentIKTargetTransform.position = new Vector3(offsetPosition.x, 
           Context.InteractionPointYOffset,offsetPosition.z);
   }

}
