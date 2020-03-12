using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this one is to test only one textbox
public class DialogueBox1 : MonoBehaviour
{
    DialogueParser2 parser;
    public string dialogue;
    public string name;
    public Sprite pose;
    public string position;
    int lineNum;

   

    // GUIStyle customStyle = new GUIStyle();
    public GUIStyle customStyle, customStyleName;

    void Start()
    {
        dialogue = "";//start dialogue maybe set to public so designer can set initial screen text?
        parser = GameObject.Find("DialogueParserObj").GetComponent<DialogueParser2>();
        lineNum = 0;

        customStyle.richText = true;

       //public GUIStyle customStyle;
        

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))//change to touch button later.
        {
            ResetImage();

            name = parser.GetName(lineNum);
            dialogue = parser.GetContent(lineNum);
            pose = parser.GetPose(lineNum);
            position = parser.GetPosition(lineNum);

            DisplayImages();
            lineNum++;
        }   
    }

    void ResetImage()
    {
        if (name != "")
        {

            GameObject character = GameObject.Find(name);
            Destroy(character);
            //SpriteRenderer currSprite = character.GetComponent<SpriteRenderer>();
            //currSprite.sprite = null;
        }
        
    }
    
    void DisplayImages()//add to make sprite child of camera?
    {
        if (name != "")
        {

            GameObject character = new GameObject(name);
            SetSpritePositions(character);
            SpriteRenderer currSprite = character.AddComponent<SpriteRenderer>();
            currSprite.sprite = pose;
            currSprite.sortingLayerName = "Character";

           // GameObject character = GameObject.Find(name);
           // SetSpritePositions(character);
           // SpriteRenderer currSprite = character.GetComponent<SpriteRenderer>();
           // currSprite.sprite = pose;
            
        }
    }

    void SetSpritePositions(GameObject spriteObj)
    {
        Vector3 temp_position = Camera.main.transform.position;

        if (position == "L")
        {
            spriteObj.transform.position = temp_position + new Vector3(-8, 0, 12);
        }
        if (position == "R")
        {
            spriteObj.transform.position = temp_position + new Vector3(8, 0, 12);
        }
    }

    void OnGUI()//setting GUI/screen style?
    {
        /* this is for middle, if different resolution, fix this to equalize?
        Debug.Log("aaaaa");
        //dialogue = GUI.TextField(new Rect(position x, position y, stretch x, stretch y), dialogue, customStyle);
        dialogue = GUI.TextField(new Rect(500, 500, 1500, 1000), dialogue, customStyle);
        //uppercorners, then sides of it

        //name box
        //(x,y,width,height)
        name = GUI.TextField(new Rect(500, 500, 500, 200), name, customStyleName);
        */
        GUI.skin.textField.fontSize = Screen.width / 20;


        //1600,800,600,200
        if (name != "")
        {

            //dialogue = GUI.TextField(new Rect(-200, 1000, 1500, 1000), dialogue, customStyle);

            //big mon
            // dialogue = GUI.TextField(new Rect(-50, 550, 1500, 500), dialogue, customStyle);


            //THIS IS FOR THE TEXT BOX
            //Debug.Log(Screen.height);
            Rect rect1 = new Rect(0, (.6f)*Screen.height, Screen.width*1, Screen.height*(.5f));
            dialogue = GUI.TextField(rect1, dialogue, customStyle);
            customStyle.fontSize = (int)(.06f * Screen.height);
            customStyle.padding = new RectOffset(300, 0, -125, 0);

            if (dialogue.Length > 100)
            {
                customStyle.fontSize = (int)(.04f * Screen.height);
            }



            //THIS IS FOR THE NAME BOX
            //(x,y,width,height)
            Rect rect2 = new Rect(Screen.width*.13f, (.6f) * Screen.height, Screen.width * .1f, Screen.height * (.1f));
            customStyleName.fontSize = (int)(.04f * Screen.height);
            customStyleName.padding = new RectOffset(30,0,35,0);
            name = GUI.TextField(rect2, name, customStyleName);

            if (name.Length > 6)//this is only a temporary stopgate, how can we make it so that they adjust to size?
            {
                customStyleName.fontSize = (int)(.04f * Screen.height);
            }





        }
        

    }
}
