using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO: check how the buildings are being destroyed - should add trees to a parent or leave them loose
public class RuralGenerator : MonoBehaviour {

    public List<Transform> trees = null;

    public int treesPerCluster = 2;
    public int numTiles = 3;
    public float treeScaleMin = 0.7f;
    public float treeScaleMax = 1.2f;
    public float chanceOfClusterSpawning = 0.7f;

    private Transform _parentTransform = null;
    private int _initialSpacing = 20;
    private int _densityLeft;
    private int _densityRight;
    private int _densityRateOfChange;

    public void RuralLandscapeGeneration(Vector3 trackLocation, bool isBranchBlock, Transform parentTransform, bool isLevelChange = false)
    {
        _parentTransform = parentTransform;

        if (isLevelChange)
        {
            _initialSpacing = 23;
        }
        else
        {
            _initialSpacing = 19;
        }

        if (trackLocation.z > 0.0f)
        {
            CreateLeftSideClusters(new Vector3(trackLocation.x, trackLocation.y, trackLocation.z - 15.0f));
        }
        else if (trackLocation.z < 0.0f)
        {
            CreateRightSideClusters(new Vector3(trackLocation.x, trackLocation.y, trackLocation.z + 15.0f));
        }
        else
        {
            if (isBranchBlock)
            {
                CreateLeftSideClusters(new Vector3(trackLocation.x, trackLocation.y, trackLocation.z - 7.5f));
                CreateRightSideClusters(new Vector3(trackLocation.x, trackLocation.y, trackLocation.z + 7.5f));
            }
            else
            {
                CreateLeftSideClusters(trackLocation);
                CreateRightSideClusters(trackLocation);
            }
        }
    }

    private void CreateLeftSideClusters(Vector3 trackLocation)
    {
        Vector3 location = new Vector3(trackLocation.x, trackLocation.y, trackLocation.z - _initialSpacing);
        
        //_densityLeft = SetDensity(_densityLeft);

        //SetCurrentTrees(_currentLeftTrees);
        
        for (int i = 0; i < numTiles; i++) // adds a row of tiles in z direction at x position
        {
            AddClusters(location, treesPerCluster, true);
            location.z = location.z - 20.0f;
        }
    }

    private void CreateRightSideClusters(Vector3 trackLocation)
    {
        Vector3 tileArea = new Vector3(trackLocation.x, trackLocation.y, trackLocation.z + _initialSpacing);

        //_densityRight = SetDensity(_densityRight);

        //SetCurrentTrees(_currentRightTrees);

        for (int i = 0; i < numTiles; i++)
        {
            AddClusters(tileArea, treesPerCluster, false);
            tileArea.z = tileArea.z + 20.0f;
        }
    }

    private void AddClusters(Vector3 tileArea, int density, bool isLeft)
    {
        float rand = Random.Range(0.0f, 1.0f);
        if (rand < chanceOfClusterSpawning)
        {
            AddTrees(new Vector3(tileArea.x - 5.0f, tileArea.y, tileArea.z - 5.0f), isLeft);
        }
        rand = Random.Range(0.0f, 1.0f);
        if (rand < chanceOfClusterSpawning)
        {
            AddTrees(new Vector3(tileArea.x + 5.0f, tileArea.y, tileArea.z - 5.0f), isLeft);
        }
        rand = Random.Range(0.0f, 1.0f);
        if (rand < chanceOfClusterSpawning)
        {
            AddTrees(new Vector3(tileArea.x - 5.0f, tileArea.y, tileArea.z + 5.0f), isLeft);
        }
        rand = Random.Range(0.0f, 1.0f);
        if (rand < chanceOfClusterSpawning)
        {
            AddTrees(new Vector3(tileArea.x + 5.0f, tileArea.y, tileArea.z + 5.0f), isLeft);
        }    
    }

