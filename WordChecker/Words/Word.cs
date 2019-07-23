using System;
using System.Collections.Generic;
using System.Text;

namespace Wordplay.Words
{
    class Word
    {
        private string text;
        public string Text {
            get {
                return text;
            }
        }

        private bool isWord;
        public bool IsWord {
            get {
                return isWord;
            }
            set {
                isWord = value;
            }
        }

        public Word(string text, bool isWord = false)
        {
            this.text = text;
            this.isWord = isWord;
        }
    }
}
