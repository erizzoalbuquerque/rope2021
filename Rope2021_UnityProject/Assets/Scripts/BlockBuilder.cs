using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBuilder : MonoBehaviour
{
    [SerializeField] GameObject blockPrefab;
    Vector3 lastObjectCreatedPosition = Vector3.zero;

    List<GameObject> blocks;

    // Start is called before the first frame update
    void Start()
    {
        blocks = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(2))
            TryToCreateBlock();

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(2))
            TryToDeleteBlock();
    }


    void TryToCreateBlock()
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
                CreateBlock(worldPosition);
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
                RemoveBlockFromList(hit.collider.gameObject);

                Destroy(hit.collider.gameObject);
            }
        }
    }

    void CreateBlock(Vector3 position)
    {
        GameObject newBlock = Instantiate(blockPrefab, position, Quaternion.identity);

        AddBlockToList(newBlock);
    }

    void DeleteBlock(GameObject blockGO)
    {
        RemoveBlockFromList(blockGO);

        Destroy(blockGO);
    }


    void AddBlockToList(GameObject block)
    {
        blocks.Add(block);
    }


    void RemoveBlockFromList(GameObject block)
    {
        blocks.Remove(block);
    }

    void ResetLevel()
    {
        foreach (GameObject go in blocks)
        {
            Destroy(go);
        }

        blocks.Clear();
    }


    [ContextMenu("Save")]
    public void Save()
    {
        int numberOfBlocks = blocks.Count;

        Vector3[] positions = new Vector3[numberOfBlocks];
        for (int i = 0; i < numberOfBlocks; i++)
        {
            GameObject go = blocks[i];

            positions[i] = go.transform.position;
        }

        SaveData.Instance.numberOfBLocks = numberOfBlocks;
        SaveData.Instance.positions = positions;
    }


    [ContextMenu("Load")]
    public void Load()
    {
        ResetLevel();

        int numberOfBlocks = SaveData.Instance.numberOfBLocks;
        for(int i = 0; i < numberOfBlocks; i++)
        {
            Vector3 position = new Vector3(
                SaveData.Instance.positions[i].x,
                SaveData.Instance.positions[i].y,
                SaveData.Instance.positions[i].z
                );

            CreateBlock(position);
        }
    }

}
