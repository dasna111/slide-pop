using UnityEngine;

public class DebuggerBox : MonoBehaviour
{
    void OnGUI()
    {
        if (!Spawner.Instance || Spawner.Instance.data == null)
            return;

        string output = "";
        for (int y = Spawner.Instance.height - 1; y >= 0; y--)
        {
            for (int x = 0; x < Spawner.Instance.width; x++)
                output += Spawner.Instance.data[y, x] + " ";
            output += "\n";
        }

        GUI.TextField(new Rect(0, 0, 100, 170), output);
    }
}
