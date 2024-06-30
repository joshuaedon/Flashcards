using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using RTLTMPro;
using TMPro;


public class EditCardSceneController : MonoBehaviour
{
    public GameObject aTextObject;
    public GameObject bTextObject;
    public GameObject aInputFieldObject;
    public GameObject bInputFieldObject;

    public static Card card;

    /* Start is called before the first frame update */
    void Start() { 
        if(card == null)
            return;

        aInputFieldObject.GetComponent<TMP_InputField>().text = card.aWord;
        bInputFieldObject.GetComponent<TMP_InputField>().text = card.bWord;
        aTextObject.GetComponent<RTLTextMeshPro>().UpdateText();
        bTextObject.GetComponent<RTLTextMeshPro>().UpdateText();
    }

    /* Update is called once per frame */
    void Update() { }

    public void Save() {
        if(card == null) { // New Card

            Card newCard = new Card(aTextObject.GetComponent<RTLTextMeshPro>().OriginalText, bTextObject.GetComponent<RTLTextMeshPro>().OriginalText);
            Card.cards.Add(newCard);

            print($"Added new word: {newCard.aWord}, {newCard.bWord}");
            
        } else { // Existing Card

            card.aWord = aTextObject.GetComponent<RTLTextMeshPro>().OriginalText;
            card.bWord = bTextObject.GetComponent<RTLTextMeshPro>().OriginalText;

            print($"Updated word: {card.aWord}, {card.bWord}");
        }
        
        Card.WriteCardsToCSV();

        EditCardSceneController.card = null;

        NavigateToScene("MainScene");
    }

    /* Navigates to another scene */
    public void NavigateToScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
