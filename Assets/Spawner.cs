using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;

    public GameObject[] blocks;

    [SerializeField] private Transform startAnchor;
    public GameObject[] GarbageBlock;

    //public Transform[,] grid;
    //public ArrayList occupiedPos = new ArrayList();
    

    public int[,] data = null;
    public GameObject[,] cubes = null;


    public int width = 6;
    public int height = 11;
    [SerializeField] private int startHeight = 6;

    private const float startPos = -2.5f;

    private int previous = -1;

    private float SpawnerTimer = 0;
    public float SpawnEvery;
    private int PanicSpawn;
    public float PanicTimer;
    private bool Matching = false;
    private bool Won;
    private string CharacterName;
    private string CharacterGame;
    private float MiniGameTimer;
    private int combo;
    public float blockMoveTime;
    public GameObject Ceilings = null;
    private Vector3 CeilingStart = new Vector3(-3, 10, 0);


    //   private int? prev = null;
    //   private int? curr = null;

    //   private int prevCount = 0;
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
    public void Start()
    {
        //data = new int[11, 6]
        //{
        //    {-1, 1, 1 ,3 ,4 ,0},
        //    {1, -1, -1 ,3 ,4 ,0},
        //    {-1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //    {1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //    {-1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //    {1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //    {-1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //    {1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //    {-1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //    {1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //    {-1 ,-1 ,-1 ,-1 ,-1 ,-1},
        //};


        data = new int[height, width];
        cubes = new GameObject[height, width];

        for (int y = 0; y < startHeight; y++)
        {
            for (int x = 0; x < width; x++)
            {
                data[y, x] = GenerateBlock(y, x, previous);
                cubes[y, x] = Instantiate(blocks[data[y, x]], GetPosition(y, x), Quaternion.identity);
                previous = data[y, x];
            }
        }

        for (int y = startHeight; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                data[y, x] = -1;
                cubes[y, x] = null;
            }
        }
        LoopMatch();
        MiniGame(Won);
        Ceiling.Asign(Player1, Player2);
        Ceilings.transform.position = CeilingStart;
    }
    public void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Ceiling.transform.position += new Vector3(0, -1, 0);
            height--;
        }
    }
    public void MiniGame(object won)
    {
        MiniGameTimer -= combo;
        MiniGameTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.M)) {
            Ceiling.transform.position += new Vector3(0, -1, 0);
        }
        if (MiniGameTimer <= 0)
        {
            //Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y - 20, Camera.main.transform.position.z); // Mini game should be 20 lines lower than actual game
            if (Won)
            {
                height--;
                Ceiling.transform.position += new Vector3(0, -1, 0);
                Won = false;
            }
        }
    }

    private void LoopMatch()
    {
        int count = Match();
        while (count > 0)
            count = Match();
    }

    void Update()
    {
        SpawnNewLineLoop();
    }

    private void SpawnNewLineLoop()
    {
        SpawnerTimer += Time.deltaTime;
        for (int x = 0; x < width; x++)
        {
            
            if (data[height-1, x] != -1)
            {
                Panic();
                Debug.Log("PANIC!!!!");
            }
        }
        if (SpawnerTimer < SpawnEvery + PanicSpawn && !Input.GetKeyDown(KeyCode.Z))
            return;
        SpawnNewLine();
        SpawnerTimer = 0;
    }

    private void SpawnNewLine()
    {
        PushUp();
        GenerateRow(0);
    }

    #region Match
    private int Match()
    {
        List<Vector2Int> matchs = new List<Vector2Int>();
        matchs = FindAllMatchs();
        combo = 1;
        RemoveMatchs(matchs);
        FallDown();
        if (matchs.Count > 3) // matchs
        {
            combo = combo * matchs.Count;
            MiniGame(Won);
        }
        Matching = true;
        return matchs.Count;
    }

    private List<Vector2Int> FindAllMatchs()
    {
        List<Vector2Int> matchs = new List<Vector2Int>();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (data[y, x] == -1)
                    continue;

                if (matchs.Contains(new Vector2Int(y, x)))
                    continue;

                matchs.AddRange(FindMatchForCell(y, x));
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
            Destroy(cubes[coords.x, coords.y]);
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
                    for (int z = y - 1; z < height - 1; z++) { 
                        data[z, x] = data[z + 1, x];
                        cubes[z, x] = cubes[z + 1, x];

                        if (cubes[z, x])
                            cubes[z, x].transform.position = GetPosition(z, x);
                    }
                }
            }
        }
    }

    private List<Vector2Int> FindMatchForCell(int y, int x)
    {
        //Debug.Log("Looking in  => " + y + " | " + x);
        List<Vector2Int> matching = new List<Vector2Int>();

        List<Vector2Int> row = FindRow(y, x);
        List<Vector2Int> column = FindColumn(y, x);

        if (row.Count > 2) matching.AddRange(row);
        if (column.Count > 2) matching.AddRange(column);

        return matching.Distinct().ToList();
    }

    private List<Vector2Int> FindColumn(int y, int x)
    {
        List<Vector2Int> matching = new List<Vector2Int>();
        matching.Add(new Vector2Int(y, x));

        for (int _y = y + 1; _y < height; _y++) // Right side
        {
            if (data[_y, x] == data[y, x])
                matching.Add(new Vector2Int(_y, x));
            else
                break;
        }

        for (int _y = y - 1; _y >= 0; _y--) // left side
        {
            if (data[_y, x] == data[y, x])
                matching.Add(new Vector2Int(_y, x));
            else
                break;
        }
        return matching;
    }

    private List<Vector2Int> FindRow(int y, int x)
    {
        List<Vector2Int> matching = new List<Vector2Int>();
        matching.Add(new Vector2Int(y, x));

        for (int _x = x + 1; _x < width; _x++) // Downward side
        {
            if (data[y, _x] == data[y, x])
                matching.Add(new Vector2Int(y, _x));
            else
                break;
        }

        for (int _x = x - 1; _x >= 0; _x--) // upward side
        {
            if (data[y, _x] == data[y, x])
                matching.Add(new Vector2Int(y, _x));
            else
                break;
        }

        return matching;
    }

    /*
    private List<Vector2Int> FindCellNeighbor(int x, int y, List<Vector2Int> matching)
    {
        matching.Add(new Vector2Int(y, x));

        for (int _y = y + 1; _y < height; _y++) // Right side
        {
            if (data[_y, x] == data[y, x])
                matching.Add(new Vector2Int(_y, x));
            else
                break;
        }

        for (int _y = y - 1; _y >= 0; _y--) // left side
        {
            if (data[_y, x] == data[y, x])
                matching.Add(new Vector2Int(_y, x));
            else
                break;
        }



        for (int _x = x + 1; _x < width; _x++) // Downward side
        {
            if (data[y, _x] == data[y, x])
                matching.Add(new Vector2Int(y, _x));
            else
                break;
        }

        for (int _x = x - 1; _x >= 0; _x--) // upward side
        {
            if (data[y, _x] == data[y, x])
                matching.Add(new Vector2Int(y, _x));
            else
                break;
        }

        //List<Vector2Int> exist = new List<Vector2Int>();


        //if (!matching.Contains(new Vector2Int(y + 1, x)) && x >= 0 && x < width && y < height  -1 && y >= 0 && data[y + 1, x] > -1)
        //{
        //    Debug.Log("Match?");
        //    if (data[y, x] == data[y + 1, x])
        //    {
        //        matching.AddRange(FindCellNeighbor(y + 1, x, matching));
        //        //exist.Add(new Vector2Int(x, y));
        //        Debug.Log("x + 1");
        //    }
        //}

        //if (!matching.Contains(new Vector2Int(y - 1, x)) && x >= 0 && x < width && y < height && y > 0 && data[y - 1, x] > -1)
        //{
        //    if (data[y, x] == data[y - 1, x])
        //    {
        //        matching.AddRange(FindCellNeighbor(y - 1, x, matching));
        //        //exist.Add(new Vector2Int(x, y));
        //        Debug.Log("x - 1");
        //    }
        //}

        //Debug.LogFormat("{0} | {1}", x,y);
        //if (!matching.Contains(new Vector2Int(y, x + 1)) && x >= 0 && x < width - 1 && y < height && y >= 0 && data[y, x + 1] > -1)
        //{
        //    if (data[y, x] == data[y, x + 1])
        //    {
        //        matching.AddRange(FindCellNeighbor(y, x + 1, matching));
        //        //exist.Add(new Vector2Int(x, y));
        //        Debug.Log("x + 1");
        //    }
        //}

        //if (!matching.Contains(new Vector2Int(y, x - 1)) && x > 0 && x < width && y < height && y >= 0 && data[y, x - 1] > -1)
        //{
        //    if (data[y, x] == data[y, x - 1])
        //    {
        //        matching.AddRange(FindCellNeighbor(y, x - 1, matching));
        //        //exist.Add(new Vector2Int(x, y));
        //        Debug.Log("x - 1");
        //    }
        //}



        //foreach (Vector2Int n in matching)
        //{
        //    if (!exist.Contains(n))
        //    {

        //        }
        //    }
        //}
        return matching;
    }*/
    #endregion

    public void Switch(int leftX, int rightX, int cursorY)
    {
        int temp = data[cursorY, leftX];
        data[cursorY, leftX] = data[cursorY, rightX];
        data[cursorY, rightX] = temp;

        var tempGO = cubes[cursorY, leftX];
        cubes[cursorY, leftX] = cubes[cursorY, rightX];
        cubes[cursorY, rightX] = tempGO;

        if(cubes[cursorY, leftX])
            cubes[cursorY, leftX].transform.position = GetPosition(cursorY, leftX);

        if (cubes[cursorY, rightX])
            cubes[cursorY, rightX].transform.position = GetPosition(cursorY, rightX);

        //iTween.MoveTo(cubes[cursorY, rightX], cubes[cursorY, rightX].transform.position, blockMoveTime);
        //iTween.MoveTo(cubes[cursorY, leftX], cubes[cursorY, leftX].transform.position, blockMoveTime);

        LoopMatch();
    }

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
        for (int y = height - 1; y > 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                data[y, x] = data[y - 1, x];
                cubes[y, x] = cubes[y - 1, x];

                if (cubes[y, x])
                    cubes[y, x].transform.position = GetPosition(y, x);
            }
        }
    }

    public void GenerateRow(int row)
    {
        for (int x = 0; x < width; x++)
        {
            data[row, x] = GenerateBlock(row, x, -1);
            cubes[row, x] = Instantiate(blocks[data[row, x]], GetPosition(row, x), Quaternion.identity);
        }
        LoopMatch();
    }

    private void Panic()
    {
        PanicSpawn = 90;
        PanicTimer -= Time.deltaTime;
        if (PanicTimer < 0)
            GameOver();
        else
            PanicSpawn = 0;
    }

                
    private void GameOver()
    {
        Debug.Log("Game Over");
        SceneManager.LoadScene(0);
    }

    public int GenerateBlock(int y, int x, int avoidIndex)
    {
        int i = -1;
        do
        {
            i = Random.Range(0, blocks.Length);
        }
        while (i == -1 || i == avoidIndex);

        return i;
    }

    private Vector3 GetPosition(int y, int x)
    {
        return new Vector3(startAnchor.position.x + (x * 1f), startAnchor.position.y + (y * 1f), 0f);
    }


    public T Choose<T>(T a, T b, params T[] p)
    {
        int random = Random.Range(0, p.Length + 2);
        if (random == 0) return a;
        if (random == 1) return b;
        return p[random - 2];
    }


    /*
     private void GarbageBlocks(List<Vector2Int> matchs, int combo)
    {
        Vector2 Pos = new Vector2(-3, 10);
        int Rand5X = Choose(-3, -2);
        Vector2 RandPos5 = new Vector2(Rand5X, 10);
        int Rand4X = Choose(-3, -1);
        Vector2 RandPos4 = new Vector2(Rand4X, 10);
        int Rand3X = Choose(-3, 0);
        Vector2 RandPos3 = new Vector2(Rand3X, 10);
        int Rand2X = Choose(-3, 1);
        Vector2 RandPos2 = new Vector2(Rand2X, 10);
        int Rand1X = Choose(-3, 2);
        Vector2 RandPos1 = new Vector2(Rand1X, 10);
        int GarbageBlockCount = matchs.Count * combo;
        print(GarbageBlockCount);
        while (GarbageBlockCount > 0) {
           if (GarbageBlockCount >= 30)
            {
                GarbageBlockCount -= 30;
                cubes[10, -3] = Instantiate(GarbageBlock[0], Pos, Quaternion.identity);
                print("Spawn 30");
            }
            if (GarbageBlockCount >= 24)
            {
                GarbageBlockCount -= 24;
                cubes[10, -3] = Instantiate(GarbageBlock[1], Pos, Quaternion.identity);
                print("Spawn 24");
            }
            if (GarbageBlockCount >= 18)
            {
                GarbageBlockCount -= 18;
                cubes[10, -3] = Instantiate(GarbageBlock[2], Pos, Quaternion.identity);
                print("Spawn 18");
            }
            if (GarbageBlockCount >= 12)
            {
                GarbageBlockCount -= 12;
                cubes[10, -3] = Instantiate(GarbageBlock[3], Pos, Quaternion.identity);
                print("Spawn 12");
            }
            if (GarbageBlockCount >= 6)
            {
                GarbageBlockCount -= 6;
                cubes[10, -3] = Instantiate(GarbageBlock[4], Pos, Quaternion.identity);
                print("Spawn 6");
            }
            if (GarbageBlockCount >= 5)
            {
                 GarbageBlockCount -= 5;
                cubes[10, Rand5X] = Instantiate(GarbageBlock[5], RandPos5, Quaternion.identity);
                print("Spawn 5");
            }
            if (GarbageBlockCount >= 4)
            {
                GarbageBlockCount -= 4;
                cubes[10, Rand4X] = Instantiate(GarbageBlock[6], RandPos4, Quaternion.identity);
                print("Spawn 4");
            }
            if (GarbageBlockCount >= 3)
            {
                GarbageBlockCount -= 3;
                cubes[10, Rand3X] = Instantiate(GarbageBlock[7], RandPos3, Quaternion.identity);
                print("Spawn 3");
            }
            if (GarbageBlockCount >= 2)
            {
                GarbageBlockCount -= 2;
                cubes[10, Rand2X] = Instantiate(GarbageBlock[8], RandPos2, Quaternion.identity);
                print("Spawn 2");
            }
            if (GarbageBlockCount >= 1)
            {
                GarbageBlockCount -= 1;
                cubes[10, Rand1X] = Instantiate(GarbageBlock[9], RandPos1, Quaternion.identity);
                print("Spawn 1");
            }
            else
                break;
            FallDown();
        }
        
    }*/

    
}
