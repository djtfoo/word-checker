using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wordplay.Words
{
    class WordCheckerSystem
    {
        private WordsAPI wordsApi;

        private Dictionary<string, Word> insertedWordsList; // each word is unique
        private int score = 0;
        public int Score {
            get {
                return score;
            }
        }

        public WordCheckerSystem()
        {
            wordsApi = new WordsAPI();

            insertedWordsList = new Dictionary<string, Word>();
            score = 0;
        }

        public async Task<bool> AddWord(string text)
        {
            // check if word already exists in List to prevent duplicates
            if (insertedWordsList.ContainsKey(text))
                return false;

            Word newWord = new Word(text);
            insertedWordsList.Add(text, newWord);

            bool doesWordExist = await wordsApi.DoesWordExist(text);
            newWord.IsWord = doesWordExist;
            UpdateScore(doesWordExist);

            return true;
        }

        private void UpdateScore(bool isWord)
        {
            if (isWord)
                score++;    // increase score by 1
            else
                score--;    // decrease score by 1
        }

        public void RemoveWord(string word) {

            if (insertedWordsList.ContainsKey(word))    // if Word exists
            {
                // get word
                Word toRemove = insertedWordsList[word];

                // remove Word from Dictionary
                insertedWordsList.Remove(word);

                // update score
                UpdateScore(!toRemove.IsWord);  // reversing the update to the score
            }
        }

        public bool GetIsWord(string word)
        {
            if (insertedWordsList.ContainsKey(word))    // Word exists in Dictionary
                return insertedWordsList[word].IsWord;

            return false;   // default
        }

        public void Reset()
        {
            insertedWordsList.Clear();
            score = 0;
        }
    }
}
