using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PunObjectPool<T> : ObjectPool<T>
{
    protected override void InstantiateObjects()
    {
        for (int i = 0; i < defaultNumberOfObjects; ++i)
            PhotonNetwork.Instantiate(originalObject.name, Vector3.zero, Quaternion.identity);

    }

    protected override GameObject GetAvailableObject()
    {
        throw new System.NotImplementedException();
    }


}
