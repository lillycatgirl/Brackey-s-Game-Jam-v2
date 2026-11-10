using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Dialogue
{
    public static class DialogueLoader
    {
        public enum Character
        {
            Player,
            OldMan,
            Girl,
            Tringle
        }

        public static Dictionary<string, Character> CharacterNames =
            new Dictionary<string, Character>()
            {
                { "Player", Character.Player },
                { "Oldman", Character.OldMan },
                { "Girl", Character.Girl },
                { "Tringle", Character.Tringle }
            };
        
        public static Dictionary<Character, string> CharacterDisplayNames =
            new()
            {
                { Character.Player, "You" },
                { Character.OldMan, "Old Man" },
                { Character.Girl, "Girl" },
                { Character.Tringle, "Monster" }
            };



        public static Dictionary<char, List<string>> FormattingCodes =
            new()
            {
                { '*', new List<string> { "<b>", "</b>" } },
                { '/', new List<string> { "<i>", "</i>" } }
            };


        public struct DialogueLine
        {
            public string Text;
            public Character Character;
            public string[] PlayerOptions;
            public bool PlayerQuestion;
            public bool HasCondition;
            public List<String> Condition;
            public bool Shady;
        }
       
        
        
        public static DialogueLine[] DialogueFromFile(TextAsset file)
        {
            var lines = file.text.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None);
            
            var formattedLines = new DialogueLine[lines.Length];
            
            
            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].Contains(':')) continue;
                
                var lineCharacter = lines[i].Substring(0, lines[i].IndexOf(':')).Trim();

                if (lineCharacter[0] == '~')
                {
                    formattedLines[i].Shady = true;
                    lineCharacter = lineCharacter.Substring(1);
                }
                else
                {
                    formattedLines[i].Shady = false;
                }
                
                // was a question
                if (lineCharacter[0] == '?')
                {
                    lineCharacter = lineCharacter.Substring(1);
                    formattedLines[i].PlayerQuestion = true;
                }
                else
                {
                    formattedLines[i].PlayerQuestion = false;
                }
                
                formattedLines[i].Condition = new List<string>();
                while (lineCharacter[0] == '(')
                {
                    var closing =  lineCharacter.IndexOf(')');
                    formattedLines[i].HasCondition = true;
                    formattedLines[i].Condition.Add(lineCharacter.Substring(1, closing - 1)); 
                    lineCharacter = lineCharacter.Substring(closing + 1);
                }
                
                lineCharacter = lineCharacter.Trim();
                formattedLines[i].Character = CharacterNames[lineCharacter];
                
                
                string dialogue = lines[i].Substring(lines[i].IndexOf(':') + 1).TrimStart();
                string formattedDialogue = "";
                bool bolded = false;
                bool italic = false;
                foreach (char c in dialogue)
                {
                    if (FormattingCodes.TryGetValue(c, out var code))
                    {
                        if (c == '*')
                        {
                            formattedDialogue += bolded ? code[1] : code[0];
                            bolded = !bolded;
                        }
                        else if (c == '/')
                        {
                            formattedDialogue += italic ? code[1] : code[0];
                            italic = !italic;
                        }
                    }
                    else
                    {
                        formattedDialogue += c;
                    }
                }

                formattedLines[i].Text = formattedDialogue;
                
                if (formattedLines[i].PlayerQuestion)
                {
                    formattedLines[i].Text = "";
                    int optionCount = 0;
                    List<int[]> options = new List<int[]>();
                    for (var j = 0; j < formattedDialogue.Length; j++)
                    {
                        if (formattedDialogue[j] == '(')
                        {
                            optionCount++;
                            options.Add(new int[] { j, -1 });
                        } else if (formattedDialogue[j] == ')')
                        {
                            options[optionCount - 1][1] = j;
                        }
                    }
                    
                    List<string> formattedOptions = new List<string>();
                    foreach (int[] option in options)
                    {
                        formattedOptions.Add(formattedDialogue.Substring(option[0] + 1, option[1] - option[0] - 1)
                        );
                    }
                    formattedLines[i].PlayerOptions = formattedOptions.ToArray();
                }
            }
            
            return formattedLines;
        }
    }
}