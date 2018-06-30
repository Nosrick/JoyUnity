using JoyLib.Code.Graphics;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        ObjectIcons.Load();
        
        JoyObject wizard = Instantiate(Resources.Load<JoyObject>("Prefabs/Sprite"));
        wizard.Initialise("Wizard", 1, new Vector2Int(0, 0), ObjectIcons.GetIcons("Jobs", "Hedge Wizard"), "Test", true);

        JoyObject warrior = Instantiate(Resources.Load<JoyObject>("Prefabs/Sprite"));
        warrior.Initialise("Warrior", 1, new Vector2Int(1, 0), ObjectIcons.GetIcons("Jobs", "Warrior"), "Test", true);
        
        JoyObject thief = Instantiate(Resources.Load<JoyObject>("Prefabs/Sprite"));
        thief.Initialise("Thief", 1, new Vector2Int(2, 0), ObjectIcons.GetIcons("Jobs", "Thief"), "Test", true);

        JoyObject naga = Instantiate(Resources.Load<JoyObject>("Prefabs/Sprite"));
        naga.Initialise("Naga", 1, new Vector2Int(1, 1), ObjectIcons.GetIcons("Reptiles", "Naga"), "Test", true);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
