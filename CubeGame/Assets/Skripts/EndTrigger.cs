using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    public GameManager manager;

    void OnTriggerEnter()
    {
        manager.CompleteLevel();
    }
}
