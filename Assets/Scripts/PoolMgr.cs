using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolMgr : MonoSingleton<PoolMgr>
{
    [SerializeField] private GameObject objectContainer;
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private List<GameObject> objectPool;

    public override void Init()
    {
        base.Init();
        // Awake Initializations here.
    }

    void Start()
    {
        objectPool = GenerateObjects(10);
    }

    void Update()
    {
    }

    List<GameObject> GenerateObjects(int objectCount, bool active = false)
    {
        for (int i = 0; i < objectCount; i++)
        {
            GameObject obj = Instantiate(objectPrefab);
            obj.transform.parent = objectContainer.transform;
            if (!active) obj.SetActive(false);
            objectPool.Add(obj);
        }
        return objectPool;
    }

    public GameObject RequestObject()
    {
        foreach (var obj in objectPool)
        {
            if (obj.activeInHierarchy == false)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        GenerateObjects(1, true);
        return null;
    }

}
