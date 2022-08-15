using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using KModkit;
using Random = UnityEngine.Random;

public class trafficBoardScript : MonoBehaviour {

    public KMAudio Audio;
    public KMBombInfo BombInfo;
    public KMBombModule BombModule;

    //logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    //buttons
    public KMSelectable[] buttons = new KMSelectable[3];  //r2, c2, d2
    
    public SpriteRenderer[] signs = new SpriteRenderer[5];  //r1, c1, d1, r2, c2

    public Sprite[] signs_direction;
    public Sprite[] signs_caution;

    private int reference;   //Indicates the sign_caution and its direction.
    private int row, column, pos; //Current position(used in calculation)
    private int selrow, selcolumn;  //Selected position(used in answering)
    private int initrow, initcolumn; //Starting grid
    private int ansrow, anscolumn;   //Correct answer
    private int active = 0;

    public int[] roadMon = new int[64]
    {
        5, 4, 2, 1, 4, 4, 6, 0,
        0, 2, 7, 5, 2, 2, 2, 4,
        4, 3, 3, 7, 2, 7, 0, 4,
        3, 2, 2, 1, 7, 0, 7, 6,
        5, 1, 1, 3, 3, 1, 6, 1,
        3, 4, 4, 6, 1, 7, 6, 7,
        3, 7, 5, 1, 0, 0, 0, 3,
        0, 5, 5, 5, 6, 6, 6, 5
    };

    public int[] roadTue = new int[64]
    {
        1, 4, 0, 2, 7, 0, 4, 7, 
        4, 5, 0, 5, 0, 7, 6, 5,
        1, 6, 5, 6, 3, 7, 3, 7,
        6, 7, 7, 2, 4, 7, 5, 6,
        2, 3, 1, 6, 5, 4, 5, 2,
        1, 3, 3, 6, 0, 3, 0, 2,
        0, 4, 0, 3, 5, 6, 2, 4,
        1, 1, 2, 4, 2, 3, 1, 1
    };

    public int[] roadWed = new int[64]
    {
        4, 3, 5, 2, 4, 1, 0, 5, 
        6, 1, 3, 1, 5, 3, 6, 2, 
        4, 0, 0, 5, 2, 5, 4, 7, 
        1, 5, 6, 7, 7, 2, 2, 0, 
        3, 7, 5, 4, 7, 2, 3, 4, 
        0, 4, 4, 6, 5, 6, 0, 6, 
        7, 0, 7, 1, 0, 2, 2, 7, 
        6, 3, 1, 6, 3, 3, 1, 1
    };

    public int[] roadThu = new int[64]
    {
        6, 3, 2, 1, 0, 7, 4, 4, 
        2, 7, 6, 1, 5, 3, 7, 5, 
        4, 3, 4, 4, 3, 7, 4, 5, 
        7, 0, 3, 4, 6, 2, 6, 4, 
        5, 7, 6, 0, 7, 5, 7, 6, 
        6, 5, 2, 2, 0, 2, 3, 2, 
        5, 5, 1, 0, 2, 1, 6, 0, 
        3, 3, 0, 0, 1, 1, 1, 1
    };

    public int[] roadFri = new int[64]
    {
        4, 3, 6, 3, 7, 0, 1, 3, 
        5, 4, 1, 0, 5, 3, 1, 6, 
        2, 6, 6, 2, 6, 1, 0, 1, 
        7, 2, 2, 7, 3, 4, 4, 4, 
        3, 4, 1, 1, 2, 0, 2, 0, 
        6, 1, 7, 2, 4, 6, 5, 6, 
        4, 7, 2, 7, 3, 0, 7, 7, 
        3, 0, 0, 5, 5, 5, 5, 5
    };

    public int[] roadSat = new int[64]
    {
        1, 2, 4, 7, 1, 4, 6, 2, 
        3, 3, 2, 7, 0, 6, 1, 3, 
        6, 3, 2, 0, 1, 1, 4, 1, 
        1, 6, 5, 5, 5, 3, 0, 3, 
        1, 6, 4, 2, 3, 3, 6, 4, 
        6, 7, 0, 7, 5, 0, 7, 0, 
        7, 2, 0, 0, 5, 5, 7, 5, 
        4, 2, 6, 5, 2, 7, 4, 4
    };

    public int[] roadSun = new int[64]
    {
        3, 3, 3, 2, 4, 5, 6, 1, 
        0, 3, 2, 6, 5, 1, 4, 1, 
        0, 2, 0, 0, 6, 3, 1, 3, 
        6, 0, 2, 6, 1, 3, 6, 2, 
        1, 3, 6, 6, 7, 7, 5, 5, 
        0, 1, 0, 0, 1, 4, 4, 7, 
        2, 5, 7, 2, 2, 7, 7, 7, 
        7, 4, 4, 5, 4, 4, 5, 5
    };

    private int[] usedRoad = new int[64];
    private int[] roadVisited = new int[64]
    {
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0
    };

    //These are only used in log:
    public string[] directionWord = { "North", "Northeast", "East", "Southeast", "South", "Southwest", "west", "NorthWest" };
    //public string[] boardWord = { "roadMon", "roadTue", "roadWed", "roadThu", "roadFri", "roadSat", "roadSun" };

    // Use this for initialization
    void Start()
    {
        moduleId = moduleIdCounter++;
        GetComponent<KMBombModule>().OnActivate += OnActivate;
        foreach (KMSelectable button in buttons)
        {
            KMSelectable pressedButton = button;
            button.OnInteract += delegate () { PressButton(pressedButton); return false; };
        }
    }

