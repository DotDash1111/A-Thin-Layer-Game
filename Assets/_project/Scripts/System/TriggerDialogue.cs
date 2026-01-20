using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{
    public GameObject dialoguePanel;
    public CharacterMovement characterMovement;

    void OnTriggerEnter()
    {
        dialoguePanel.SetActive(true);
        characterMovement.AllowMovement = false;
        characterMovement.allowJump = false;
        Destroy(gameObject);
    }	
}
