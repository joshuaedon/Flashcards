using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using UnityEngine;

public class Card
{
    public static List<Card> cards;

    public string aWord;
    public string bWord;
    public DateTime revisionDate;
    public int abRevisionStep;
    public int baRevisionStep;

    public Card(string aWord, string bWord, DateTime revisionDate, int abRevisionStep, int baRevisionStep) {
        this.aWord = aWord;
        this.bWord = bWord;
        this.revisionDate = revisionDate;
        this.abRevisionStep = abRevisionStep;
        this.baRevisionStep = baRevisionStep;
    }

    public Card(string aWord, string bWord) {
        this.aWord = aWord;
        this.bWord = bWord;
        SetDefaultStats();
    }

    public void SetDefaultStats() {
        this.revisionDate = System.DateTime.Now;
        this.abRevisionStep = Settings.startingRevisionStep;
        this.baRevisionStep = Settings.startingRevisionStep;
    }

    public Card(string csvRow) {
        // Split row into column values 
        string[] columnValues = csvRow.Split(new char[] {','});

        // Exit if empty row
        if(columnValues.Length <= 1)
            return;
        
        // Get the revision dates and steps for both directions of translations
        this.aWord = columnValues[0];
        
        if(this.aWord == "")
            return;

        this.bWord = columnValues[1];
        this.revisionDate = DateTime.Parse(columnValues[2]);
        this.abRevisionStep = Int32.Parse(columnValues[3]);
        this.baRevisionStep = Int32.Parse(columnValues[4]);
    }

    /* Creates a list of cards from the spreadsheet */
    public static void RetrieveCards() {
        
        int timestampStart = System.DateTime.Now.Millisecond;

        cards = new List<Card>();

        // Get the Arabic word dataset
        string csvFilePath = Path.Combine(Application.persistentDataPath, "Arabic.csv");
        MonoBehaviour.print(csvFilePath);
        string[] rows = File.ReadAllLines(csvFilePath);

        for (var rowI = 1; rowI < rows.Length; rowI++) {
            // Split row into column values
            string[] columnValues = rows[rowI].Split(new char[] {','});

            // Continue if empty row
            if(columnValues.Length <= 1)
                continue;

            // Create card and add to the list
            Card newCard = new Card(rows[rowI]);
            if(newCard.aWord == "")
                continue;
            cards.Add(newCard);
        }

        MonoBehaviour.print($"{cards.Count} cards retrieved");

        MonoBehaviour.print($"TIME RetrieveCards: {System.DateTime.Now.Millisecond - timestampStart}");
    }

    public static void WriteCardsToCSV() {
        string csvFilePath = Path.Combine(Application.persistentDataPath, "Arabic.csv");

        using(StreamWriter writer = new StreamWriter(csvFilePath)) {
            string csvContent = "English,Arabic,Revision Date,AB Revision Step,BA Revision Step";

            foreach(Card card in cards) {
                csvContent += $"\n{card.aWord},{card.bWord},{card.revisionDate},{card.abRevisionStep},{card.baRevisionStep}";
            }

            writer.WriteLine(csvContent);
        }

        // MonoBehaviour.print($"{cards.Count} cards saved");
    }
}
