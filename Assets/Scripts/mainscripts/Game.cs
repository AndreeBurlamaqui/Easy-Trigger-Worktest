using UnityEngine;
using System.Collections.Generic;

public class Game : MonoBehaviour {

    Main main;
    int myRes;
    Gfx gfx;
    Snd snd;

    static string PLAY = "play";

    string gameStatus;

    int   camWidth;
    int   camHeight;
    float camX;
    float camY;

    Player player;

    bool leftKey, rightKey, jumpKey, duckKey, shootKey;
    int playerHorizontal, playerVertical;
    bool playerShoot;
    bool playerShootRelease = true;

    List<GeneralObject> gameObjects;
    int gameObjectLength;



    public void Init(Main inMain) {

        main  = inMain;
        gfx   = main.gfx;
        myRes = gfx.myRes;
        snd   = main.snd;

        camWidth  = gfx.screenWidth / myRes;
        camHeight = gfx.screenHeight / myRes;

        gameObjects = new List<GeneralObject>();
        gameObjectLength = 0;

        gameStatus  = PLAY;

    }

    

    void Update() {
       
        if (gameStatus==PLAY) {

            GoKeys();

            GoCam();

            GoObjects();

        } 

    }





    private void GoKeys() {
       
        // ---------------------------------------------------------------
        // NORMAL KEYBOARD
		// ---------------------------------------------------------------

		if (Input.GetKeyDown(KeyCode.LeftArrow))  { leftKey   = true; }
        if (Input.GetKeyUp(KeyCode.LeftArrow))    { leftKey   = false; }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { rightKey  = true; }
        if (Input.GetKeyUp(KeyCode.RightArrow))   { rightKey  = false; }
        if (Input.GetKeyDown(KeyCode.UpArrow))    { jumpKey   = true; }
        if (Input.GetKeyUp(KeyCode.UpArrow))      { jumpKey   = false; }
        if (Input.GetKeyDown(KeyCode.DownArrow))  { duckKey   = true; }
        if (Input.GetKeyUp(KeyCode.DownArrow))    { duckKey   = false; }
        if (Input.GetKeyDown(KeyCode.M))          { jumpKey   = true; }
        if (Input.GetKeyUp(KeyCode.M))            { jumpKey   = false; }
        if (Input.GetKeyDown(KeyCode.N))          { shootKey  = true; }
        if (Input.GetKeyUp(KeyCode.N))            { shootKey  = false; }

        playerHorizontal = 0;
        if (leftKey) { playerHorizontal-=1; }
        if (rightKey) { playerHorizontal+=1; }

        playerVertical = 0;
        if (jumpKey) { playerVertical-=1; }
        if (duckKey) { playerVertical+=1; }

        playerShoot = false;

        if (shootKey) {
            if (playerShootRelease) {
                playerShootRelease = false;
                playerShoot = true;
            }
            
        } else {
            if (!playerShootRelease) {
                playerShootRelease = true;
            }
        }
    }


    
    void GoCam() {

        camX = 480 - camWidth/2;
        camY = 600 - camHeight/2;

        gfx.MoveLevel(camX, camY);
       
    }
    


    public void AddLevelObject(GeneralObject inObj) {

        gameObjects.Add(inObj);
        gameObjectLength++;

    }



    void GoObjects(bool inDoActive=true) {

        for (int i = 0; i<gameObjectLength; i++) {

            if (!gameObjects[i].FrameEvent()) {
                gameObjects.RemoveAt(i);
                i--;
                gameObjectLength--;
            }
        }

    }



}