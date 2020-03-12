using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Beatmap.AudioTimetable;


public class BeatInitialize : MonoBehaviour//for now just not worrying about long notes. 
{
    // Start is called before the first frame update
    //5 sprites, one for each beat type
    //initalize each beat, set invis, and each has a script that when time= makes them appear and start moving. 
    public Sprite A_Note,B_Note,C_Note, D_Note, E_Note;
    public Camera Cam;//main camera
    private float timer = 0f;

    void Start()
    {
        List<Sprite> BeatSprites = new List<Sprite>();
        BeatSprites.Add(A_Note);
        BeatSprites.Add(B_Note);
        BeatSprites.Add(C_Note);
        BeatSprites.Add(D_Note);
        BeatSprites.Add(E_Note);

        

        //this is for practice
        int Num_of_Beats = 10;

        for (int i = 0; i < Num_of_Beats; ++i)
        {
            string Beat_Number = "Beat_" + i.ToString();
            GameObject Beat_Current = new GameObject(Beat_Number);
            Beat_Current.transform.parent = Cam.transform;
            SetBeatType(Beat_Current);
            //adding script to turn on
            BeatTurnOn turn_on_beat = Beat_Current.AddComponent<BeatTurnOn>();
            //here would be getting the start time information
            turn_on_beat.TimeStart = 5f * i;//to be changed

        }
    }

    // Update is called once per frame
    void Update()
    {
        //time = 240 frames a beat?
        timer += Time.deltaTime;
        Debug.Log(timer);



    }

    public void SetBeatType(GameObject curr_beat)
    {//here i need something to differentiate the beat types? for now just set to beat_a
     //creating invis sprite depending on beat note type and setting its position and image
       
        SpriteRenderer currSprite = curr_beat.AddComponent<SpriteRenderer>();
        currSprite.enabled = false;
        //currSprite.sortingLayerName = "Beats";
        //currSprite.gameObject.layer = 1;
        currSprite.sprite = A_Note;
        currSprite.transform.localPosition = new Vector3(0, 0, 80);
        //currSprite.transform.position = new Vector3(0, 0, -10);
        currSprite.transform.localScale = new Vector3(5, 5, 5);
        //currSprite.transform.localScale = new Vector3(currSprite.transform.localScale.x, currSprite.transform.localScale.y,currSprite.transform.localScale.z * -1);

    } 

   



}