    private void AddTrees(Vector3 clusterArea, bool isLeft)
    {
        for (int i = 0; i < treesPerCluster; i++)
        {                        
            Transform newTree;

            // randomly select one of the trees in the list
            int rand = Random.Range(0, trees.Count);

            newTree = Instantiate(trees[rand]) as Transform;

            // random rotation
            newTree.transform.eulerAngles = new Vector3(newTree.transform.eulerAngles.x,
                newTree.transform.eulerAngles.y + Random.Range(0, 360), newTree.transform.eulerAngles.z);

            // random scaling
            newTree.transform.localScale = newTree.transform.localScale * Random.Range(treeScaleMin, treeScaleMax);

            // find a vacant position and set
            float displacement = newTree.GetComponent<Renderer>().bounds.extents.z;
            if (isLeft)
            {
                displacement = -displacement;
            }
            
            Vector3 treePosition = GetRandomPosition(clusterArea) + new Vector3(0, 0, displacement);
            
            if (Time.time != 0)
            {
                
                if (isLeft)
                {
                    newTree.transform.position = treePosition + new Vector3(0, -20f, -15f); // add bounds and lerp difference
                    SpawnedObjectLerper lerper = newTree.gameObject.AddComponent<SpawnedObjectLerper>();
                    lerper.Initialise(20f, 15f);
                }
                else
                {
                    newTree.transform.position = treePosition + new Vector3(0, -20f, 15f); // add bounds and lerp difference
                    SpawnedObjectLerper lerper = newTree.gameObject.AddComponent<SpawnedObjectLerper>();
                    lerper.Initialise(20f, -15f);
                }
                
            }
            else
            {
                newTree.transform.position = treePosition + new Vector3(0, 0, displacement); // add bounds
            }

            //newTree.name = "Tree " + newTree.transform.position.x + ", " + newTree.transform.position.z; // use if need tree position later

            newTree.transform.parent = _parentTransform;            
        }
    }

    Vector3 GetRandomPosition(Vector3 clusterArea)
    {
        return new Vector3(clusterArea.x + Random.Range(-5.0f, 5.0f), 0,
                   clusterArea.z + Random.Range(-5.0f, 5.0f));
    }




    ///////////////////////////////////////////////////////////
    ////////// Currently unused functions.////////////////////
    //////////////////////////////////////////////////////////

    //private int SetDensity(int density)
    //{
    //    int rand = Random.Range(-1, 2);

    //    switch (rand)
    //    {
    //        case -1:
    //            density = Mathf.Max(density - _densityRateOfChange, minTrees);
    //            break;
    //        case 0:
    //            break;
    //        case 1:
    //            density = Mathf.Min(density + _densityRateOfChange, maxTrees);
    //            break;
    //    }

    //    return density;
    //}

    //private void SetCurrentTrees(List<Transform> currentTrees)
    //{
    //    int rand = Random.Range(-1, 2);

    //    switch (rand)
    //    {
    //        case -1:
    //            // remove a tree
    //            if (currentTrees.Count > minVariation)
    //            {
    //                int r = Random.Range(0, currentTrees.Count);
    //                currentTrees.RemoveAt(r);
    //            }
    //            break;
    //        case 0:
    //            // do nothing
    //            // or for more variation switch a tree with another
    //            break;
    //        case 1:
    //            // add a tree
    //            if (currentTrees.Count < maxVariation)
    //            {
    //                Transform t;
    //                bool treeAdded = false;
    //                while (!treeAdded)
    //                {
    //                    t = trees[Random.Range(0, trees.Count)];
    //                    if (!currentTrees.Contains(t))
    //                    {
    //                        currentTrees.Add(t);
    //                        treeAdded = true;
    //                    }
    //                }
    //            }
    //            break;
    //    }
    //} 


    //void Awake()
    //{
    //_densityRateOfChange = (maxTrees - minTrees) / densityRateOfChange;
    //_densityLeft = Random.Range(minTrees, maxTrees);
    //_densityRight = Random.Range(minTrees, maxTrees);

    //_currentLeftTrees = new List<Transform>();
    //_currentRightTrees = new List<Transform>();

    //PopulateCurrentTreeLists();
    //}

    //private void PopulateCurrentTreeLists()
    //{
    //    if (maxVariation > trees.Count) maxVariation = trees.Count;

    //    int rand1 = Random.Range(minVariation, maxVariation + 1);
    //    int rand2 = Random.Range(minVariation, maxVariation + 1);

    //    while (_currentLeftTrees.Count < rand1)
    //    {
    //        Transform t;
    //        bool treeAdded = false;
    //        while (!treeAdded)
    //        {
    //            t = trees[Random.Range(0, trees.Count)];
    //            if (!_currentLeftTrees.Contains(t))
    //            {
    //                _currentLeftTrees.Add(t);
    //                treeAdded = true;
    //            }
    //        }
    //    }

    //    while (_currentRightTrees.Count < rand2)
    //    {
    //        Transform t;
    //        bool treeAdded = false;
    //        while (!treeAdded)
    //        {
    //            t = trees[Random.Range(0, trees.Count)];
    //            if (!_currentRightTrees.Contains(t))
    //            {
    //                _currentRightTrees.Add(t);
    //                treeAdded = true;
    //            }
    //        }
    //    }
    //}


    //public int minTrees = 0;
    //public int maxTrees = 15;
    //public int densityRateOfChange = 5;
    //public int minVariation = 2;
    //public int maxVariation = 6;

    //private List<Transform> _currentLeftTrees;
    //private List<Transform> _currentRightTrees;
}
