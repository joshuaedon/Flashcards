using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneController : MonoBehaviour
{
    public GameObject ScrollViewContentObject;
    public GameObject CardTilePrefab;


    /* Start is called before the first frame update */
    void Start()
    {
        // Retrieve cards from spreadsheet if not already retrieved
        if(Card.cards == null)
            Card.RetrieveCards();

        // Populate cards in scroll view
        PopulateScrollView();
    }

    /* Update is called once per frame */
    void Update() { }

    /* Populates the ScrollView with tiles displaying all cards */
    private void PopulateScrollView() {
        float tileHeight = CardTilePrefab.GetComponent<RectTransform>().rect.height;

        // Add card GameObjects
        for(int i = 0; i<Card.cards.Count; i++) {
            Card card = Card.cards[i];
            GameObject cardTileObject = Instantiate(CardTilePrefab, ScrollViewContentObject.transform);
            CardTile cardTile = cardTileObject.GetComponent<CardTile>();
            cardTile.SetCard(card);
        }
    }

    /* Navigates to another scene */
    public void NavigateToScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
