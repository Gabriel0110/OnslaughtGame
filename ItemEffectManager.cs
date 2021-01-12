using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectManager : MonoBehaviour
{
    [SerializeField] private GameObject playerTarget;
    [SerializeField] private GameObject spellEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.instance.GetComponent<PlayerManager>().hasTarget)
        {
            playerTarget = PlayerController.instance.GetComponent<PlayerManager>().playerTarget;
            //spellProjectile.GetComponent<ProjectileController>().target = playerTarget;
        }
    }
}
