using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorControls : MonoBehaviour
{
    public static int leftX, rightX, cursorY;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        leftX = (int)transform.position.x + 2;
        rightX = (int)transform.position.x + 3;
        cursorY = (int)transform.position.y;

        if (Input.GetKeyDown(KeyCode.Q))
            SceneManager.LoadScene(0);

        if (Input.GetKeyDown(KeyCode.W))
            if (transform.position.y < 10)
                transform.position += new Vector3(0, 1, 0);

        if (Input.GetKeyDown(KeyCode.S))
            if (transform.position.y > 0)
                transform.position += new Vector3(0, -1, 0);

        if (Input.GetKeyDown(KeyCode.A))
            if (transform.position.x > -2)
                transform.position += new Vector3(-1, 0, 0);

        if (Input.GetKeyDown(KeyCode.D))
            if (transform.position.x < 2)
                transform.position += new Vector3(1, 0, 0);


        if (Input.GetKeyDown(KeyCode.Space))
            Spawner.Instance.Switch(leftX, rightX, cursorY);
    }
}
