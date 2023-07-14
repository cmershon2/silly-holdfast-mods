using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InteractionTalk : MonoBehaviour
{
    public GameObject InteractionCanvas;
    public Text InteractionUIText;
    public AudioSource InteractionAudio;
    public float InteractionInputCooldown = 5.0f;
    public Color InteractionTextColorDefault;
    public Color InteractionTextColorCooldown;
    public string InteractionText;
    public string InteractionCooldownText;
    public AudioClip[] InteractionAudioClips;
    public bool pokemonAnimatedInteraction;
    public float pokemonAnimatedInteractionTime = 2.0f;
    public Material pokemonDefaultEyes;
    public Material pokemonDefaultMouth;
    public Material pokemonInteractionEyes;
    public Material pokemonInteractionMouth;
    public Renderer[] pokemonEyes;
    public Renderer pokemonMouth;

    private bool _canInteract;
    private bool _interactCooldown;

    // Start is called before the first frame update
    void Start()
    {
        InteractionCanvas.SetActive(false);
        InteractionUIText.text = InteractionText;
    }

    // Update is called once per frame
    void Update()
    {
        if(_canInteract==true && _interactCooldown==false)
        {
            InteractionUIText.color = InteractionTextColorDefault;
            InteractionUIText.text = InteractionText;
            if (Input.GetKeyUp(KeyCode.E))
            {
                InteractionAudio.PlayOneShot(RandomIdleAudioClip());
                Invoke("ResetCooldown", InteractionInputCooldown);
                _interactCooldown = true;

                if(pokemonAnimatedInteraction == true)
                {
                    StartCoroutine(ChangeMaterial());
                }
            }
        }
        else if (_canInteract==true && _interactCooldown==true)
        {
            InteractionUIText.color = InteractionTextColorCooldown;
            InteractionUIText.text = InteractionCooldownText;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        InteractionCanvas.SetActive(true);
        _canInteract = true;
    }

    public void OnTriggerExit(Collider other)
    {
        InteractionCanvas.SetActive(false);
        _canInteract = false;
    }

    private void ResetCooldown(){
        _interactCooldown = false;
    }

    // return a random idle audio clip
    AudioClip RandomIdleAudioClip(){
        return InteractionAudioClips[Random.Range(0, InteractionAudioClips.Length)];
    }

    IEnumerator ChangeMaterial()
    {
        foreach(Renderer eye in pokemonEyes){
            eye.material = pokemonInteractionEyes;
        }
        pokemonMouth.material = pokemonInteractionMouth;
        yield return new WaitForSeconds(pokemonAnimatedInteractionTime);
        foreach(Renderer eye in pokemonEyes){
            eye.material = pokemonDefaultEyes;
        }
        pokemonMouth.material = pokemonDefaultMouth;
    }
}
