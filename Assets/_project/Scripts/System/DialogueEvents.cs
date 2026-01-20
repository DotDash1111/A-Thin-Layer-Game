using SuperAnimatedDialogue.Runtime;
using UnityEngine;

public class DialogueEvents : MonoBehaviour
{
    public DialogueSystem dialogueSystem;
    public CharacterMovement characterMovement;
    public GameObject altar;

    void Update()
    {
        if (dialogueSystem.isEndDialogue == true && dialogueSystem != null && characterMovement != null)
        {
            characterMovement.AllowMovement = true;
            characterMovement.allowJump = true;
            altar.SetActive(true);
        }
    }
}
