using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System;
using KModkit;
using System.Linq;
using KeepCoding;
using System.Text;
using Rnd = UnityEngine.Random;

public class factoryCubesScript : MonoBehaviour {

    public KMAudio audio;
    public KMBombModule bombModule;
    public KMBombInfo bomb;

    public GameObject TopLeft;
    public GameObject TopRight;
    public GameObject BottomLeft;
    public GameObject BottomRight;
    public GameObject CoverTop;
    public GameObject CoverLeft;
    public GameObject CoverRight;
    public KMSelectable[] Tools;
    public Material White;
    public Material[] Colors;
    public Renderer[] ToolRender;
    public Renderer[] CubeSides;
    public Renderer[] DisplayCubeSides;
    public GameObject Cube;

    private string[] UsableItems = new string[] { "", "", "", "", "", "", "", "", "", "" };
    private Material[] cool = new Material[] { null, null, null, null, null, null, null, null, null, null };
    private string[] CubeTools = new string[] { "top", "left", "right" };

    private bool isSolved, struck = false;
    private bool sandbox = false;
    private static bool playSound = false;
    private static int moduleIdCounter = 1;
    private int moduleId, batteries, ports, indicators, cubePresses, last;
    private long serialNumber;
    private string serial;
    private string order;
    private int ordered, stages = 0;
    private bool coverTop, coverLeft, coverRight = false;
    private bool displayTop, displayLeft, displayRight = false;


    // Use this for initialization

    private void Awake()
    {
        bombModule.OnActivate += FunnySound;
        for (int i = 0; i < 10; i++)
        {
            KMSelectable pressedKey = Tools[i];
            int j = i;
            Tools[i].OnInteract += delegate () { Button(j); return false; };
        }
    }
    void FunnySound()
    {
        if (playSound == false)
        {
            playSound = true;
            audio.PlaySoundAtTransform("theme", transform);
        }
    }
    void Start() {
        playSound = false;
        InitializeModule();
    }
    void InitializeModule()
    {
        moduleId = moduleIdCounter++;
        //batteries = bomb.GetBatteryCount();
        //ports = bomb.GetPortCount();
        //indicators = bomb.GetIndicators().Count();
        //last = bomb.GetSerialNumber()[5];

        //serial = bomb.GetSerialNumber();
        //string newserial;
        //newserial = alphaToInt(serial);

        //serialNumber = Int64.Parse(newserial);
        //serialNumber = Int64.Parse(Convert.ToString(serialNumber, 8));
        //serialNumber += batteries + ports + indicators;
        //serialNumber = Int64.Parse(Convert.ToString(serialNumber, 8));
        //Debug.LogFormat("[Factory Cubes #{0}] Serial after number conversion: {1}", moduleId, serialNumber.ToString());
        FindTools();
        DisplayTheCube();
    }

    private string alphaToInt(string s)
    {
        string s2 = s.Replace("A", "1").Replace("B", "2").Replace("C", "3").Replace("D", "4").Replace("E", "5").Replace("F", "6").Replace("G", "7").Replace("H", "8")
        .Replace("I", "9").Replace("J", "10").Replace("K", "11").Replace("L", "12").Replace("M", "13").Replace("N", "14").Replace("O", "15").Replace("P", "16")
        .Replace("Q", "17").Replace("R", "18").Replace("S", "19").Replace("T", "20").Replace("U", "21").Replace("V", "22").Replace("W", "23")
        .Replace("X", "24").Replace("Y", "25").Replace("Z", "26");

        return s2;
    }

    private string intToMonth(string s)
    {
        string s2 = s.Replace("10", "OCTOBER").Replace("11", "NOVEMBER").Replace("12", "DECEMBER").Replace("1", "JANUARY")
            .Replace("2", "FEBRUARY").Replace("3", "MARCH").Replace("4", "APRIL").Replace("5", "MAY").Replace("6", "JUNE")
            .Replace("7", "JULY").Replace("8", "AUGUST").Replace("9", "SEPTEMBER");

        return s2;
    }
    private void OnDestroy()
    {
        playSound = false;
    }

