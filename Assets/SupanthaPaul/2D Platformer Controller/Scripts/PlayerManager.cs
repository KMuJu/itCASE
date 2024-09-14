using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public Transform target;
    [SerializeField] public Transform startPosition;
    void Start()
    {
        target = startPosition;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
