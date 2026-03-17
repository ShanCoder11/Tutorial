using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    PlayerInput playerInput;    
    InputAction moveAction;

    Vector2 direction;
    
    [SerializeField] float playerSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Walking");
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();

 //       if (direction.sqrMagnitude > 1f)
 //       {
 //          direction = direction.normalized;
 //       }
    }

    void MovePlayer()
    {
        Debug.Log(direction);
        direction = moveAction.ReadValue<Vector2>();
        transform.position += new Vector3(direction.x, 0, direction.y) * Time.deltaTime * playerSpeed;
    }
}
