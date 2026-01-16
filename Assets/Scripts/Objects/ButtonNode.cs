using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonNode : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private Node placedNode;
    public LayerMask playerLayer;
    public UnityEvent OnPressed;            // 버튼을 밟았을 때 이벤트

    private bool _isPressed = false;

    private void Update()
    {
        // 플레이어 탐색
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        // 플레이어 위치가 버튼의 위치와 같을 때
        bool isPlayerOnMe = (player != null && player.currentNode == placedNode && !player.isMoving);

        if (isPlayerOnMe && !_isPressed)
        {
            Press();
        }
    }

    private void Press()
    {
        _isPressed = true;
        OnPressed.Invoke();
    }
}
