using UnityEngine;

public class KaijuMegaspikanAnimController : MonoBehaviour
{
    [SerializeField] private float _moveBlendSpeed = 1f;

    private Animator _animator;

    private bool _dead;
    private bool _inAction;
    private float _moveForwards;
    private float _moveRight;
    private float _turnRight;
    private float _moveForwardsGoal;
    private float _moveRightGoal;
    private float _turnRightGoal;

    /*
     * TurnLeftRight: float -1 to 1
     * MoveLeftRight: float -1 to 1
     * MoveBackForward: float -1 to 1
     * Roar: Trigger
     * Attack: Trigger
     * AttackIndex: Int 1-3 (Left attacks) 4-6 (Right attacks) 7-10 (Spitters)
     * HitResponse: Trigger
     * HitLeftRight: float -1 to 1
     * HitBackFront: float -1 to 1
     * Death: Trigger
     * ActionEnd Event Function
     */

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_dead) return;
        float delta = Time.deltaTime * _moveBlendSpeed;
        _moveForwards = Mathf.MoveTowards(_moveForwards, _moveForwardsGoal, delta);
        _moveRight = Mathf.MoveTowards(_moveRight, _moveRightGoal, delta);
        _turnRight = Mathf.MoveTowards(_turnRight, _turnRightGoal, delta);

        _animator.SetFloat("MoveBackForward", _moveForwards);
        _animator.SetFloat("MoveLeftRight", _moveRight);
        _animator.SetFloat("TurnLeftRight", _turnRight);
    }

    // Called by end of action animation
    private void ActionEnd() => OnActionEnd();
    private void OnActionEnd()
    {
        Debug.Log("Action End");
        _inAction = false;
    }

    public void SetMovement(float forwards, float right)
    {
        if (_dead) return;
        Debug.Log($"Move Forward: {forwards}, Move Right: {right}");
        _moveForwardsGoal = forwards;
        _moveRightGoal = right;
    }

    public void SetTurn(float turnRight)
    {
        if (_dead) return;
        Debug.Log($"Turn Right: {turnRight}");
        _turnRightGoal = turnRight;
    }

    public void Roar()
    {
        if (_dead) return;
        Debug.Log("Roar Start");
        _animator.SetTrigger("Roar");
        _inAction = true;
    }

    public void Attack(int attackIndex)
    {
        if (_dead) return;
        Debug.Log("Attack Start");
        _animator.SetInteger("AttackIndex", attackIndex);
        _animator.SetTrigger("Attack");
        _inAction = true;
    }

    public void Hit(Vector3 hitDirection)
    {
        if (_dead) return;
        Debug.Log("Hit Reaction Start");
        Vector3 localHitDir = transform.InverseTransformDirection(hitDirection.normalized);
        _animator.SetFloat("HitLeftRight", localHitDir.x);
        _animator.SetFloat("HitBackFront", localHitDir.z);
        _animator.SetTrigger("HitResponse");
    }

    public void Kill()
    {
        Debug.Log("Death Triggered");
        _animator.SetTrigger("Death");
        _dead = true;
    }
}
