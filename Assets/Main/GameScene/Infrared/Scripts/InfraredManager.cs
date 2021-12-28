using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfraredManager : ObjectPool<InfraredManager>
{
    public Camera infraredCamera;
    [Space]
    public Material allyIR;
    public Material enemyIR;
    public float minActiveDistance = 100f;
    public bool infraActive = false;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        infraActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
            infraActive = true;
        else if (Input.GetMouseButtonUp(2))
            infraActive = false;   
    }

    public void AddInfrared(BaseEntity baseEntity)
    {
        InfraredObject infraredObject = GetAvailableObject().GetComponent<InfraredObject>();
        infraredObject.ActivateInfrared(this, baseEntity);
    }

    protected override GameObject GetAvailableObject()
    {
        foreach (Transform transform in objectContainer)
        {
            if (!transform.gameObject.activeSelf && transform.GetComponent<InfraredObject>().active == false)
            {
                transform.gameObject.SetActive(true);
                return transform.gameObject;
            }
        }
        GameObject newGO = Instantiate(originalObject, objectContainer);
        newGO.SetActive(true);
        return newGO;
    }


}
