using UnityEngine;
using System.Collections;

public class TerrainSelector : MonoBehaviour {

    public Terrain sandyTerrain = null;
    public Terrain grassyTerrain = null;
    public Terrain pebbleTerrain = null;
    [HideInInspector]
    public Terrain currentTerrain = null;

    void Start()
    {
        if (sandyTerrain == null)
        {
            Debug.Log("Sandy Terrain not attached to Terrain Selector.");
        }

        if (grassyTerrain == null)
        {
            Debug.Log("Grassy Terrain not attached to Terrain Selector.");
        }

        if (pebbleTerrain == null)
        {
            Debug.Log("Pebble Terrain not attached to Terrain Selector.");
        }
    }

    public void DisableAllEnvironments()
    {
        sandyTerrain.gameObject.SetActive(false);
        grassyTerrain.gameObject.SetActive(false);
        pebbleTerrain.gameObject.SetActive(false);
    }

    public void EnableSandyTerrain()
    {
        grassyTerrain.gameObject.SetActive(false);
        pebbleTerrain.gameObject.SetActive(false);
        sandyTerrain.gameObject.SetActive(true);

        currentTerrain = sandyTerrain;
    }

    public void EnableGrassyTerrain()
    {
        sandyTerrain.gameObject.SetActive(false);
        pebbleTerrain.gameObject.SetActive(false);
        grassyTerrain.gameObject.SetActive(true);

        currentTerrain = grassyTerrain;
    }

    public void EnablePebbleTerrain()
    {
        sandyTerrain.gameObject.SetActive(false);
        grassyTerrain.gameObject.SetActive(false);
        pebbleTerrain.gameObject.SetActive(true);

        currentTerrain = pebbleTerrain;
    }
}
