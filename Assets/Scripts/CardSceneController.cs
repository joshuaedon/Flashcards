using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using RTLTMPro;
using TMPro;

public class CardSceneController : MonoBehaviour
{

    public GameObject fromTextObject;
    public GameObject toTextObject;
    public GameObject cardCountTextObject;
    public GameObject correctButtonObject;
    public GameObject incorrectButtonObject;

    private List<CardInstance> cardsToRevise;
    private CardInstance currentCard;

    /* Start is called before the first frame update */
    void Start()
    {
        SelectRandomCard();
    }

    /* Update is called once per frame */
    void Update()
    {

    }

    /* Gets list of words that are ready to be revised */
    private void GetWordsToRevise() {

        // Retrieve cards from spreadsheet if not already retrieved
        if(Card.cards == null)
            Card.RetrieveCards();

        cardsToRevise = new List<CardInstance>();
        DateTime now = System.DateTime.Now;

        foreach(Card card in Card.cards) {
            // If either revision dates are in the past, create and add a card instance to revise
            if(card.abRevisionDate < now)
                cardsToRevise.Add(new CardInstance(true, card));
            if(card.baRevisionDate < now)
                cardsToRevise.Add(new CardInstance(false, card));      
        }

        print($"Cards: {Card.cards.Count}, Variations to revise: {cardsToRevise.Count}");
    }

    public void MarkCorrect() {

        // Set the next revision date to the current date + the step and double the step so that the next appearance of the word is twice as far
        if(currentCard != null && currentCard.ab) {
            currentCard.card.abRevisionDate = DateTime.Now.AddMinutes(currentCard.card.abRevisionStep);
            currentCard.card.abRevisionStep *= 2;
        } else if (currentCard != null) {
            currentCard.card.baRevisionDate = DateTime.Now.AddMinutes(currentCard.card.baRevisionStep);
            currentCard.card.baRevisionStep *= 2;
        }

        // Set these updated values in the csv
        Card.WriteCardsToCSV();

        SelectRandomCard();
    }

    public void MarkIncorrect() {

        // Set the next revision date to the current date and half the step
        if(currentCard != null && currentCard.ab) {
            currentCard.card.abRevisionDate = DateTime.Now;
            currentCard.card.abRevisionStep = Math.Max(1, currentCard.card.abRevisionStep / 2);
        } else if(currentCard != null) {
            currentCard.card.baRevisionDate = DateTime.Now;
            currentCard.card.baRevisionStep = Math.Max(1, currentCard.card.baRevisionStep / 2);
        }

        // Set these updated values in the csv
        Card.WriteCardsToCSV();

        // Readd the card to the list
        if(currentCard != null)
            cardsToRevise.Add(currentCard);

        // Randomely select the next card
        SelectRandomCard();
    }

    public void showAnswer() {
        toTextObject.SetActive(true);
        correctButtonObject.SetActive(true);
        incorrectButtonObject.SetActive(true);
    }

    private void SelectRandomCard() {
        // Find card to revise if there are no cards in the list
        if((cardsToRevise?.Count ?? 0) == 0) {
            GetWordsToRevise();
            if(cardsToRevise.Count == 0) {
                SetCard(null);
                return;
            }
        }

        // Get a random card and set it to the current
        // Keep selecting a random card until it is not the previously selected card
        int randomI = UnityEngine.Random.Range(0, cardsToRevise.Count);
        while(cardsToRevise.Count > 1 && cardsToRevise[randomI] == currentCard)
            randomI = UnityEngine.Random.Range(0, cardsToRevise.Count);
        SetCard(cardsToRevise[randomI]);

        // Remove the card from the list (this will be readded if answered incorrectly)
        cardsToRevise.RemoveAt(randomI);

        SetCardCount();
    }

    private void SetCard(CardInstance cardInstance) {
        currentCard = cardInstance;

        if(cardInstance == null) {
            fromTextObject.GetComponent<RTLTextMeshPro>().text = "No Words to Revise";
            toTextObject.GetComponent<RTLTextMeshPro>().text = "";
            return;
        }

        Card card = cardInstance.card;

        // Set words
        fromTextObject.GetComponent<RTLTextMeshPro>().text = cardInstance.ab ? card.aWord : card.bWord;
        toTextObject.GetComponent<RTLTextMeshPro>().text = cardInstance.ab ? card.bWord : card.aWord;

        // Hide answer and correct and incorrect buttons
        toTextObject.SetActive(false);
        correctButtonObject.SetActive(false);
        incorrectButtonObject.SetActive(false);
    }

    private void SetCardCount() {
        cardCountTextObject.GetComponent<TextMeshProUGUI>().text = cardsToRevise.Count.ToString();
    }

    /* Navigates to another scene */
    public void NavigateToScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
