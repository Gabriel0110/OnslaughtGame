using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    public DuloGames.UI.UISpellInfo spellInfo;
    public float spellDuration = 0f;
    public float timer = 0f;
    public Collider[] hitColliders = null;

    // Start is called before the first frame update
    void Start()
    {
        
        if (spellInfo != null)
        {
            InvokeRepeating("Effect", 0.01f, 1f);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spellDuration != 0f)
        {
            if (timer < spellDuration)
            {
                timer += Time.deltaTime;
                if (timer >= spellDuration)
                {
                    CancelInvoke("Effect");
                    Destroy(this.gameObject);

                }
            }
        }
    }

    public void Effect()
    {
        hitColliders = Physics.OverlapSphere(this.transform.position, spellInfo.AOERange);
        if (spellInfo.Name == "Grounded Darkness" && hitColliders != null)
        {
            PlayerController.instance.GetComponent<SpellManager>().GroundedDarknessEffect(hitColliders, spellInfo);
        }

        if (spellInfo.Name == "Gravity Field" && hitColliders != null)
        {
            PlayerController.instance.GetComponent<SpellManager>().GravityFieldEffect(hitColliders, spellInfo);
        }
    }

}
