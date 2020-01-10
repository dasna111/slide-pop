using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorControls2 : MonoBehaviour
{
    public static int leftX2, rightX2, cursorY2;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        leftX2 = (int)transform.localPosition.x +2;
        rightX2 = (int)transform.localPosition.x + 3;
        cursorY2 = (int)transform.position.y;

        if (Input.GetKeyDown(KeyCode.Q))
            SceneManager.LoadScene(0);

        if (Input.GetKeyDown(KeyCode.UpArrow))
            if (transform.position.y < 10)
                transform.position += new Vector3(0, 1, 0);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            if (transform.position.y > 0)
                transform.position += new Vector3(0, -1, 0);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            if (transform.position.x > 8)
                transform.position += new Vector3(-1, 0, 0);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            if (transform.position.x < 11)
                transform.position += new Vector3(1, 0, 0);


        if (Input.GetKeyDown(KeyCode.KeypadEnter))
            Spawner2.Instance.Switch(leftX2, rightX2, cursorY2);
    }
}
