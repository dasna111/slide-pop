using UnityEngine;
using System.Collections;

public class DebuggerBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void OnGUI () {
        //if(Grid.Instance.needsToBeChecked) {

        if (!Spawner.Instance || Spawner.Instance.data == null)
            return;

			string output = "";
        for (int y = Spawner.Instance.height - 1; y >= 0; y--)
        {
            //for (int y = Grid.Instance.height - 1; y > -1; y--) {
				for(int x = 0; x < Spawner.Instance.width; x++)


                output += Spawner.Instance.data[y,x] + " ";
            //if(Grid.Instance.grid[x, y] != null)
            //	switch(Grid.Instance.grid[x, y].name.Substring(0, 1)) {
            //		case "R":  break;
            //		case "G": output += "G "; break;
            //		case "T": output += "T "; break;
            //		case "V": output += "V "; break;
            //		case "Y": output += "Y "; break;
            //	}
            //else 
            //	output += "X ";

            output += "\n";
			}

        GUI.TextField(new Rect(0, 0, 100,170), output);


            //GetComponent<GUIText>().text ;
			//Grid.Instance.needsToBeChecked = false;
		//}
	}
	
}
