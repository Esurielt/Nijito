using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public class DialogueParser2 : MonoBehaviour
{
    // Start is called before the first frame update
    //this script reads dialogue text, loads text, then looks for spites/images and loads them. 
    List<DialogueLine> lines;
    List<Sprite> images;
    Dictionary<string, Sprite[]> SDict = new Dictionary<string, Sprite[]>();
    


    struct DialogueLine
    {
        public string name;
        public string content;
        public int pose;
        public string position;

        public DialogueLine(string n, string c, int p, string pos)
        {
            name = n;
            content = c;
            pose = p;
            position = pos;
        }
    }

    void Start()
    {
       
        lines = new List<DialogueLine>();
        string file = "Dialogue";
        string sceneNum = EditorApplication.currentScene;//"Scene1"
        sceneNum = Regex.Replace(sceneNum, "[^0-9]", "");//"1"

        file += "3";//sceneNum;//file=file+scenNum("Dialogue3")sceneNum
        file += ".txt";

        
        //just set file to anything u want the text file name. 
        LoadDialogue(file);
       // Debug.Log(file);

        images = new List<Sprite>();
        LoadImages();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadImages()//loads all sprites in resource folder and ex[ibuki, miku]
    {
        for (int i = 0; i < lines.Count; i++)
        {
            string imageName = lines[i].name;//goes through all lines of dialogue, loads only

            //char used names.
            if (!SDict.ContainsKey(imageName))
            {
                SDict[imageName] = Resources.LoadAll<Sprite>("Tyler/Sprites/" + imageName);
            }
          
        }
    }

    public string GetName(int lineNumber)
    {
        if (lineNumber < lines.Count)
            return lines[lineNumber].name;
        return "";
    }

    public string GetContent(int lineNumber)
    {
        if (lineNumber < lines.Count)
            return lines[lineNumber].content;
        return "";
    }
    public Sprite GetPose(int lineNumber)
    {
        if (lineNumber < lines.Count)
            //return images[lines[lineNumber].pose];

           

            return (SDict[lines[lineNumber].name][lines[lineNumber].pose]);
        return null;
    }
    public string GetPosition(int lineNumber)
    {
        if (lineNumber < lines.Count)
            return lines[lineNumber].position;
        return "";
    }

    void LoadDialogue(string filename)//takes in file("Dialogue1.txt") and reads from it while parsing
    {//then it puts the values into DialogueLine Structure and makes a list of them in list lines

        ///here put the path to the place for the dialogue. 
        string file = "Assets/Tyler/Visual_Novel/Resources/Dialogue/" + filename;
        string line;
        StreamReader r = new StreamReader(file);
        using (r)
        {
            do
            {
                line = r.ReadLine();

                if (line != null)
                {
                    string[] line_values = SplitCsvLine(line);

                    if (line_values.Count() < 4)//DEFAULT POSE AND POSITION SET TO "0" AND "R"
                    {
                        string[] line_values1 = { line_values[0], line_values[1], "0", "R" };
                        line_values = line_values1;
                    }
                    DialogueLine line_entry = new DialogueLine(line_values[0], line_values[1], int.Parse(line_values[2]), line_values[3]);
                    lines.Add(line_entry);
                }
            }
            while (line != null);
            r.Close();
        }
    }

    string[] SplitCsvLine(string line)
    {
        string pattern = @"
        # Match one value in valid CSV string.
         (?!\s *$)                                      # Don't match empty last value.
         \s *                                           # Strip whitespace before value.
         (?:                                           # Group for value alternatives.
           '(?<val>[^'\\]*(?:\\[\S\s]
        [^'\\]*)*)'       # Either $1: Single quoted string,
         | ""(?<val>[^""\\]* (?:\\[\S\s] [^""\\]*)*)""   # or $2: Double quoted string,
         | (?<val>[^,'""\s\\]*(?:\s+[^,'""\s\\]+)*)    # or $3: Non-comma, non-quote stuff.
         )                                             # End group of value alternatives.
         \s*                                           # Strip whitespace after value.
         (?:,|$)                                       # Field ends on comma or EOS.
         ";
        string[] values = (from Match m in Regex.Matches(line, pattern,
                RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                RegexOptions.Multiline)
                           select m.Groups[1].Value).ToArray();
        return values;

    }






}


