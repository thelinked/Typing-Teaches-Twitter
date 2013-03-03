﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHunspell;
using NUnit.Framework;
using towerDefence;

namespace AnalysisLibsUnitTests
{
    [TestFixture]
    public class SpellCheckerTests
    {
        private SpellChecker spellChecker;
        private string tweet;
        [SetUp]
        public void setup()
        {
            var hunspell = new Hunspell("en_GB-oed.aff", "en_GB-oed.dic");
            spellChecker = new SpellChecker(hunspell);
        }

        [Test]
        public void apostrophe()
        {
            tweet = "RT @BieberIndonesia: I actually don't know how to make him happy at the moment. #OperationMakeBieberSmile";
            var sentence = spellChecker.CheckSentence(tweet);
            Assert.That(sentence.HasMisspelling,Is.False);
        }

        [Test]
        public void comma()
        {
            tweet =
                "#OperationMakeBieberSmile accomplished Im so happy that Justin smiled because when Justin smiles I smile,when he's happy Im happy!!LOVE U JB";
            var sentence = spellChecker.CheckSentence(tweet);
            Assert.That(sentence.Words.ElementAt(12).Word,Is.EqualTo("smile"));
        }

        [Test]
        public void apostrophe2()
        {
            tweet =
                "RT @iSmileForBieb: This is an amazing feeling . #OperationMakeBieberSmile - mission complete. I'm so proud of this family. #beliebers ht ...";
            var sentence = spellChecker.CheckSentence(tweet);
            Assert.That(sentence.Words.ElementAt(7).Word, Is.EqualTo("I'm"));
        
        }

        [Test]
        public void trailing_exclamation()
        {
            tweet = "RT @bob I hate this!";
            var sentence = spellChecker.CheckSentence(tweet);
            Assert.That(sentence.HasMisspelling, Is.False);


            tweet = "RT @bob I hate thsi!";
            sentence = spellChecker.CheckSentence(tweet);
            Assert.That(sentence.HasMisspelling, Is.True);
        }

        [Test]
        public void trailing_questionmark()
        {
            tweet =
                "You see this? Jay brings new details about bad blood again!!!  #OperationMakeBieberSmile http://t.co/hYaA6sOP1G";
            var sentence = spellChecker.CheckSentence(tweet);
            Assert.That(sentence.HasMisspelling, Is.False);
        }
       
    }
}
