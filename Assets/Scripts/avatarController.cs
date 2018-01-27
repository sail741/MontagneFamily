﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class avatarController : MonoBehaviour
{
    public float moveSpeed = 2;
    public float jumpForce = 20;

    private bool jumpAbility = true;
    private Rigidbody2D rigidbody;
    private avatarLife avatarLife;
    private float maximumJumpY;

    private Inventory inventory;

    //*** Sons ***//

    FMOD.Studio.EventInstance collectible; //Instanciation du son
    FMOD.Studio.ParameterInstance agePourCollectible; //Instanciation du paramètre lié au son

    FMOD.Studio.EventInstance saut;
    FMOD.Studio.ParameterInstance agePourSaut;

    FMOD.Studio.EventInstance reception;
    FMOD.Studio.ParameterInstance agePourReception;

    FMOD.Studio.EventInstance receptionTropHaut;
    FMOD.Studio.ParameterInstance agePourReceptionTropHaut;


    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        inventory = GetComponent<Inventory>();
        avatarLife = GetComponent<avatarLife>();

        //*** Sons ***//

        collectible = FMODUnity.RuntimeManager.CreateInstance("event:/Avatar/Collectible"); // Chemin du son 
        collectible.getParameter("Age", out agePourCollectible); // Va chercher le paramètre FMOD "Age" et le stocke dans le paramètre "agePourCollectible".
        agePourCollectible.setValue(0.0f); // Valeur du paramètre en début de partie

        saut = FMODUnity.RuntimeManager.CreateInstance("event:/Avatar/Saut");
        saut.getParameter("Age", out agePourSaut); 
        agePourSaut.setValue(0.0f);

        reception = FMODUnity.RuntimeManager.CreateInstance("event:/Avatar/Reception");
        reception.getParameter("Age", out agePourReception);
        agePourReception.setValue(0.0f);

        receptionTropHaut = FMODUnity.RuntimeManager.CreateInstance("event:/Avatar/Reception_Trop_Haut");
        receptionTropHaut.getParameter("Age", out agePourReceptionTropHaut);
        agePourReceptionTropHaut.setValue(0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        jump();
    }

    void movement()
    {
        float h = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        transform.Translate(Vector2.right * h);
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpAbility)
        {
            rigidbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            maximumJumpY = rigidbody.position.y;
            jumpAbility = false;
            FMODUnity.RuntimeManager.PlayOneShot("event:/Avatar/Saut"); // Joue le son une fois
        }
        if(!jumpAbility)
        {
            if(rigidbody.position.y > maximumJumpY)
            {
                maximumJumpY = rigidbody.position.y;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Collision Recoltable
        if (collision.gameObject.tag == "Recoltable")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Avatar/Collectible"); // Joue le son une fois
            ItemController item = collision.gameObject.GetComponent<ItemController>();
            item.ApplyEffect(gameObject);
       
            inventory.GetItem(item);
            print("get an item !");
            Destroy(collision.gameObject);
        } else if(collision.gameObject.tag == "Plateform")
        {

            //ne marche pas tjs
            float highFall = maximumJumpY - rigidbody.velocity.y;

            if (highFall >= 6)
            {
                avatarLife.TakeDamage(5);
            } else if (highFall >= 5)
            {
                avatarLife.TakeDamage(4);
            } else if (highFall >= 4)
            {
                avatarLife.TakeDamage(2);
            } else if (highFall >= 3)
            {
                avatarLife.TakeDamage(1);
            }

            if (!jumpAbility)
            {
                jumpAbility = true;
            
                if(highFall < 3)
                {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Avatar/Reception"); // Joue le son une fois
                } else if(avatarLife.currentLife > 0)
                {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Avatar/Reception_Trop_Haut");
                }
                
            }
            
        }

    }

    public void setAllAgePourAudio(float value)
    {
        agePourCollectible.setValue(value);
        agePourSaut.setValue(value);
        agePourReception.setValue(value);
        agePourReceptionTropHaut.setValue(value);
    }

}
