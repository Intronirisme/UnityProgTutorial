using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StateMachine : MonoBehaviour
{
    public enum States
    {
        S_Search,
        S_Attack
    }
    public States defaultState;
    private MonoBehaviour _activeState;
    private GameObject _eyes_root;
    private ConeCollider _eyesight;

    // Start is called before the first frame update
    void Start()
    {
        //récupérer les références

        //des composants
        _eyes_root = gameObject.FindInChildren("Eyes");

        //des états
        switch (defaultState)
        {
            case States.S_Search :
                _activeState = GetComponent<S_Search>();
                break;

            case States.S_Attack :
                _activeState = GetComponent<S_Attack>();
                break;

            default:
                Debug.Log("Miconfigured state");
                break;
        }

        if(_activeState != null)
        {
            Debug.Log("state machine ready");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}