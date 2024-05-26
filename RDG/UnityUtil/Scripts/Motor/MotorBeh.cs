using System.Linq;
using RDG.UnityUtil.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace RDG.Util.Scripts.Motor {

  public class MotorBeh : MonoBehaviour {
    public enum JumpState {
      Able, InProgress, Exhausted
    }
    private class MovementInfo {
      public Vector3 Position { get; set; }
      public Quaternion Rotation { get; set; }
      public Vector3 ForwardNormal { get; set; }
      public float RemainingDistance { get; set; }
      public bool IsJumping { get; set; }
      public bool IsGrounded { get; set; }
    }

    // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
    private const RigidbodyConstraints FREEZE_ROTATION = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    private static readonly Vector3[] DowncastOffsets ={
      Vector3.left, Vector3.zero, Vector3.right, Vector3.forward, Vector3.back
    };
    
    [SerializeField] private MotorSo motors;
    [SerializeField] private MotorConfig config;
    [SerializeField] private MotorEvents events;
    [SerializeField] private Rigidbody body;
    
    private GroundBeh[] myGround = {};
    private LayerMask groundLayerMask;
    
    private bool isJumpRequested;
    private Vector2 moveRequested;
    private bool isRequestLocked;
    private bool isDashRequested;
    
    private float fallVelocity;
    private float jumpDuration;
    private JumpState jumpState;
    private bool isGrounded;
    private float dashStartTime;
    private Vector3 dashFacing;
    
    public void Start() {
      body.constraints = FREEZE_ROTATION;
      myGround = body.GetComponentsInChildren<GroundBeh>();
      groundLayerMask = motors.GroundLayerMask;
    }

    public void RequestDriveDirection(Vector2 dir) {
      moveRequested = dir;
    }

    public void RequestJumpFlag(bool jumpFlag) {
      isJumpRequested = jumpFlag;
    }

    public void RequestDash() {
      if (isRequestLocked) {
        return;
      }
      
      isDashRequested = true;
    }

    public void LockRequests() {
      isRequestLocked = true;
    }

    public void UnlockRequests() {
      isRequestLocked = false;
    }

    public bool IsRequestLocked => isRequestLocked;
    
    public Vector2 LastMoveDirection { get; private set; }
    

    public void FixedUpdate() {
      var bodyTransform = body.transform;
      var directorDir = moveRequested;
      if (isRequestLocked) {
        directorDir = Vector2.zero;
      }
      var info = new MovementInfo{
        Position = bodyTransform.position,
        Rotation = bodyTransform.rotation,
        ForwardNormal = new Vector3(directorDir.x, 0, directorDir.y).normalized,
        IsJumping = false,
        RemainingDistance = config.moveSpeed * Time.fixedDeltaTime
      };
      
      Debug.DrawLine(info.Position, info.Position + (info.ForwardNormal * 10), Color.magenta);
      UpdateStateForDash(info);
      UpdateStateForJumpUp(info);
      UpdateStateForForwardRotation(info);
      UpdateStateForGroundAngle(info);
      UpdateStateForForwardMove(info);
      UpdateStateForForwardMove(info);
      UpdateStateForFalling(info);
      UpdateStateForGroundLanding(info);
      Debug.DrawLine(info.Position, info.Position + (info.ForwardNormal * 5), Color.cyan);
      body.MovePosition(info.Position);
      body.MoveRotation(info.Rotation);

      UpdateStateForGroundLanding(info);
      LastMoveDirection = directorDir;
    }
    
    public bool IsDashing { get; private set; }

    public float DashDuration => config.dashDurationSeconds;

    public JumpState Jump => jumpState;

    public bool IsGrounded => isGrounded;

    private void UpdateStateForDash(MovementInfo info) {
      if (isDashRequested && !IsDashing && !isRequestLocked) {
        IsDashing = true;
        isDashRequested = false;
        events.onDashStart.Invoke();
        dashStartTime = Time.time;
        dashFacing = info.ForwardNormal.magnitude > 0 ? info.ForwardNormal : body.transform.forward;
      }
      if (IsDashing && Time.time - dashStartTime >= config.dashDurationSeconds) {
        isDashRequested = false;
        IsDashing = false;
        events.onDashStop.Invoke();
      }
      if (!IsDashing) {
        return;
      }
      info.ForwardNormal = dashFacing;
      info.RemainingDistance *= config.dashFactor;

    }
    
    // Handle landing back on the ground
    private void UpdateStateForGroundLanding(MovementInfo info) {
      var wasGrounded = isGrounded;
      isGrounded = info.IsGrounded;
      if (isGrounded && !wasGrounded) {
        events.onGrounded.Invoke();
      }
      if (isGrounded && !isJumpRequested) {
        jumpState = JumpState.Able;
      }
    }

    // Handle movement due to jump upward cycle
    private void UpdateStateForJumpUp(MovementInfo state) {
      if (isJumpRequested && jumpState == JumpState.Able && !isRequestLocked) {
        jumpState = JumpState.InProgress;
        events.onJump.Invoke();
      }

      if (jumpState != JumpState.InProgress) {
        return;
      }
      
      if (!isJumpRequested || isRequestLocked) {
        jumpState = JumpState.Exhausted;
        return;
      }
      
      var maxJump = motors.Config.jumpCurve.keys.Last().time;
      if (jumpDuration > maxJump) {
        jumpState = JumpState.Exhausted;
        return;
      }
      
      state.IsJumping = true;
      jumpDuration += Time.fixedDeltaTime;
      var jumpSpeed = config.jumpSpeed * motors.Config.jumpCurve.Evaluate(jumpDuration) * Time.deltaTime;
      var heightOffset = Vector3.up * config.height;
      var ground = UpCast(state.Position + heightOffset, jumpSpeed, out var hit);
      if (ground == null) {
        state.Position += Vector3.up * jumpSpeed;
        return;
      }

      state.Position = hit.point - heightOffset;
      jumpState = JumpState.Exhausted;
    }

    // Update the forward direction for tilted ground when grounded
    private void UpdateStateForGroundAngle(MovementInfo state) {
      var ground = DownCast(state.Position, config.radius, motors.Config.groundCheckDistance, out var hit, out _);
      if (ground == null) {
        return;
      }

      if (state.ForwardNormal.sqrMagnitude == 0) {
        return;
      }
      
      var cross = Vector3.Cross(hit.normal, state.ForwardNormal);
      var groundTangentDir = Vector3.Cross(cross, hit.normal);
      state.ForwardNormal = groundTangentDir.normalized;
    }
    
    // Move along the forward direction, tangent to any walls encountered
    private void UpdateStateForForwardMove(MovementInfo state) {
      if (state.ForwardNormal.sqrMagnitude == 0 || state.RemainingDistance <= 0.0f) {
        return;
      }

      var distance = state.RemainingDistance + config.radius;
      var position = state.Position + Vector3.up * (motors.Config.wallHeightCheck);
      var wall = WidthCast(position, state.ForwardNormal, config.radius, distance, out var hit);
      if (wall == null) {
        state.Position += state.ForwardNormal * state.RemainingDistance;
        state.RemainingDistance = 0;
        return;
      }

      // Move up to or out out of wall
      var hitDistance = hit.distance;
      state.Position += state.ForwardNormal * (hitDistance - config.radius);
      state.RemainingDistance -= (hitDistance - config.radius);
      
      // Forward is now tangent to wall
      var cross = Vector3.Cross(hit.normal, state.ForwardNormal);
      state.ForwardNormal = Vector3.Cross(cross, hit.normal);
    }

    // Rotate to face the forward direction when one is present
    private void UpdateStateForForwardRotation(MovementInfo state) {
      if (state.ForwardNormal.sqrMagnitude == 0) {
        return;
      }

      state.Rotation = Quaternion.RotateTowards(
        state.Rotation,
        Quaternion.LookRotation(state.ForwardNormal, Vector3.up),
        Time.fixedDeltaTime * config.rotationSpeed
      );
    }

    // Move downward when not jumping and in the air
    private void UpdateStateForFalling(MovementInfo state) {
      if (state.IsJumping) {
        return;
      }
      
      fallVelocity += (motors.Config.fallRate * Time.fixedDeltaTime);
      fallVelocity = Mathf.Min(fallVelocity, motors.Config.fallMaxSpeed);
      var ground = DownCast(state.Position, config.radius, fallVelocity, out var hit, out var offset);
      if (ground == null) {
        state.Position += Vector3.down * fallVelocity;
        return;
      }
      
      jumpDuration = 0.0f;
      fallVelocity = 0.0f;
      state.IsGrounded = true;
      state.Position = hit.point - offset * config.radius;
    }
    
    private Ground GetGround(int hitIndex) {
      return motors.GetGround(CacheUtil.RaycastHits[hitIndex].collider);
    }

    private Ground GetNearestCacheGround(int count, out RaycastHit hit) {
      Ground nearestGround = null;
      var nearest = float.MaxValue;
      hit = CacheUtil.RaycastHits[0];
      for (var i = 0; i < count; i++) {
        var ground = GetGround(i);
        if (ground == null) {
          continue;
        }
        
        if (myGround.Contains(ground)) {
          continue;
        }
        
        var distance = CacheUtil.RaycastHits[i].distance;
        if (distance >= nearest) {
          continue;
        }

        hit = CacheUtil.RaycastHits[i];
        nearestGround = ground;
        nearest = distance;
      }
      return nearestGround;
    }
    
    private Ground WidthCast(Vector3 position, Vector3 direction, float radius, float distance, out RaycastHit hit) {
      direction = direction.normalized;
      Vector3[] positions ={
        position + Vector3.Cross(direction, Vector3.up).normalized * radius,
        position,
        position + Vector3.Cross(Vector3.up, direction).normalized * radius
      };
      Ground nearestGround = null;
      var nearestDistance = float.MaxValue;
      hit = CacheUtil.RaycastHits[0];
      foreach (var aPos in positions) {
        if (motors.Config.drawDebug) {
          Debug.DrawRay(aPos, direction * distance, Color.yellow);
        }
        var hitCount = Physics.RaycastNonAlloc(aPos, direction, CacheUtil.RaycastHits, distance, groundLayerMask);
        var ground = GetNearestCacheGround(hitCount, out var groundHit);
        if(ground == null){
          continue;
        }
        if (groundHit.distance >= nearestDistance) {
          continue;
        }
        nearestGround = ground;
        nearestDistance = groundHit.distance;
        hit = groundHit;
      }
      return nearestGround;
    }
    
    private Ground DownCast(Vector3 position, float radius, float distance, out RaycastHit hit, out Vector3 hitOffset) {
      var direction = Vector3.down;
      Ground nearestGround = null;
      var nearestDistance = float.MaxValue;
      hit = CacheUtil.RaycastHits[0];
      hitOffset = Vector3.zero;
      foreach (var offset in DowncastOffsets) {
        var pos = position + (offset * radius) + Vector3.up * motors.Config.checkBackupDistance;
        var checkDistance = distance + motors.Config.checkBackupDistance;
        if (motors.Config.drawDebug) {
          Debug.DrawRay(pos, direction * checkDistance, Color.magenta);
        }
        var hitCount = Physics.RaycastNonAlloc(pos, direction, CacheUtil.RaycastHits, checkDistance, groundLayerMask);
        var ground = GetNearestCacheGround(hitCount, out var groundHit);
        if(ground == null){
          continue;
        }
        if (groundHit.distance >= nearestDistance) {
          continue;
        }
        hitOffset = offset;
        nearestGround = ground;
        nearestDistance = groundHit.distance;
        hit = groundHit;
      }
      return nearestGround;
    }
    
    private Ground UpCast(Vector3 position, float distance, out RaycastHit hit) {
      var dirNormal = Vector3.up;
      var checkStart = position + Vector3.down * motors.Config.checkBackupDistance;
      var checkDistance = distance + motors.Config.checkBackupDistance;
      if (motors.Config.drawDebug) {
        Debug.DrawRay(checkStart, dirNormal.normalized * checkDistance, Color.magenta);
      }
      var hitCount = Physics.RaycastNonAlloc(checkStart, dirNormal, CacheUtil.RaycastHits, checkDistance, groundLayerMask);
      var nearestGround = GetNearestCacheGround(hitCount, out var upHit);
      hit = upHit;
      return nearestGround;
    }

    public void OnDestroy() {
      events.Release();
    }

  }
  
}