    private void OnActivate()
    {
        active = 1;

        initrow = Random.Range(0, 8);
        initcolumn = Random.Range(0, 8);
        reference = Random.Range(0, 8);
        row = column = 0;
        signs[0].sprite = signs_direction[initrow];
        signs[1].sprite = signs_direction[initcolumn];
        signs[2].sprite = signs_caution[reference];
        selrow = selcolumn = 0;
        Debug.LogFormat("[Traffic Board #{0}] Starting grid: row {1}, column {2}.", moduleId, initrow + 1, initcolumn + 1);
        Debug.LogFormat("[Traffic Board #{0}] Stopping direction: {1}.", moduleId, directionWord[reference]);

        //Determine the correct road board to use:
        if(Convert.ToString(DateTime.Now.DayOfWeek) == "Monday") usedRoad = roadMon;
        else if (Convert.ToString(DateTime.Now.DayOfWeek) == "Tuesday") usedRoad = roadTue;
        else if (Convert.ToString(DateTime.Now.DayOfWeek) == "Wednesday") usedRoad = roadWed;
        else if (Convert.ToString(DateTime.Now.DayOfWeek) == "Thursday") usedRoad = roadThu;
        else if (Convert.ToString(DateTime.Now.DayOfWeek) == "Friday") usedRoad = roadFri;
        else if (Convert.ToString(DateTime.Now.DayOfWeek) == "Saturday") usedRoad = roadSat;
        else if (Convert.ToString(DateTime.Now.DayOfWeek) == "Sunday") usedRoad = roadSun;
        Debug.LogFormat("[Traffic Board #{0}] Today is {1}.", moduleId, Convert.ToString(DateTime.Now.DayOfWeek));

        //Calculate the correct answer:
        int mustStop = 0;
        row = initrow; column = initcolumn; UpdatePos();
        roadVisited[pos] = 1;
        MoveOneStep(usedRoad[pos]);
        Debug.LogFormat("[Traffic Board #{0}] Moving to row {1}, column {2}...", moduleId, row + 1, column + 1);
        while(mustStop == 0)
        {
            if(usedRoad[pos] == reference)
            {
                mustStop = 1;
                Debug.LogFormat("[Traffic Board #{0}] You have stopped at row {1}, column {2} because of condition 1.", moduleId, row + 1, column + 1);
            }
            else if(roadVisited[pos] == 1)
            {
                mustStop = 1;
                Debug.LogFormat("[Traffic Board #{0}] You have stopped at row {1}, column {2} because of condition 2 or 3.", moduleId, row + 1, column + 1);
            }
            else
            {
                roadVisited[pos] = 1;
                MoveOneStep(usedRoad[pos]);
                Debug.LogFormat("[Traffic Board #{0}] Moving to row {1}, column {2}...", moduleId, row + 1, column + 1);
            }
        }
        ansrow = row; anscolumn = column;
    }
	
	// Update is called once per frame
	public void Update ()
    {
        signs[3].sprite = signs_direction[selrow];
        signs[4].sprite = signs_direction[selcolumn];
    }

    private void UpdatePos()
    {
        pos = 8 * row + column;
    }

    private void MoveOneStep(int dir)
    {
        switch (dir)
        {
            case 0: MoveN(); break;
            case 1: MoveNE(); break;
            case 2: MoveE(); break;
            case 3: MoveSE(); break;
            case 4: MoveS(); break;
            case 5: MoveSW(); break;
            case 6: MoveW(); break;
            case 7: MoveNW(); break;
        }
    }

    private void MoveN()
    {
        row = (row == 0) ? 7 : row - 1;
        UpdatePos();
    }

    private void MoveS()
    {
        row = (row == 7) ? 0 : row + 1;
        UpdatePos();
    }

    private void MoveE()
    {
        column = (column == 7) ? 0 : column + 1;
        UpdatePos();
    }

    private void MoveW()
    {
        column = (column == 0) ? 7 : column - 1;
        UpdatePos();
    }

    private void MoveNE()
    {
        MoveN(); MoveE();
    }

    private void MoveSE()
    {
        MoveS(); MoveE();
    }

    private void MoveSW()
    {
        MoveS(); MoveW();
    }

    private void MoveNW()
    {
        MoveN(); MoveW();
    }

    private void PressButton(KMSelectable bt)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, this.transform);
        GetComponent<KMSelectable>().AddInteractionPunch();

        if (active == 0) { }
        else
        {
            if(bt == buttons[0])
            {
                selrow = (selrow == 7) ? 0 : selrow + 1;
            }
            else if(bt == buttons[1])
            {
                selcolumn = (selcolumn == 7) ? 0 : selcolumn + 1;
            }
            else
            {
                if((selrow == ansrow) && (selcolumn == anscolumn))
                {
                    Debug.LogFormat("[Traffic Board #{0}] You have chosen row {1}, column {2}.", moduleId, selrow + 1, selcolumn + 1);
                    Debug.LogFormat("[Traffic Board #{0}] Module solved.", moduleId);
                    BombModule.HandlePass();
                    active = 0;
                }
                else
                {
                    Debug.LogFormat("[Traffic Board #{0}] You have chosen row {1}, column {2}.", moduleId, selrow + 1, selcolumn + 1);
                    Debug.LogFormat("[Traffic Board #{0}] Incorrect answer.", moduleId);
                    BombModule.HandleStrike();
                }
            }
        }
    }
}
