using UnityEngine;
using UnityEngine.InputSystem;

public class KaijuMegaspikanInputController : MonoBehaviour
{
    [SerializeField] private bool _debug;

    private KaijuMegaspikanAnimController _controller;

    private void Awake()
    {
        _controller = FindFirstObjectByType<KaijuMegaspikanAnimController>();
    }

    private void OnMove(InputValue value)
    {
        var input = value.Get<Vector2>();
        var forwards = input.y;
        var right = input.x;
        _controller.SetMovement(forwards, right);
        if (_debug) Debug.Log($"Move: {forwards}, {right}");
    }

    private void OnTurn(InputValue value)
    {
        var turnInput = value.Get<float>();
        _controller.SetTurn(turnInput);
    }

    private void OnLook(InputValue value)
    {
        // Look is not used for the Kaiju
    }

    private void OnAttackLeft(InputValue value)
    {
        if (value.isPressed)
        {
            _controller.Attack(Random.Range(1, 4));
        }
    }

    private void OnAttackRight(InputValue value)
    {
        if (value.isPressed)
        {
            _controller.Attack(Random.Range(4, 7));
        }
    }

    private void OnRoar(InputValue value)
    {
        if (value.isPressed)
        {
            _controller.Roar();
        }
    }

    private void OnSpit(InputValue value)
    {
        if (value.isPressed)
        {
            _controller.Attack(Random.Range(7, 9));
        }
    }

    private void OnSummon(InputValue value)
    {
        if (value.isPressed)
        {
            _controller.Attack(9);
        }
    }
}