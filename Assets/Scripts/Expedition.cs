using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Expedition : MonoBehaviour
{

    public List<GameObject> expeditionTeam = new(), selectedButtons;
    public List<string> teamMemberNames = new();
    [SerializeField] private Text timerText;
    public ExpeditionData expeditionData;
    public bool inProgress = false;
    public bool isCompleted = false;
    public float expeditionTimer = 0f;
    public int difficulty;
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public List<Image> catImages;
    public Sprite defaultSprite;
    private AudioManager audioManager;
    //private Expedition expedition;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    void OnMouseUp()
    {
        if (pauseMenu.activeSelf)
        {
            return;
        }
        if (optionsMenu.activeSelf)
        {
            return;
        }
        if (isCompleted)
        {
            ClaimRewards();
        }
        else if (!inProgress)
        {
            timerText.text = "";
            ExpeditionManager.instance.OpenTeamSelectUI(this);
        }
    }
    private void OnEnable()
    {
        if (expeditionTimer > 0) StartCoroutine(StartExpeditionTimer());
    }

    public void SetSelectedTeam(List<GameObject> team, List<GameObject> buttons)
    {
        if (expeditionTeam.Count > 0) return;
        expeditionTeam = new List<GameObject>(team);
        selectedButtons = new(buttons);
        teamMemberNames = team.Select(cat => cat.name).ToList();
        foreach (GameObject btn in selectedButtons)
        {
            btn.GetComponent<Image>().color *= .5f;
            btn.transform.Find("CheckBox").GetComponent<Toggle>().enabled = false;
        }
        //foreach (GameObject cat in team) CatFaces(true, cat.GetComponent<SpriteRenderer>().sprite);
    }

    public bool SendCatsonExpedition()
    {
        if (expeditionTeam.Count == 0)
        {
            Debug.Log("No cats selected for the expedition!");
            return false;
        }

        if (!EligibleCatStats())
        {
            Debug.Log("The cats on your team have too low stats to go on the expedition!");
            return false;
        }

        Debug.Log("Sending the following cats on an expedition:");
        foreach (GameObject cat in expeditionTeam)
        {
            Debug.Log(cat.name);
        }

        //expeditionTeam.Clear();
        timerText.text = "";
        isCompleted = false;
        inProgress = true;
        expeditionTimer = expeditionData.expeditionTime;
        //GameManager.instance.ExpeditionCoins = 0;
        StartCoroutine(StartExpeditionTimer());
        return true;
    }

    private bool EligibleCatStats()
    {
        int totalStrength = 0;
        int totalSpeed = 0;
        int totalDefense = 0;

        foreach (GameObject cat in expeditionTeam)
        {
            Cat catComponent = cat.GetComponent<Cat>();
            totalStrength += catComponent.stats.strength;
            totalSpeed += catComponent.stats.speed;
            totalDefense += catComponent.stats.defense;
        }

        if (totalStrength < expeditionData.recommendedStrength * 0.5f ||
        totalSpeed < expeditionData.recommendedSpeed * 0.5f ||
        totalDefense < expeditionData.recommendedDefense * 0.5f)
        {
            return false;
        }
        return true;
    }

    public void CatFaces(bool isOn, Sprite catFace)
    {
        if (isOn)
        {
            foreach (Image image in catImages)
            {
                if (image.sprite == defaultSprite)
                {
                    image.sprite = catFace;
                    break;
                }
            }

        }

        else
        {
            foreach (Image image in catImages)
            {
                if (image.sprite == catFace)
                {
                    image.sprite = defaultSprite;
                    break;
                }
            }
        }
    }



    private IEnumerator StartExpeditionTimer()
    {

        //expeditionTimer = expeditionData.expeditionTime;
        //expedition = this;
        while (expeditionTimer > 0)
        {
            yield return new WaitForSeconds(1f);
            expeditionTimer -= 1f;
            UpdateVisualTimer();
            Debug.Log("Time remaining: " + expeditionTimer + " seconds");
            //expedition = this;

        }

        Debug.Log("Expedition complete! Ready to claim.");
        //UpdateVisualTimer();
        isCompleted = true;
        inProgress = false;
        UpdateVisualTimer();
    }

    public void UpdateVisualTimer()
    {
        if (isCompleted)
        {
            timerText.text = "Expedition Complete!";
        }
        else
        {
            timerText.text = "Time Left: " + Mathf.CeilToInt(expeditionTimer) + "s";
        }
    }

    private void CatsVSExpeditionStats()
    {

        int totalStrength = 0;
        int totalSpeed = 0;
        int totalDefense = 0;

        foreach (GameObject cat in expeditionTeam)
        {
            Cat catComponent = cat.GetComponent<Cat>();
            totalStrength += catComponent.stats.strength;
            totalSpeed += catComponent.stats.speed;
            totalDefense += catComponent.stats.defense;
        }

        int checksPassed = 0;
        Debug.Log($"Total Stats - Strength: {totalStrength}, Speed: {totalSpeed}, Defense: {totalDefense}");
        Debug.Log($"Expedition Requirements - Strength: {expeditionData.recommendedStrength}, Speed: {expeditionData.recommendedSpeed}, Defense: {expeditionData.recommendedDefense}");
        if (PassStatCheck(totalStrength, expeditionData.recommendedStrength)) checksPassed++;
        if (PassStatCheck(totalSpeed, expeditionData.recommendedSpeed)) checksPassed++;
        if (PassStatCheck(totalDefense, expeditionData.recommendedDefense)) checksPassed++;

        switch (checksPassed)
        {

            case 0:
                Debug.Log("Failure! No rewards!");
                audioManager.playFailSound();
                break;

            case 1:
                Debug.Log("Minimal Rewards");
                audioManager.playVictorySound();
                GameManager.instance.AddCoin(20 * difficulty);
                break;

            case 2:
                Debug.Log("All rewards!");
                audioManager.playVictorySound();
                GameManager.instance.AddCoin(40 * difficulty);
                break;

            case 3:
                Debug.Log("All rewards + bonus!");
                audioManager.playVictorySound();
                GameManager.instance.AddCoin(80 * difficulty);
                break;
        }




    }

    private bool PassStatCheck(int totalStats, int recommendedStat)
    {
        float successChance = Mathf.Clamp01((float)totalStats / recommendedStat);
        float roll = Random.Range(0f, 1f);
        return roll <= successChance;
    }


    private void ClaimRewards()
    {
        if (!isCompleted)
        {
            Debug.Log("Expedition not done!");
            return;
        }

        CatsVSExpeditionStats();
        timerText.text = "";
        expeditionTeam.Clear();
        foreach (GameObject btn in selectedButtons)
        {
            btn.GetComponent<Image>().color *= 2;
            btn.transform.Find("CheckBox").GetComponent<Toggle>().enabled = true;
        }
        selectedButtons.Clear();
        foreach (Image image in catImages)
        {
            Debug.Log("Clearing cat images");
            image.sprite = defaultSprite;
        }
        isCompleted = false;
        Debug.Log("Expedition finished, team returned!");
    }


    public void LoadTimer(float savedTimer, bool completed)
    {
        isCompleted = completed;
        if (isCompleted)
        {
            timerText.text = "Expedition Complete!";
            return;
        }
        expeditionTimer = savedTimer;

        if (expeditionTimer > 0) StartCoroutine(StartExpeditionTimer());
        //if(isCompleted == true) UpdateVisualTimer();
        else
        {
            UpdateVisualTimer();
        }
    }


}