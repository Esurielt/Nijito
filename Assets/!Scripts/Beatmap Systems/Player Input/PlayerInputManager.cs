using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Beatmap.PlayerInput
{
    class PlayerInputManager:MonoBehaviour
    {
        ScoreManager scoreManager;

        public Canvas canvas;
        public Text ComboText;
        public Text ScoreText;

        public int currentIndex;

        private void Start()
        {
            
        }

        private void Update()
        {
            // update current index

            // get input
            Dictionary<int, int> playerinput = GetInput();
            if (playerinput == null)
            {
                return;
            }
            // get compare result
            int[] result = scoreManager.Compare(currentIndex, playerinput);

            // TODO: update UI

        }

        private Dictionary<int, int> GetInput()
        {
            if (Input.touchCount <= 0)
            {
                return null;
            }
            Dictionary<int, int> playerinput = new Dictionary<int, int>();
            Dictionary<TouchPhase, int> TouchPrasetoInt = new Dictionary<TouchPhase, int>
                {
                    { TouchPhase.Began, 0 } ,
                    { TouchPhase.Canceled, 1 },
                    { TouchPhase.Ended, 2 },
                    { TouchPhase.Moved, 3 },
                    { TouchPhase.Stationary, 4 }
                };
            for (int i = 0; i < Input.touchCount; i++)
            {
                // get type
                int inputType = -1;
                inputType = TouchPrasetoInt[Input.GetTouch(i).phase];

                // get index by touch position
                int channelIndex = -1;
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = Input.GetTouch(i).position;
                GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();
                List<RaycastResult> results = new List<RaycastResult>();
                gr.Raycast(pointerEventData, results);
                foreach (RaycastResult r in results)
                {
                    if (r.gameObject.GetComponent<Button>())
                        channelIndex = Int32.Parse(r.gameObject.GetComponent<Button>().tag);
                }

                playerinput.Add(channelIndex, inputType);
            }
            // return
            return playerinput;
        }
    }
}
