using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using RTLTMPro;
using TMPro;
using System.Threading;
using System.Linq;

public class CardSceneController : MonoBehaviour
{

    public GameObject fromTextObject;
    public GameObject toTextObject;
    public GameObject cardCountTextObject;
    public GameObject correctButtonObject;
    public GameObject incorrectButtonObject;

    private List<Card> cardsToRevise;
    private List<Card> cardBacklog;
    private Card currentCard;
    private bool currentCardAB;

    /* Start is called before the first frame update */
    void Start() {
        SelectNextCard();
    }

    /* Update is called once per frame */
    void Update() { }

    /* Gets list of words that are ready to be revised */
    private void GetWordsToRevise() {
        int timestampStart = System.DateTime.Now.Millisecond;

        // Retrieve cards from spreadsheet if not already retrieved
        if(Card.cards == null)
            Card.RetrieveCards();

        // Sort cards by Revision Step max to min. This ensures the most known cards are fed to the user first and also ensures 
        // the number of words shown to the user is minimised (cards previously shown will always take priority over new cards). It also 
        // means that if a card's ab step is far greater than the ba step, it will be shown first in the list and often until the steps equalise.
        Card.cards = Card.cards.OrderByDescending(c => Math.Max(c.abRevisionStep, c.baRevisionStep)).ToList();

        DateTime now = System.DateTime.Now;

        cardsToRevise = new List<Card>();
        cardBacklog = new List<Card>();

        foreach(Card card in Card.cards) {
            // If the revision date is in the past, add the card to the revision list, otherwise, add it to the backlog
            if(card.revisionDate < now)
                cardsToRevise.Add(card); 
            else
                cardBacklog.Add(card); 
        }

        // Sort the card backlog by revision date oldest to newest, so that the card which will be soonest added to the revision list is always first
        cardBacklog = cardBacklog.OrderBy(c => c.revisionDate).ToList();

        print($"Total Cards: {Card.cards.Count}, Cards to revise: {cardsToRevise.Count}, Cards backlog: {cardBacklog.Count}");

        print($"TIME GetWordsToRevise: {System.DateTime.Now.Millisecond - timestampStart}");
    }

    public void MarkCorrect() {

        if(currentCard != null) {
            
            // Set the next revision date to the current date + the step. This takes the minimum step out of the AB and BA variations
            // so that the user is forced to revise the word as often as their worst variation
            currentCard.revisionDate = DateTime.Now.AddMinutes(Math.Min(currentCard.abRevisionStep,currentCard.baRevisionStep));

            // Double the step so that the word will appear half as regularly
            if(currentCardAB)
                currentCard.abRevisionStep *= 2;
            else
                currentCard.baRevisionStep *= 2;

            // Set these updated values in the csv
            Card.WriteCardsToCSV();
        }

        // Select the next card
        SelectNextCard();
    }

    public void MarkIncorrect() {

        if(currentCard != null) {
            // Half the revision step
            if(currentCardAB)
                currentCard.abRevisionStep = Math.Max(1, currentCard.abRevisionStep / 2);
            else
                currentCard.baRevisionStep = Math.Max(1, currentCard.baRevisionStep / 2);

            // Set the revision date to now + a delay
            currentCard.revisionDate = DateTime.Now.AddMinutes(Settings.incorrectCardDelay);

            // Set these updated values in the csv
            Card.WriteCardsToCSV();

            // Readd the card to the list
            cardsToRevise.Add(currentCard);
        }

        // Select the next card
        SelectNextCard();
    }

    public void ShowAnswer() {
        toTextObject.SetActive(true);
        correctButtonObject.SetActive(true);
        incorrectButtonObject.SetActive(true);
    }

    private void SelectNextCard() {
        // Find card to revise if there are no cards in the list
        if((cardsToRevise?.Count ?? 0) == 0) {
            GetWordsToRevise();
            if(cardsToRevise.Count == 0) {
                SetCard(null);
                return;
            }
        }

        // Get the first card in the list and set it to the current. This card should be the card with the longest Revision Step
        SetCard(cardsToRevise[0]);

        // Remove the card from the list (this will be readded if answered incorrectly)
        cardsToRevise.RemoveAt(0);

        // Set the UI text showing the number of remaining cards
        SetCardCount();
    }

    /* Configures the UI to display the current card*/
    private void SetCard(Card card) {
        currentCard = card;

        // Configure UI if no Card is provided (none to be revised)
        if(card == null) {
            fromTextObject.GetComponent<RTLTextMeshPro>().text = "No Words to Revise";
            toTextObject.GetComponent<RTLTextMeshPro>().text = "";
            return;
        }

        // Determine whether ab or ba should be revised
        currentCardAB = currentCard.abRevisionStep <= currentCard.baRevisionStep;

        // Set words
        fromTextObject.GetComponent<RTLTextMeshPro>().text = currentCardAB ? card.aWord : card.bWord;
        toTextObject.GetComponent<RTLTextMeshPro>().text = currentCardAB ? card.bWord : card.aWord;

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
