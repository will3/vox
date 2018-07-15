using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace FarmVox
{
    public class TopBar : MonoBehaviour
    {
        // Use this for initialization
        private bool showBuildMenu = false;
        private GameController gameController;

        void Start()
        {
            gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
            Assert.IsNotNull(gameController);
        }

        private float buttonWidth = 100;
        private float buttonHeight = 30;

        private void OnGUI()
        {
            var x = 0;
            var y = 0;
            if (GUI.Button(new Rect(x, y, buttonWidth, buttonHeight), "Build"))
            {
                showBuildMenu = !showBuildMenu;
            }

            if (showBuildMenu)
            {
                drawBuildMenu(new Vector2(0, 30));
            }
        }

        private void drawBuildMenu(Vector2 from)
        {
            var panelHeight = 400;
            GUI.BeginGroup(new Rect(from.x, from.y, buttonWidth, panelHeight));
            if (GUI.Button(new Rect(0, 0, 100, 30), "Farm"))
            {
                showBuildMenu = false;
                buildFarm();
            }
            GUI.EndGroup();
        }

        private void buildFarm()
        {
            gameController.Highlight.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}