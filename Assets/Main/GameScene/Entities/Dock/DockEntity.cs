using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DockEntity : MonoBehaviour
{
    [HideInInspector]
    public BaseEntity baseEntity;

    public List<EntityHealth> warehouses;

    int totalWarehouses;

    private void Start()
    {
        baseEntity = GetComponent<BaseEntity>();
        totalWarehouses = warehouses.Count;
    }

    /// <summary>
    /// Returns percentage of current warehouses/initial warehouses. 10% = 0.1f;
    /// </summary>
    /// <returns>(float)Percentage</returns>
    public float GetPercentageOfExistingWarehouses()
    {
        int numberOfWarehouses = 0;
        foreach (EntityHealth warehouse in warehouses)
        {
            if (warehouse.currHealth > 0f)
                ++numberOfWarehouses;
        }

        return numberOfWarehouses / totalWarehouses;

    }
}
