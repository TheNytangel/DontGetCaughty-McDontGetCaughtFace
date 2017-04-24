using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private struct SaveState
    {
        public Dictionary<string, bool> keys;
        public Dictionary<string, bool> doors;
        public Vector3 respawnLocation;
        public bool alarmIsSounding;
    };

    public static GameManager instance;
    public GameObject player;
    private SaveState saveState;

    public GameObject game;
    public GameObject deadScreen;

    public GameObject eText;
    public GameObject eOpen;
    public GameObject eEscape;

    public GameObject[] flashlights;
    public GameObject[] keys;
    public GameObject[] doors;
    public AudioSource alarm;

    private int lives;

    // Use this for initialization
    void Start ()
    {
        // Do the singleton
		if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        alarm = GetComponent<AudioSource>();

        // Initialize they dictionary in the save state BECAUSE APPARENTLY YOU HAVE TO DO THAT AND IT TOOK ME 15 MINUTES TO FIGURE OUT THE ERROR
        saveState.keys = new Dictionary<string, bool>(4);
        saveState.doors = new Dictionary<string, bool>(3);
        
        ResetEverything();
    }

    private void ResetEverything()
    {
        // Make sure the right object is active
        game.SetActive(true);
        deadScreen.SetActive(false);

        // Set the lives to 3 and make sure the "lives" images get reset
        lives = 3;
        foreach (GameObject image in flashlights)
        {
            image.SetActive(true);
        }
        // Set all the keys to active
        foreach (GameObject key in keys)
        {
            key.SetActive(true);
        }
        // Set all the doors to closed
        foreach (GameObject door in doors)
        {
            door.GetComponent<Door>().CloseImmediately();
        }
        // Set all guards to their spawn positions
        foreach (GameObject guard in GameObject.FindGameObjectsWithTag("Guard"))
        {
            guard.GetComponent<Guard>().ResetGuard();
        }

        // Stop the alarm from playing if it is
        alarm.Stop();
    }

    // Update is called once per frame
    void Update ()
    {
        // If the user has died and they press space, go back to the game
		if(deadScreen.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            deadScreen.SetActive(false);
            game.SetActive(true);
            // If the user hasn't lost yet, set them back to the last checkpoint
            LoadSaveState();
        }

        // If user presses escape, go back to main menu
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
	}

    public void SetCheckpoint(Vector3 newLocation)
    {
        // Save the location they will spawn at if they die
        saveState.respawnLocation = newLocation + new Vector3(0, 1);

        // Clear the dictionaries otherwise it yells at me because there are already keys with the same name
        saveState.keys.Clear();
        saveState.doors.Clear();

        // Save the keys that the player has acquired so far
        for (int i = 0; i < keys.Length; ++i)
        {
            Key tempKey = keys[i].GetComponent<Key>();
            saveState.keys.Add(tempKey.usedFor, tempKey.owned);
        }

        // Get the open state of doors and save them the most janky way possible
        for (int i = 0; i < doors.Length; ++i)
        {
            saveState.doors.Add(doors[i].GetComponent<KeyedDoor>().requiredKey, doors[i].GetComponent<Door>().isOpen());
        }

        saveState.alarmIsSounding = alarm.isPlaying;
    }

    public void Seen()
    {
        // Decrement lives and remove a flashlight
        --lives;
        flashlights[lives].SetActive(false);

        // If the user goes to 0 lives, they have now lost
        if (lives == 0)
        {
            SceneManager.LoadScene("Lose");
            return;
        }

        alarm.Stop();
        
        game.SetActive(false);
        deadScreen.SetActive(true);
    }

    private void LoadSaveState()
    {
        // Move the player to the checkpoint location
        player.transform.position = saveState.respawnLocation;

        // Go through the keys to see which ones they player had before
        for(int i = 0; i < keys.Length; ++i)
        {
            // Set the key to active. It will be deactivated later if the player has it
            keys[i].SetActive(true);

            // Get the key component of the key
            Key tempKey = keys[i].GetComponent<Key>();

            // Find the value (if it was owned) in the dictionary for what that key is used for
            saveState.keys.TryGetValue(tempKey.usedFor, out tempKey.owned);

            // If it was owned by the player, set it to inactive since they already have it now
            if(tempKey.owned)
            {
                keys[i].SetActive(false);
            }
        }

        // Go through the doors and open/close them
        for (int i = 0; i < doors.Length; ++i)
        {
            bool openThisDoor = false;
            // Find whether the door was open or not in the dictionary
            saveState.doors.TryGetValue(doors[i].GetComponent<KeyedDoor>().requiredKey, out openThisDoor);

            // If it was open, open it. If it was closed, close it.
            if (openThisDoor)
            {
                doors[i].GetComponent<Door>().OpenImmediately();
            }
            else
            {
                doors[i].GetComponent<Door>().CloseImmediately();
            }
        }
        
        // Reset guards to start positions
        foreach (GameObject guard in GameObject.FindGameObjectsWithTag("Guard"))
        {
            guard.GetComponent<Guard>().ResetGuard();
        }

        // Play the alarm inside the warehouse if it was playing before
        if(saveState.alarmIsSounding)
        {
            alarm.Play();
        }
        else
        {
            alarm.Stop();
        }
    }
}
