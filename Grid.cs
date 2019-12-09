using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

    public int width = 6;
	public int height = 11;
    public int startHeight = 6;

    public Transform[,] grid;
	public bool needsToBeChecked = false;	//For debugger
	public bool changed = false;
	public ArrayList occupiedPos = new ArrayList();

    public static Grid Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    // Use this for initialization
    void Start ()
    {
        grid = new Transform[width, height];
        Spawner.Instance.Setup();
	}
	
	// Update is called once per frame
	void Update () {
		//DropBlocks();
		
		//if(Input.GetKeyDown(KeyCode.Space)) {
		//	SwapBlocks(CursorControls.leftX, CursorControls.rightX, CursorControls.cursorY);
		//}
	}
	
	public void PushUpRow() {
        for (int x = 0; x < width; x++)
            for (int y = height - 1; y > 0; y--){
                grid[x, y] = grid[x, y - 1];

                if (grid[x, y] != null)
                    grid[x, y].position += new Vector3(0, 1, 0);
            }
        changed = true;
	}

	public void DestroyBlock(int x, int y) {
		Vector2 v = new Vector2(x, y);
        Debug.Log("x: " + x + " y : " + y);
		
		if(!occupiedPos.Contains(v))
			occupiedPos.Add(v);

        if (grid.GetLength(0) > x && x > 0 && grid.GetLength(1) > y && y > 0)
        {
            iTween.ColorTo(grid[x, y].gameObject, 
                iTween.Hash("time", 1.2f, "color", Color.clear, "onCompleteTarget", GameObject.Find("Grid"), "onCompleteParams", v, "onComplete", "NullBlocks"));
        }
	}
	
	private void NullBlocks(Vector2 v) {
		int x = (int)v.x, 
			y = (int)v.y;
			
		if(grid[x, y] != null) {
			occupiedPos.Remove(v);
			Destroy(grid[x, y].gameObject);
			grid[x, y] = null;
			changed = true;
		}

	}
	
	private void DropBlocks() {
		ArrayList yPos = new ArrayList();

		for(int x = 0; x < width; x++) {
			for(int y = 0; y < height; y++)
				if(grid[x, y] != null)
					yPos.Add(y);

			foreach(int y in yPos) {
				for(int i = y; i - 1 >= 0 && grid[x, i - 1] == null; i--) {
					grid[x, i].position += new Vector3(0, -1, 0);
					grid[x, i - 1] = grid[x, i];
					grid[x, i] = null;
				}			
			}

			yPos.Clear();
		}
	}

    public void SwapBlocks(int leftX, int rightX, int cursorY) {
        Transform leftBlock = grid[leftX, cursorY],
                  rightBlock = grid[rightX, cursorY];

        if (leftBlock == null && rightBlock == null)
            { 
                changed = true;
            return;
            }
        foreach (Vector2 v in occupiedPos)
        {
            int x = (int)v.x,
                y = (int)v.y;

            if ((leftX == x || rightX == x) && cursorY == y)
            {
                changed = true;
                return;
            }
		}

		Vector3 leftPos = new Vector3 ((float)leftX - 2.5f, (float)cursorY, 0),
				rightPos = new Vector3 ((float)rightX - 2.5f, (float)cursorY, 0);

		if(leftBlock == null) {
			grid [leftX, cursorY] = rightBlock;
			rightBlock.position = leftPos;
			grid [rightX, cursorY] = null;
		}
		else if(rightBlock == null) {
			grid [rightX, cursorY] = leftBlock;
			leftBlock.position = rightPos;
			grid [leftX, cursorY] = null;
		}
		else {
			grid [leftX, cursorY].position = rightPos;
			grid [rightX, cursorY].position = leftPos;
			grid [leftX, cursorY] = rightBlock;
			grid [rightX, cursorY] = leftBlock;
		}

		changed = true;
		needsToBeChecked = true;
	}
}