    void FindTools()
    {
        string month = alphaToInt(intToMonth(DateTime.Today.Month.ToString()));
        //Debug.LogFormat("[Factory Cubes #{0}] This Month: {1}", moduleId, intToMonth(DateTime.Today.Month.ToString()));
        //Debug.LogFormat("[Factory Cubes #{0}] Month Conversion: {1}", moduleId, month);
        long temp = Int64.Parse(month);
        serialNumber += temp;
        //Debug.LogFormat("[Factory Cubes #{0}] Serial after adding month: {1}", moduleId, serialNumber.ToString());
        int choice = 0;
        for (int i = 0; i < month.Length; i++)
        {
            if (Int64.Parse(month[i].ToString()) < 10)
            {
                if (UsableItems[Int64.Parse(month[i].ToString()) % 10] == "")
                {
                    UsableItems[Int64.Parse(month[i].ToString()) % 10] = CubeTools[choice];
                    choice++;
                }
            }
            if (choice >= 3)
            {
                break;
            }
        }
        for (int i = 0; i < UsableItems.Length; i++)
        {
            if (UsableItems[i] == "")
            {
                UsableItems[i] = Colors[(last + i) % 16].name;
                cool[i] = Colors[(last + i) % 16];
            }
        }
        //order = serialNumber.ToString();
        //Debug.LogFormat("[Factory Cubes #{0}] Submission String: {1}", moduleId, order);
        for (int i = 0; i < UsableItems.Length; i++)
        {
            Debug.LogFormat("[Factory Cubes #{0}] {1}th tool is {2}", moduleId, i, UsableItems[i]);
        }
        for (int i = 0; i < UsableItems.Length; i++)
        {
            for (int j = 0; j < Colors.Length; j++)
            {
                if (UsableItems[i] == Colors[j].name)
                {
                    ToolRender[i].material = Colors[j];
                }
            }
        }
    }

    void DisplayTheCube()
    {
        string[] GenerateCube = UsableItems;
        GenerateCube.Remove(UsableItems.IndexOf("top"));
        GenerateCube.Remove(UsableItems.IndexOf("left"));
        GenerateCube.Remove(UsableItems.IndexOf("right"));
        for (int i = 0; i < 4; i++)
        {
            int y = Rnd.Range(0, UsableItems.Length);
            while(UsableItems[y] == "top"|| UsableItems[y] == "left"|| UsableItems[y] == "right")
            {
                y = (y + 1) % UsableItems.Length;
            }
            DisplayCubeSides[i].material = cool[y];
        }
        Debug.LogFormat("[Factory Cubes #{0}] Stage {1}: top-left: {2}, top-right: {3}, bottom-left: {4}, bottom-right: {5}", moduleId, stages+1, DisplayCubeSides[0].material.name.Replace(" (Instance)", ""), DisplayCubeSides[1].material.name.Replace(" (Instance)", ""), DisplayCubeSides[2].material.name.Replace(" (Instance)", ""), DisplayCubeSides[3].material.name.Replace(" (Instance)", ""));

    }
    // Update is called once per frame
    void Update() {
        if (coverTop)
        {
            CoverTop.SetActive(true);
        }
        else
        {
            CoverTop.SetActive(false);
        }
        if (coverLeft)
        {
            CoverLeft.SetActive(true);
        }
        else
        {
            CoverLeft.SetActive(false);
        }
        if (coverRight)
        {
            CoverRight.SetActive(true);
        }
        else
        {
            CoverRight.SetActive(false);
        }

    }

