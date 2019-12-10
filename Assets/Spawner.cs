using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Spawner : MonoBehaviour {

    public static Spawner Instance;
    public GameObject[] blocks;

    public int[,] data = null;

    public int width = 6;
    public int height = 11;
    [SerializeField] private int startHeight = 6;

    //private const float startPos = -2.5f;

	private int previous = -1;

    private float SpawnerTimer = 0;
    public float SpawnEvery;


    //   private int? prev = null;
    //private int? curr = null;

    //private int prevCount = 0;
    //   private object prevBlock;

    //   private bool Finished;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    // Use this for initialization
    public void Setup ()
    {

        data = new int[height, width];

        for (int y = 0; y < startHeight; y++)
        {
			for(int x = 0; x < width; x++)
            {
                data[y, x] = GenerateBlock(x, y, previous);
                previous = data[y, x];   
            }
		}

        for (int y = startHeight; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                data[y, x] = -1;
            }
        }

                //MatchColumn();
                //MatchRow();
    }



    // Update is called once per frame
    void Update()
    {
        SpawnerTimer = SpawnerTimer + Time.deltaTime;
        if (SpawnerTimer >= SpawnEvery || Input.GetKeyDown(KeyCode.Z))
        {
            SpawnNewLine();
            SpawnerTimer = SpawnerTimer - SpawnEvery;

        }
            //if (Grid.Instance.changed)
            //{
            //    MatchColumn();
            //    MatchRow();
            //    Grid.Instance.changed = false;
            //}

            Match();
    }

    private void SpawnNewLine()
    {
        PushUp();
        GenerateRow(0);
    }

    #region Match
    private int Match()
    {
        List<Vector2Int> matchs = FindAllMatchs();
        RemoveMatchs(matchs);
        FallDown();

        return matchs.Count;
    }

    private List<Vector2Int> FindAllMatchs()
    {
        List<Vector2Int> matchs = new List<Vector2Int>();
        List<Vector2Int> currentMatchs;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (matchs.Contains(new Vector2Int(y, x)))
                    continue;

                currentMatchs = FindMatchForCell(y, x);
                matchs.AddRange(currentMatchs);
            }
        }

        return matchs;
    }

    private void RemoveMatchs(List<Vector2Int> matchs)
    {
        for (int i = 0; i < matchs.Count; i++)
        {
            var coords = matchs[i];
            data[coords.x, coords.y] = -1;
        }
    }
    private void FallDown()
    {
        for (int y = 1; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (data[y - 1, x] == -1)
                {
                    data[y - 1, x] = data[y, x];
                    MoveRowDown(x, y);
                }
            }
        }
    }

    private void MoveRowDown(int x, int y)
    {
        data[y, x] = data[y - 1, x];
    }

    private List<Vector2Int> FindMatchForCell(int x, int y)
    {
        List<Vector2Int> matching = new List<Vector2Int>();
        matching.AddRange(FindCellNeighbor(x, y, matching));
        return matching.Count < 3 ? new List<Vector2Int>() : matching;
    }

    // TODO check cells cordiantes if they exist in the list so we ignore them

    private List<Vector2Int> FindCellNeighbor(int x, int y, List<Vector2Int> matching)
    {
        matching.Add(new Vector2Int(x, y));
        List<Vector2Int> exist = new List<Vector2Int>();
        //foreach (Vector2Int n in exist)
        //{
        //    if (matching.Contains(n))
        //    {
                Debug.Log("Match?");
                if (x >= 0 && y <= height && y >= 0 && x <= width && data[x, y] != -1)      // checks if cell coordiantes hits the borders
                {
                    if (data[x, y] == data[x + 1, y])
                    {
                        matching.AddRange(FindCellNeighbor(x + 1, y, matching));
                        exist.AddRange(matching);
                        Debug.Log("x + 1");
                    }
                    if (data[x, y] == data[x - 1, y])
                    {
                        matching.AddRange(FindCellNeighbor(x - 1, y, matching));
                        exist.AddRange(matching);
                        Debug.Log("x - 1");
                    }
                    if (data[x, y] == data[x, y + 1])
                    {
                        matching.AddRange(FindCellNeighbor(x, y + 1, matching));
                        exist.AddRange(matching);
                        Debug.Log("y + 1");
                    }
                    if (data[x, y] == data[x, y - 1])
                    {
                        matching.AddRange(FindCellNeighbor(x, y - 1, matching));
                        exist.AddRange(matching);
                        Debug.Log("y - 1");
                    }
                }
            //}
        //}
        return matching;
    }
    #endregion


    /*
    private void MatchRow()
    {
		Stack matchingBlocks = new Stack();
        

        for (int y = 0; y < Grid.Instance.height; y++)
        {
			for(int x = 0; x < Grid.Instance.width; x++)
            {
		 		if(matchingBlocks.Count > 0)
                {
					if(Grid.Instance.grid[x, y] != null)
                    {
                        string prevBlock = null;


                        if (Grid.Instance.grid[(int)matchingBlocks.Peek(), y] != null)
                            prevBlock = Grid.Instance.grid[(int)matchingBlocks.Peek(), y].name.Substring(0, 1);

                        string currBlock = Grid.Instance.grid[x, y].name.Substring(0, 1);
                        if (prevBlock != null && !prevBlock.Equals(currBlock))
                        {
                            if (matchingBlocks.Count < 3)
                            {
                                matchingBlocks.Clear();
                                matchingBlocks.Push(x);
                            }
                        }
                        else
                            matchingBlocks.Push(x);
                    }
                    else if(matchingBlocks.Count < 3)
		 				matchingBlocks.Clear();		
		 		}
		 		else
					matchingBlocks.Push(x);
		 	}
		 	
			if(matchingBlocks.Count > 2)
				for(int i = matchingBlocks.Count; i > 0; i--)
					Grid.Instance.DestroyBlock((int)matchingBlocks.Pop(), y);
			else
				matchingBlocks.Clear();
		}
	
	
		ArrayList matchingCol = new ArrayList();
		int prevCount = 1;
			
		for(int y = 0; y < Grid.Instance.height; y++) {
			for(int x = 0; x < Grid.Instance.width; x++) {
				if(Grid.Instance.grid[x, y] != null) {
					Transform currBlock = Grid.Instance.grid[x, y];
                    Transform prevBlock = Grid.Instance.grid[0, 0];


                    if (prevBlock != null)
						if (!currBlock.name.Substring(0, 1).Equals(prevBlock.name.Substring(0, 1)))
                        {
							if (prevCount > 2)
								for (int i = 0; i < prevCount; i++)
									matchingCol.Add(x - i - 1);	
							
							prevCount = 1;
						}
						else
							prevCount++;
						
					prevBlock = currBlock;
				}
				else {
					if (prevCount > 2)
						for (int i = 0; i < prevCount; i++)
							matchingCol.Add(x - i - 1);	

					prevBlock = null;
					prevCount = 1;
				}
			}

			if (prevCount > 2)
				for (int i = 0; i < prevCount; i++)
					matchingCol.Add(Grid.Instance.width - i - 1);	

			foreach(int x in matchingCol)
				Grid.Instance.DestroyBlock(x, y);
				
			matchingCol = new ArrayList();
			prevBlock = null;
			prevCount = 1;
		}
    }

    private void MatchColumn() {
        ArrayList matchingCol = new ArrayList();
        GameObject prevBlock = null;
        int prevCount = 1;

		for(int x = 0; x < Grid.Instance.width; x++)
        {
			for(int y = 0; y < Grid.Instance.height; y++)
            {
                if (Grid.Instance.grid[x, y] == null)
                {
					if(prevCount > 2)
						for (int i = 0; i < prevCount; i++)
							matchingCol.Add(y - i - 1);	
					break;

				}

				GameObject currBlock = Grid.Instance.grid[x, y].gameObject;

				if(prevBlock != null)
	            	if (!currBlock.name.Substring(0, 1).Equals(prevBlock.name.Substring(0,1)))
                    {
						if (prevCount > 2)
							for (int i = 0; i < prevCount; i++)
								matchingCol.Add(y - i - 1);	

						prevCount = 1;
					}
	                else
	                	prevCount++;



                    prevBlock = currBlock;
                }

            foreach(int y in matchingCol)
				Grid.Instance.DestroyBlock(x, y);
			
			matchingCol = new ArrayList();
            prevBlock = null;
            prevCount = 1;
		}


	}
    */

    public void PushUp()
    {
        for (int y = height - 1; y > 0 ; y--)
        {
            for (int x = 0; x < width; x++)
            {
                data[y, x] = data[y - 1, x];
            }
        }
    }

	public void GenerateRow(int row)
    {
		//transform.position = new Vector3(startPos, 0, 0);
		
		for(int x = 0; x < width; x++)
        {
            data[row, x] = GenerateBlock(x, row, -1);
			//transform.position += new Vector3(1.0f, 0, 0);
		}

        //Grid.Instance.changed = true;
    }

	public int GenerateBlock(int x, int y, int avoidIndex)
    {
        int i = -1;
        do
        {
            i = UnityEngine.Random.Range(0, blocks.Length);
        }
        while (i == -1 || i == avoidIndex);

        //GameObject block = (GameObject)Instantiate (blocks [i], transform.position, Quaternion.identity);
        //Grid.Instance.grid [x, y] = block.transform;

        return i;
	}
}
