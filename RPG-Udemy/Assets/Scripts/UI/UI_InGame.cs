using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI_InGame.cs摘要
/// 游戏内UI管理器，负责显示和更新玩家状态、技能冷却和资源信息
/// 包括生命值、技能冷却时间和灵魂值的实时更新
/// </summary>
public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;  // 玩家属性引用，用于获取生命值信息
    [SerializeField] private Slider slider;            // 玩家生命值滑块

    // 技能冷却UI元素
    [SerializeField] private Image dashImage;          // 冲刺技能冷却图标
    [SerializeField] private Image parryImage;         // 格挡技能冷却图标
    [SerializeField] private Image crystalImage;       // 水晶技能冷却图标
    [SerializeField] private Image swordImage;         // 剑技能冷却图标
    [SerializeField] private Image blackholeImage;     // 黑洞技能冷却图标
    [SerializeField] private Image flaskImage;         // 药水冷却图标

    private SkillManager skills;                       // 技能管理器引用

    [Header("Souls info")]
    [SerializeField] private TextMeshProUGUI currentSouls;  // 当前灵魂值文本
    [SerializeField] private float soulsAmount;             // 当前显示的灵魂值
    [SerializeField] private float increaseRate;            // 灵魂值UI增长速度（平滑效果）

    /// <summary>
    /// 初始化UI组件和事件监听
    /// </summary>
    void Start()
    {
        // 订阅玩家生命值变化事件
        if (playerStats != null)
            playerStats.onHealthChanged += UpdateHealthUI;

        // 获取技能管理器实例
        skills = SkillManager.instance;
    }

    /// <summary>
    /// 每帧更新UI显示和检测技能使用
    /// </summary>
    void Update()
    {
        // 更新灵魂值显示
        UpdataSoulsUI();

        // 检测各种技能按键并更新冷却UI
        // 冲刺技能 (左Shift键)
        if (Input.GetKeyDown(KeyCode.LeftShift) && skills.dash.dashUnlocked)
            SetCooldownOf(dashImage);

        // 格挡技能 (Q键)
        if (Input.GetKeyDown(KeyCode.Q) && skills.parry.parryUnlocked)
            SetCooldownOf(parryImage);

        // 水晶技能 (F键)
        if (Input.GetKeyDown(KeyCode.F) && skills.crystal.crystalUnlocked)
            SetCooldownOf(crystalImage);

        // 剑技能 (鼠标右键)
        if (Input.GetKeyDown(KeyCode.Mouse1) && skills.sword.swordUnlocked)
            SetCooldownOf(swordImage);

        // 黑洞技能 (R键)
        if (Input.GetKeyDown(KeyCode.R) && skills.blackhole.blackHoleUnlocked)
            SetCooldownOf(blackholeImage);

        // 药水使用 (1键) - 需要装备药水
        if (Input.GetKeyDown(KeyCode.Alpha1) && Inventory.instance.GetEquipment(EquipmentType.Flask) != null)
            SetCooldownOf(flaskImage);

        // 持续更新所有技能的冷却显示
        CheckCoolDownOf(dashImage, skills.dash.cooldown);
        CheckCoolDownOf(parryImage, skills.parry.cooldown);
        CheckCoolDownOf(crystalImage, skills.crystal.cooldown);
        CheckCoolDownOf(swordImage, skills.sword.cooldown);
        CheckCoolDownOf(blackholeImage, skills.blackhole.cooldown);
        CheckCoolDownOf(flaskImage, Inventory.instance.flaskCooldown);
    }

    /// <summary>
    /// 更新灵魂值UI显示
    /// 实现平滑增长效果，使数值变化更加自然
    /// </summary>
    private void UpdataSoulsUI()
    {
        // 如果显示的灵魂值小于实际灵魂值，平滑增加显示值
        if (soulsAmount < PlayerManager.instance.GetCurrency())
        {
            soulsAmount += increaseRate * Time.deltaTime;  // 按设定速率平滑增加
        }
        else
            soulsAmount = PlayerManager.instance.GetCurrency();  // 确保不超过实际值

        // 更新UI文本显示
        currentSouls.text = ((int)soulsAmount).ToString();
    }

    /// <summary>
    /// 更新生命值UI显示
    /// 在玩家生命值变化时调用
    /// </summary>
    private void UpdateHealthUI()
    {
        // 设置滑块最大值为玩家最大生命值
        slider.maxValue = playerStats.GetMaxHealthValue();
        // 设置滑块当前值为玩家当前生命值
        slider.value = playerStats.currentHealth;
    }

    /// <summary>
    /// 设置技能冷却UI效果
    /// 当技能被使用时调用
    /// </summary>
    /// <param name="_image">技能冷却图标</param>
    private void SetCooldownOf(Image _image)
    {
        // 如果技能已冷却完毕，重置冷却图标为满
        if (_image.fillAmount <= 0)
            _image.fillAmount = 1;
    }

    /// <summary>
    /// 更新技能冷却UI显示
    /// 随时间减少填充量，实现冷却效果
    /// </summary>
    /// <param name="_image">技能冷却图标</param>
    /// <param name="_coolDown">技能冷却时间(秒)</param>
    private void CheckCoolDownOf(Image _image, float _coolDown)
    {
        // 如果技能正在冷却中，减少填充量
        if (_image.fillAmount > 0)
            _image.fillAmount -= 1 / _coolDown * Time.deltaTime;  // 按冷却时间计算减少速率
    }
}
