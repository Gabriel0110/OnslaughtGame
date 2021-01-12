using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public GameObject target;
    public GameObject spellEffect;
    private float speed = 50f;
    public float spellDamage;
    private Collider[] hitColliders;

    private GameObject spellEffectInstance;

    public DuloGames.UI.UISpellInfo spellInfo;

    // Start is called before the first frame update
    void Start()
    {
        spellEffectInstance = Instantiate(spellEffect, transform.position, PlayerController.instance.transform.rotation, transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (spellEffectInstance != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position + new Vector3(0f, 1f, 0f), Time.deltaTime * speed);
            spellEffectInstance.transform.position = transform.position;
            spellEffectInstance.transform.rotation = transform.rotation;

            hitColliders = Physics.OverlapSphere(transform.position, 0.1f);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject == target)
                {
                    target.GetComponent<NPCManager>().damageTargetNPC(spellDamage, spellInfo);
                    PlayerController.instance.GetComponent<SpellManager>().destroyProjectile();
                    Destroy(spellEffectInstance);
                }
            }
        }
        
    }

    public void setTarget(GameObject target)
    {
        this.target = target;
    }

    public void setSpellEffect(GameObject effect)
    {
        this.spellEffect = effect;
    }

    public void setSpeed(float speed)
    {
        this.speed = speed;
    }

    public void setSpellDamage(float amount)
    {
        this.spellDamage = amount;
    }

    public void setSpellInfo(DuloGames.UI.UISpellInfo spellInfo)
    {
        spellInfo = spellInfo;
    }
}
