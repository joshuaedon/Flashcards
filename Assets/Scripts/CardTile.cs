using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class CardTile : MonoBehaviour
{
    public GameObject aTextObject;
    public GameObject bTextObject;
    public Sprite[] batteries;
    public GameObject aBatteryObject;
    public GameObject bBatteryObject;

    public Card card;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }
    
    /* Called after creation, sets the card and display text on the tile */
    public void SetCard(Card card) {
        this.card = card;

        // Set words
        aTextObject.GetComponent<RTLTextMeshPro>().text = card.aWord;
        bTextObject.GetComponent<RTLTextMeshPro>().text = card.bWord;

        // Set battery icons - the icon next to the word symbolises the duration translating FROM that word
        aBatteryObject.GetComponent<UnityEngine.UI.Image>().sprite = batteries[
            card.abRevisionStep < 60 ? 0 : // < 1 hour
            card.abRevisionStep < 1440 ? 1 : // < 1 day
            card.abRevisionStep < 43200 ? 2 : // < 1 month
            card.abRevisionStep < 525960 ? 3 : 4 // < 1 year
        ];
        bBatteryObject.GetComponent<UnityEngine.UI.Image>().sprite = batteries[
            card.baRevisionStep < 60 ? 0 : 
            card.baRevisionStep < 1440 ? 1 : 
            card.baRevisionStep < 43200 ? 2 :
            card.baRevisionStep < 525960 ? 3 : 4
        ];
    }

    public void DropdownUpdated(Dropdown change) {
        switch(change.value) {
            case 0: { /* Edit */

                EditCardSceneController.card = this.card;
                SceneManager.LoadScene("EditCardScene");
                break;

            } case 1: { /* Reset */

                this.card.SetDefaultStats();
                Card.WriteCardsToCSV();
                SetCard(this.card);
                break;

            } case 2: { /* Delete */

                // Remove the Card from the list and save all cards to the CSV
                Card.cards.Remove(this.card);
                Card.WriteCardsToCSV();
                // Remove the Card Tile GameObject
                Destroy(gameObject);
                break;

            }
        }
    }
}