    void Button(int pressedKey)
    {
        if (UsableItems[pressedKey] == "top")
        {
            audio.PlaySoundAtTransform("otherSound", transform);
            if (coverTop)
            {
                coverTop = false;
            }
            else
            {
                coverTop = true;
            }
        }
        else if (UsableItems[pressedKey] == "left")
        {
            audio.PlaySoundAtTransform("otherSound", transform);
            if (coverTop == false)
            {
                if (coverLeft)
                {
                    coverLeft = false;
                }
                else
                {
                    coverLeft = true;
                }
            }

        }
        else if (UsableItems[pressedKey] == "right")
        {
            audio.PlaySoundAtTransform("otherSound", transform);
            if (coverTop == false)
            {
                if (coverRight)
                {
                    coverRight = false;
                }
                else
                {
                    coverRight = true;
                }
            }
        }
        else
        {
            audio.PlaySoundAtTransform("paintSound", transform);
            if ((coverLeft && coverRight) || (coverLeft && coverRight && coverTop))
            {

            }
            else if (coverTop && coverLeft)
            {
                CubeSides[3].material = cool[pressedKey];
            }
            else if (coverTop && coverRight)
            {
                CubeSides[2].material = cool[pressedKey];
            }
            else if (coverTop)
            {
                CubeSides[2].material = cool[pressedKey];
                CubeSides[3].material = cool[pressedKey];
            }
            else if (coverRight)
            {
                CubeSides[0].material = cool[pressedKey];
                CubeSides[2].material = cool[pressedKey];
            }
            else if (coverLeft)
            {
                CubeSides[1].material = cool[pressedKey];
                CubeSides[3].material = cool[pressedKey];
            }
            else
            {
                CubeSides[0].material = cool[pressedKey];
                CubeSides[1].material = cool[pressedKey];
                CubeSides[2].material = cool[pressedKey];
                CubeSides[3].material = cool[pressedKey];
            }
        }
        if (isSolved && !sandbox)
        {
            coverTop = false;
            coverLeft = false;
            coverRight = false;
            sandbox = true;
        }
        if (struck)
        {
            for (int i = 0; i < 4; i++)
            {
                CubeSides[i].material = White;
            }
            coverTop = false;
            coverLeft = false;
            coverRight = false;
            struck = false;
        }
        if (!isSolved)
        {
            FunnyMoment();
        }
    }

    void FunnyMoment()
    {

        if (CubeSides[0].material.name == DisplayCubeSides[0].material.name && CubeSides[1].material.name == DisplayCubeSides[1].material.name && CubeSides[2].material.name == DisplayCubeSides[2].material.name && CubeSides[3].material.name == DisplayCubeSides[3].material.name && !coverTop && !coverLeft && !coverRight && !isSolved)
        {
            if(stages == 2)
            {
                bombModule.HandlePass();
                isSolved = true;
                audio.PlaySoundAtTransform("solve", transform);
            }
            else
            {
                stages++;
                for (int i = 0; i < 4; i++)
                {
                    CubeSides[i].material = White;
                    DisplayCubeSides[i].material = White;
                }
                DisplayTheCube();
                coverTop = false;
                coverLeft = false;
                coverRight = false;
                audio.PlaySoundAtTransform("stage", transform);
            }
        }

    }

    /*
    void OrderInTheCourt(int pressedKey)
    {
        if (!isSolved)
        {
            
            if (order[ordered].ToString() == pressedKey.ToString())
            {
                ordered++;
            }
            else
            {
                Debug.LogFormat("{0}", order[ordered]);
                bombModule.HandleStrike();
                ordered = 0;
                struck = true;
                coverTop = false;
                coverLeft = false;
                coverRight = false;
            }
            if (ordered == order.Length)
            {
                bombModule.HandlePass();
                isSolved = true;
            }
        }
    }
    */
    
	//twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"To press the buttons on the module, use !{0} press [0-9] (The numbers can be chained. Example: !{0} press 69420)";
    #pragma warning restore 414
    
    IEnumerator ProcessTwitchCommand(string command)
    {
		string[] parameters = command.Split(' ');
		if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (parameters.Length != 2)
			{
				yield return "sendtochaterror Parameter length invalid. Command ignored.";
				yield break;
			}
			
			if (parameters[1].Length == 0)
			{
				yield return "sendtochaterror Number length invalid. Command ignored.";
				yield break;
			}
			
			for (int x = 0; x < parameters[1].Length; x++)
			{
				int Out;
				if (!int.TryParse(parameters[1][x].ToString(), out Out))
				{
					yield return "sendtochaterror A number given is not valid. Command ignored.";
					yield break;
				}
				Tools[Out].OnInteract();
				yield return new WaitForSecondsRealtime(0.1f);
			}
		}
	}
}
