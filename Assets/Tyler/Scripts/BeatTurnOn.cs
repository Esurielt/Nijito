using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatTurnOn : MonoBehaviour
{
    // Start is called before the first frame update
    public SpriteRenderer spriteRenderer;
    public float TimeStart;
    bool MoveBeat = false;
    public float Speed = 2f;
    private Vector3 Endpoint;
    public Sprite Note;
    private float timer = 0f;

    void Start()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        Endpoint = transform.localPosition - new Vector3(0, 100);
        


    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (MoveBeat)
        {
            Debug.Log(Endpoint);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Endpoint, Time.deltaTime * Speed);
        }
        
        if (timer >= TimeStart)
        {
            MoveBeat = true;
            this.spriteRenderer.enabled = true;
        }
        


        
    }

 


}
