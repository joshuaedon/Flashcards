using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using UnityEngine;

public class Card
{
    public static List<Card> cards;

    public bool ab;
    public string aWord;
    public string bWord;
    public DateTime abRevisionDate;
    public int abRevisionStep;
    public DateTime baRevisionDate;
    public int baRevisionStep;

    public Card(string aWord, string bWord, DateTime abRevisionDate, int abRevisionStep, DateTime baRevisionDate, int baRevisionStep) {
        this.aWord = aWord;
        this.bWord = bWord;
        this.abRevisionDate = abRevisionDate;
        this.abRevisionStep = abRevisionStep;
        this.baRevisionDate = baRevisionDate;
        this.baRevisionStep = baRevisionStep;
    }

    public Card(string aWord, string bWord) {
        this.aWord = aWord;
        this.bWord = bWord;
        SetDefaultStats();
    }

    public void SetDefaultStats() {
        this.abRevisionDate = System.DateTime.Now;
        this.abRevisionStep = Settings.startingRevisionStep;
        this.baRevisionDate = System.DateTime.Now;
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
        this.abRevisionDate = DateTime.Parse(columnValues[2]);
        this.abRevisionStep = Int32.Parse(columnValues[3]);
        this.baRevisionDate = DateTime.Parse(columnValues[4]);
        this.baRevisionStep = Int32.Parse(columnValues[5]);
    }

    /* Creates a list of cards from the spreadsheet */
    public static void RetrieveCards() {
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
    }

    public static void WriteCardsToCSV() {
        string csvFilePath = Path.Combine(Application.persistentDataPath, "Arabic.csv");

        using(StreamWriter writer = new StreamWriter(csvFilePath)) {
            string csvContent = "English,Arabic,AB Revision Date,Ab Revision Step,BA Revision Date,BA Revision Step";

            foreach(Card card in cards) {
                csvContent += $"\n{card.aWord},{card.bWord},{card.abRevisionDate},{card.abRevisionStep},{card.baRevisionDate},{card.baRevisionStep}";
            }

            writer.WriteLine(csvContent);
        }

        // MonoBehaviour.print($"{cards.Count} cards saved");
    }
}
