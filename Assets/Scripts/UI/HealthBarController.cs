using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    private Health bossHealth;
    [SerializeField] private RectTransform playerHealthIndicator;
    [SerializeField] private Image blackFon;
    [SerializeField] private CutSceneController cutSceneController;
    [SerializeField] private DeathPopup deathPopup;

    [SerializeField] private GameObject healthPotionInterfaceElements;
    [SerializeField] private Text healPotionCountText;

    [SerializeField] private GameObject healthBarObj;
    [SerializeField] private Text bossNameText;
    [SerializeField] private RectTransform bossHealthIndicator;
    [SerializeField] private RectTransform bossBloodNameFiller;

    public static Action startLevelEvent;
    public static Action endLevelEvent;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartHideBlackScreen();
        blackFon.gameObject.SetActive(true);
        cutSceneController.gameObject.SetActive(true);
        
        startLevelEvent += StartHideBlackScreen;
        endLevelEvent += StartShowingBlackScreen;
        
        SceneController.exitToMenu += DestroySelf;
        SceneController.restartLvl += HideDeathPopup;
        SceneController.restartLvl += UpdatePlayerHealthBar;
        
        SceneController.actionGetInterfaceController?.Invoke(this);
        
        playerHealth.onPersonTakeDamage += UpdatePlayerHealthBar;
        playerHealth.onPersonHealing += UpdatePlayerHealthBar;

        PlayerController.onHealPotionCountChange += UpdateViewCountHealPotion;
    }

    private void UpdateViewCountHealPotion(int count)
    {
        if (healthPotionInterfaceElements.activeSelf == false)
        {
            healthPotionInterfaceElements.SetActive(true);
        }
        
        healPotionCountText.text = count.ToString();
    }

    private void HideDeathPopup()
    {
        deathPopup.Hide();
    }

    private void UpdatePlayerHealthBar()
    {
        var width = playerHealth.CurrentHp / playerHealth.MaxHp;
        playerHealthIndicator.localScale = new Vector3(width, 1f, 1f);
    }
    
    private void UpdateBossHealthBar()
    {
        var width = bossHealth.CurrentHp / bossHealth.MaxHp;
        bossHealthIndicator.localScale = new Vector3(width, 1f, 1f);
    }

    public void SetBossHealth(Health newBossHealth, string bossName)
    {
        bossHealth = newBossHealth;
        bossHealth.onPersonTakeDamage += UpdateBossHealthBar;
        bossNameText.text = bossName;
        StartCoroutine(StartShowingBossHealthBar());
    }

    private IEnumerator StartShowingBossHealthBar()
    {
        var currentPosY = healthBarObj.transform.localPosition.y;
        while (currentPosY > 471.64f)
        {
            currentPosY -= Time.deltaTime * 75f;
            
            healthBarObj.transform.localPosition = new Vector3(0f, currentPosY, 1f);
            yield return null;
        }
    }
    
    private IEnumerator StartHidingBossHealthBar()
    {
        var currentPosY = healthBarObj.transform.localPosition.y;
        while (currentPosY < 650)
        {
            currentPosY += Time.deltaTime * 75f;
            
            healthBarObj.transform.localPosition = new Vector3(0f, currentPosY, 1f);
            yield return null;
        }

        bossBloodNameFiller.transform.localScale = new Vector3(1f, 0f, 1f);
    }

    public void ShowBloodOnBossName()
    {
        StartCoroutine(StartShowingBloodOnBossName());
    }

    private IEnumerator StartShowingBloodOnBossName()
    {
        var indicatorScaleY = 0f;

        while (indicatorScaleY < 1f)
        {
            indicatorScaleY += Time.deltaTime;
            bossBloodNameFiller.localScale = new Vector3(1f, indicatorScaleY, 1f);
            yield return null;
        }

        StartCoroutine(StartHidingBossHealthBar());
    }

    private void StartShowingBlackScreen()
    {
        StartCoroutine(ShowBlackScreen());
    }
    
    private void StartHideBlackScreen()
    {
        StartCoroutine(HideBlackScreen());
    }
    
    private IEnumerator ShowBlackScreen()
    {
        var startColor = blackFon.color;
        var alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime;
            
            blackFon.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }
    }
    
    private IEnumerator HideBlackScreen()
    {
        var startColor = blackFon.color;
        var alpha = 1f;
        while (alpha >= 0)
        {
            alpha -= Time.deltaTime;
            
            blackFon.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }
    }
    
    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        SceneController.exitToMenu -= DestroySelf;
        SceneController.restartLvl -= HideDeathPopup;
        SceneController.restartLvl -= UpdatePlayerHealthBar;
        
        startLevelEvent -= StartHideBlackScreen;
        endLevelEvent -= StartShowingBlackScreen;
        
        playerHealth.onPersonTakeDamage -= UpdatePlayerHealthBar;
        playerHealth.onPersonHealing -= UpdatePlayerHealthBar;

        if (bossHealth)
        {
            bossHealth.onPersonTakeDamage -= UpdateBossHealthBar;
        }

        PlayerController.onHealPotionCountChange -= UpdateViewCountHealPotion;
    }
}
