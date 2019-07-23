using System;
using Xamarin.Forms;
using System.Collections.Generic;
using Wordplay.Words;

namespace WordChecker
{
    public partial class MainPage : ContentPage
    {
        private WordCheckerSystem wordCheckerSystem;
        private Dictionary<string, Grid> wordToGridMap;

        public MainPage()
        {
            InitializeComponent();

            // initialize Word Checker system
            wordCheckerSystem = new WordCheckerSystem();

            // initialize mapping of a word to the Grid
            wordToGridMap = new Dictionary<string, Grid>();

            // disable response texts
            WaitingRequestText.IsVisible = false;
            DuplicateWordText.IsVisible = false;
        }

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            // if string is empty OR just 1 word length (letter), ignore
            if (string.IsNullOrWhiteSpace(WordEntry.Text) || WordEntry.Text.Length <= 1)
            {
                WordEntry.Text = string.Empty;
                return;
            }

            // display waiting text
            WaitingRequestText.IsVisible = true;

            // get entry text
            string entryText = WordEntry.Text.ToLower();

            // clear editor field
            WordEntry.Text = string.Empty;

            // add to list of Words in Word Checker system
            bool addedSuccessfully = await wordCheckerSystem.AddWord(entryText);

            // update UI
            if (addedSuccessfully)
            {    // word was added

                // update score text
                UpdateScoreText();

                // create word label
                var grid = AddWordToUI(entryText);
                wordToGridMap.Add(entryText, grid);

                // add word label to StackLayout of Labels
                insertedWords.Children.Add(grid);
            }
            else
            {  // is a duplicate word

                // display duplicate warning text
                DuplicateWordText.IsVisible = true;
                // disable duplicate warning text after 3 seconds
                Device.StartTimer(new TimeSpan(0, 0, 3), () => DuplicateWordText.IsVisible = false);
            }

            // disable waiting text
            WaitingRequestText.IsVisible = false;
        }

        private Grid AddWordToUI(string word)
        {
            // create and define Grid
            Grid grid = new Grid { HorizontalOptions = LayoutOptions.Center };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });    // tick or cross Image whether word is correct or not
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });  // word Label
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(12) });    // delete ImageButton

            // find out if is a word or not
            bool isWord = wordCheckerSystem.GetIsWord(word);

            // create elements in Grid
            var image = new Image { Source = isWord ? "green.png" : "red.png" };
            var label = new Label { Text = word, FontSize = 18, HorizontalOptions = LayoutOptions.Center };
            var imageButton = new ImageButton
            {
                Source = "cross.png",
                ClassId = word,     // use ClassId to access the word
                IsEnabled = true
            };
            imageButton.Clicked += OnDeleteButtonClicked;

            grid.Children.Add(image, 0, 0);     // left
            grid.Children.Add(label, 1, 0);     // middle
            grid.Children.Add(imageButton, 2, 0);   // right

            return grid;
        }

        private void UpdateScoreText()
        {
            ScoreText.Text = wordCheckerSystem.Score.ToString() + " Points";
        }

        private void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            // get ImageButton and word
            ImageButton imgButton = (ImageButton)sender;
            string word = imgButton.ClassId;

            // delete this Grid
            Grid gridToDelete = wordToGridMap[word];
            insertedWords.Children.Remove(gridToDelete);

            // remove from Dictionaries
            wordToGridMap.Remove(word);
            wordCheckerSystem.RemoveWord(word);

            // update score text
            UpdateScoreText();
        }

        void OnClearButtonClicked(object sender, EventArgs e)
        {
            WordEntry.Text = string.Empty;
        }

        private bool afterOnTextChanged = false;
        void EntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (afterOnTextChanged)
            {
                afterOnTextChanged = false;
                return;
            }

            string newText = e.NewTextValue;
            if (!string.IsNullOrWhiteSpace(newText))
            {
                char lastChar = newText.Substring(newText.Length - 1, 1)[0];
                if (!char.IsLetter(lastChar))
                {
                    WordEntry.Text = e.OldTextValue;
                    afterOnTextChanged = true;
                }
            }
        }

        private bool resetButtonClicked = false;
        public void OnResetButtonClicked(object sender, EventArgs e)
        {
            if (resetButtonClicked)
            {
                Reset();
                resetButtonClicked = false;
            }
            else
            {
                resetButtonClicked = true;
                Device.StartTimer(new TimeSpan(0, 0, 1), () => resetButtonClicked = false);
            }
        }

        private void Reset()
        {
            // reset Word Checker system
            wordCheckerSystem.Reset();

            // clear list of Words
            wordToGridMap.Clear();
            insertedWords.Children.Clear();

            // reset score text
            UpdateScoreText();

            // disable response text
            DuplicateWordText.IsVisible = false;
        }
    }   // end of MainPage class
}   // end of namespace
