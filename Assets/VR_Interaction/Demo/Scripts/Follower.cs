using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Follower : NetworkBehaviour
{
    [SerializeField] private Transform pos;


    private void Update()
    {
        if (isOwned)
        {
            transform.position = pos.position;
            transform.rotation = pos.rotation;
        }
    }
}
