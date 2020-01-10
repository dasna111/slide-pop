using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBox2 : MonoBehaviour
{
    void OnGUI()
    {
        if (!Spawner2.Instance || Spawner2.Instance.data == null)
            return;

        string output = "";
        for (int y = Spawner2.Instance.height - 1; y >= 0; y--)
        {
            for (int x = 0; x < Spawner2.Instance.width; x++)
                output += Spawner2.Instance.data[y, x] + " ";
            output += "\n";
        }

        GUI.TextField(new Rect(0, 0, 100, 170), output);
    }
}
