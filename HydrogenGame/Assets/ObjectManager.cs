using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] GameObject[] items;
    private ObjectPool<GameObject>[] itemPools;
    private const int maxItems = 40;
    private bool item3OnScreen = false;

    public List<GameObject> activeItems = new List<GameObject>();

    void Awake()
    {
        itemPools = new ObjectPool<GameObject>[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            int index = i;
            itemPools[i] = new ObjectPool<GameObject>(
                () => CreatePoolObject(index),
                OnGetFromPool,
                OnReleaseToPool,
                OnDestroyPoolObject,
                collectionCheck: true
            );
        }
    }

    void Start()
    {
        for (int i = 0; i < maxItems; i++)
        {
            Vector3 spanPos = new Vector3(Random.Range(-2.5f, 2.5f), 5, 0);
            GameObject newItem = GetPoolItem(spanPos);
            activeItems.Add(newItem);
        }
    }

    private GameObject CreatePoolObject(int index)
    {
        GameObject obj = Instantiate(items[index]);
        obj.tag = "Item" + index;
        obj.AddComponent<BoxCollider2D>();
        obj.AddComponent<ItemClickHandler>().Initialize(this);
        return obj;
    }

    private void OnGetFromPool(GameObject target)
    {
        target.SetActive(true);
    }

    private void OnReleaseToPool(GameObject target)
    {
        if (target.CompareTag("Item3"))
        {
            item3OnScreen = false;
        }
        target.SetActive(false);
    }

    private void OnDestroyPoolObject(GameObject target)
    {
        Destroy(target);
    }

    public GameObject GetPoolItem(Vector3 spanPos)
    {
        int randomIndex = ChooseItemIndex();
        GameObject obj = itemPools[randomIndex].Get();
        obj.transform.position = spanPos;

        if (randomIndex == 3)
        {
            item3OnScreen = true;
        }

        return obj;
    }

    private int ChooseItemIndex()
    {
        float randomValue = Random.value;
        //if (!item3OnScreen)
        //{
        //    return 3;
        //}

        if (randomValue < 0.4f)
        {
            return 0;
        }
        else if (randomValue < 0.8f)
        {
            return 1;
        }
        else if (randomValue < 0.99f)
        {
            return 2;
        }
        else 
        {
            return 3;
        }

    }

    public void ReleaseItem(GameObject obj)
    {
        for (int i = 0; i < itemPools.Length; i++)
        {
            if (obj.CompareTag("Item" + i))
            {
                itemPools[i].Release(obj);
                return;
            }
        }
    }

    public void ItemClicked(GameObject obj)
    {
        if (activeItems.Contains(obj))
        {
            activeItems.Remove(obj);
            ReleaseItem(obj);

            // 新しいアイテムを生成してリストに追加
            Vector3 spanPos = new Vector3(Random.Range(-2.5f, 2.5f), 5, 0);
            GameObject newItem = GetPoolItem(spanPos);
            activeItems.Add(newItem);
        }
    }
}

public class ItemClickHandler : MonoBehaviour
{
    private ObjectManager gamePool;

    public void Initialize(ObjectManager pool)
    {
        gamePool = pool;
    }

    void OnMouseDown()
    {
        //gamePool.ItemClicked(gameObject);
    }
}
