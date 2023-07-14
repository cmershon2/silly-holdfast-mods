using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class pokemonBase : MonoBehaviour
{
    [SerializeField]
    PokemonIndex pokemonAnimationIndex = new PokemonIndex();

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        // Fetch local gameObject"s animator
        anim = gameObject.GetComponent<Animator>();
        
        switch(pokemonAnimationIndex)
        {
            case PokemonIndex.Eevee:
                anim.SetInteger("pokemon_index", 0);
                break;
            case PokemonIndex.Grookey:
                anim.SetInteger("pokemon_index", 1);
                break;
            case PokemonIndex.Jigglypuff:
                anim.SetInteger("pokemon_index", 2);
                break;
            case PokemonIndex.Jirachi:
                anim.SetInteger("pokemon_index", 3);
                break;
            case PokemonIndex.Mew:
                anim.SetInteger("pokemon_index", 4);
                break;
            case PokemonIndex.Pachirisu:
                anim.SetInteger("pokemon_index", 5);
                break;
            case PokemonIndex.Pikachu:
                anim.SetInteger("pokemon_index", 6);
                break;
            case PokemonIndex.Roselia:
                anim.SetInteger("pokemon_index", 7);
                break;
            case PokemonIndex.Spinda:
                anim.SetInteger("pokemon_index", 8);
                break;

            default:
                Debug.LogError("Pokemon Base: unknown pokemon index.");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

enum PokemonIndex
{
    Eevee,
    Grookey,
    Jigglypuff,
    Jirachi,
    Mew,
    Pachirisu,
    Pikachu,
    Roselia,
    Spinda
}