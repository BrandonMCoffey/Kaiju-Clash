using UnityEngine;
using UnityEngine.Splines;

public class KaijuShohebraAnimator : MonoBehaviour
{
    [SerializeField] private float _moveBlendSpeed = 1f;
    [SerializeField] private int _headSegments;
    [SerializeField] private int _bodySegments;
    [SerializeField] private int _tailSegments;

    [SerializeField] private Animator _headAnimator;
    [SerializeField] private SplineContainer _bodySpline;

    private bool _dead;
    private bool _inAction;

    private float _moveForwardsGoal;
    private float _moveRightGoal;
    private float _turnRightGoal;

    private float _moveForwards;
    private float _moveRight;
    private float _turnRight;

    private void Update()
    {
        if (_dead) return;
        float delta = Time.deltaTime * _moveBlendSpeed;
        _moveForwards = Mathf.MoveTowards(_moveForwards, _moveForwardsGoal, delta);
        _moveRight = Mathf.MoveTowards(_moveRight, _moveRightGoal, delta);
        _turnRight = Mathf.MoveTowards(_turnRight, _turnRightGoal, delta);

        //_animator.SetFloat("MoveBackForward", _moveForwards);
        //_animator.SetFloat("MoveLeftRight", _moveRight);
        //_animator.SetFloat("TurnLeftRight", _turnRight);
    }

    private void UpdateSpline()
    {
    }

    // Called by end of action animation
    private void OnActionEnd() => ActionEnd();
    private void ActionEnd()
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

    public void Kill()
    {
        Debug.Log("Death Triggered");
        //_animator.SetTrigger("Death");
        _dead = true;
    }
}