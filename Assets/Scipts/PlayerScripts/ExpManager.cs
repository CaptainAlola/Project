using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpManager : MonoBehaviour
{
    public int level;
    public int currectExp;
    public int expToLevel = 10;
    public float expGrowthMultiplier = 1.2f;
    public Slider expSlider;
    public TMP_Text currentLevelText;


    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            GainExperience(2);
        }
    }


    private void OnEnable()
    {
        Enemy_Health.OnMonsterDefeated += GainExperience;
    }
    private void OnDisable()
    {
        Enemy_Health.OnMonsterDefeated -= GainExperience;
    }


    public void GainExperience(int amount)
    {
        currectExp += amount;
        if (currectExp >= expToLevel)
        {
            LevelUp(); 
        }

        UpdateUI();

    }

    private void LevelUp() 
    {
        level++;
        currectExp -= expToLevel;
        expToLevel = Mathf.RoundToInt(expToLevel * expGrowthMultiplier);
    }

    public void UpdateUI() 
    {
        expSlider.maxValue = expToLevel;
        expSlider.value = currectExp;
        currentLevelText.text = "Level: " + level;
    }
}
