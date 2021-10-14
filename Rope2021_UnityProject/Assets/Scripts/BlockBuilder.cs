using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBuilder : MonoBehaviour
{
    [SerializeField] GameObject solidBlockPrefab;
    [SerializeField] GameObject lavaBlockPrefab;

    [SerializeField] Transform blocksHolder;

    List<GameObject> solidBlocks;
    List<GameObject> lavaBlocks;

    // Start is called before the first frame update
    void Start()
    {
        solidBlocks = new List<GameObject>();
        lavaBlocks = new List<GameObject>();

        if (blocksHolder == null)
        {
            blocksHolder = new GameObject("BlocksHolder").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(1))
            TryToCreateSolidBlock();

        if (!Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(1))
            TryToCreateLavaBlock();

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(1))
            TryToDeleteBlock();
    }


    void TryToCreateSolidBlock()
    {
        Plane plane = new Plane(Vector3.back, 0);

        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            Vector3 worldPosition = ray.GetPoint(distance);
            worldPosition = new Vector3(Mathf.Round(worldPosition.x), Mathf.Round(worldPosition.y), Mathf.Round(worldPosition.z));

            if (!Physics2D.OverlapPoint(worldPosition))
            {
                CreateSolidBlock(worldPosition);
            }
        }
    }


    private void TryToCreateLavaBlock()
    {
        Plane plane = new Plane(Vector3.back, 0);

        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            Vector3 worldPosition = ray.GetPoint(distance);
            worldPosition = new Vector3(Mathf.Round(worldPosition.x), Mathf.Round(worldPosition.y), Mathf.Round(worldPosition.z));

            if (!Physics2D.OverlapPoint(worldPosition))
            {
                CreateLavaBlock(worldPosition);
            }
        }
    }


    void TryToDeleteBlock()
    {
        print("Sarted DeleteBlock()");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            print("Hit Object");

            if (hit.collider.gameObject.CompareTag("LevelEditorBlock"))
            {
                DeleteBlock(hit.collider.gameObject);
            }
        }
    }

    void CreateSolidBlock(Vector3 position)
    {
        GameObject newBlock = Instantiate(solidBlockPrefab, position, Quaternion.identity,blocksHolder);

        RegisterSolidBlock(newBlock);
    }


    void CreateLavaBlock(Vector3 position)
    {
        GameObject newBlock = Instantiate(lavaBlockPrefab, position, Quaternion.identity,blocksHolder);

        RegisterLavaBlock(newBlock);
    }


    void DeleteBlock(GameObject blockGO)
    {
        UnregisterBlock(blockGO);

        Destroy(blockGO);
    }


    void RegisterSolidBlock(GameObject block)
    {
        solidBlocks.Add(block);
    }
    

    private void RegisterLavaBlock(GameObject lavaBlock)
    {
        lavaBlocks.Add(lavaBlock);
    }


    void UnregisterBlock(GameObject block)
    {
        solidBlocks.Remove(block);
        lavaBlocks.Remove(block);
    }

    void ResetLevel()
    {
        foreach (GameObject go in solidBlocks)
        {
            Destroy(go);
        }

        foreach (GameObject go in lavaBlocks)
        {
            Destroy(go);
        }

        solidBlocks.Clear();
        lavaBlocks.Clear();
    }


    [ContextMenu("Save")]
    public void Save()
    {
        //SolidBlocks
        int numberOfSolidBlocks = solidBlocks.Count;

        Vector3[] solidBlocksPositions = new Vector3[numberOfSolidBlocks];
        for (int i = 0; i < numberOfSolidBlocks; i++)
        {
            GameObject go = solidBlocks[i];

            solidBlocksPositions[i] = go.transform.position;
        }

        SaveData.Instance.numberOfSolidBlocks = numberOfSolidBlocks;
        SaveData.Instance.solidBlocksPositions = solidBlocksPositions;

        //LavaBlocks
        int numberOfLavaBlocks = lavaBlocks.Count;

        Vector3[] lavaBlocksPositions = new Vector3[numberOfLavaBlocks];
        for (int i = 0; i < numberOfLavaBlocks; i++)
        {
            GameObject go = lavaBlocks[i];

            lavaBlocksPositions[i] = go.transform.position;
        }

        SaveData.Instance.numberOfLavaBlocks = numberOfLavaBlocks;
        SaveData.Instance.lavaBlocksPositions = lavaBlocksPositions;
    }


    [ContextMenu("Load")]
    public void Load()
    {
        ResetLevel();

        int numberOfSolidBlocks = SaveData.Instance.numberOfSolidBlocks;
        for(int i = 0; i < numberOfSolidBlocks; i++)
        {
            Vector3 position = new Vector3(
                SaveData.Instance.solidBlocksPositions[i].x,
                SaveData.Instance.solidBlocksPositions[i].y,
                SaveData.Instance.solidBlocksPositions[i].z
                );

            CreateSolidBlock(position);
        }

        int numberOfLavaBlocks = SaveData.Instance.numberOfLavaBlocks;
        for (int i = 0; i < numberOfLavaBlocks; i++)
        {
            Vector3 position = new Vector3(
                SaveData.Instance.lavaBlocksPositions[i].x,
                SaveData.Instance.lavaBlocksPositions[i].y,
                SaveData.Instance.lavaBlocksPositions[i].z
                );

            CreateLavaBlock(position);
        }
    }

}
