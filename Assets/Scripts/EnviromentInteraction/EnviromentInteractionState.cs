using UnityEngine;

public abstract class EnviromentInteractionState : BaseState<EnviromentInteractionStateMachine.
    EEnviromentInteractionState>
{
   protected EnviromentInteractionContext Context;
   
   public EnviromentInteractionState(EnviromentInteractionContext context,
       EnviromentInteractionStateMachine.EEnviromentInteractionState stateKey) : base(stateKey)
   {
      Context = context;
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
       Context.currentIKTargetTransform.position = offsetPosition;
   }

}
