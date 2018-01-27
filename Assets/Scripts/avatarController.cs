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

    FMOD.Studio.EventInstance collectible; //Instanciation du son
    FMOD.Studio.ParameterInstance agePourCollectible; //Instanciation du paramètre lié au son


    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        inventory = GetComponent<Inventory>();
        avatarLife = GetComponent<avatarLife>();

        collectible = FMODUnity.RuntimeManager.CreateInstance("event:/Avatar/Collectible"); // Chemin du son 
        collectible.getParameter("Age", out agePourCollectible); // Va chercher le paramètre FMOD "Age" et le stocke dans le paramètre "agePourCollectible".
        agePourCollectible.setValue(0.0f); // Valeur du paramètre en début de partie


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
        }
        if(!jumpAbility)
        {
            if(rigidbody.position.y > maximumJumpY)
            {
                maximumJumpY = rigidbody.position.y;
            }
        }
    }

    public void setAgePourCollectible(float value)
    {
        agePourCollectible.setValue(value);
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
            jumpAbility = true;
        }

    }

}
