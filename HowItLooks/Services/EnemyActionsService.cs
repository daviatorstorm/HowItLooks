using HowItLooks.Entities;
using HowItLooks.Extension;
using HowItLooks.Models;
using HowItLooks.Services;
using System.Collections.ObjectModel;

namespace HowItLooks.Services
{
    class EnemyActionsService
    {
        private readonly DatabaseService _db = new();
        private readonly ActionLogService? _logService;
        public EnemyActionsService(ActionLogService? logManager = null)
        {
            _logService = logManager;
        }

        public async Task ChangeInitiative(Page page, Enemy enemy, Action? onSorted = null)
        {
            string result = await page.DisplayPromptAsync(
                Translator.Instance["ChangeOfInitiative"],
                Translator.Instance["EnterNewInitiative"],
                "OK",
                Translator.Instance["Cancel"],
                keyboard: Keyboard.Numeric);

            if (int.TryParse(result, out int newInitiative))
            {
                int previousInitiative = enemy.Initiative;
                enemy.Initiative = newInitiative;
                _db.UpdateMonster(new EnemyEntity(enemy));
                if (_logService != null && _logService.GetIsRoundState() == true)
                    _logService.LogAction($"{enemy.Name} змінено Initiative з ({previousInitiative}) на ({enemy.Initiative})", enemy.CreatureType.ToString());

                onSorted?.Invoke();
            }
        }

        public async Task IncreaseHP(Page page, Enemy enemy)
        {
            string result = await page.DisplayPromptAsync(
                Translator.Instance["IncreaseTheHP"],
                Translator.Instance["HowMuchToIncreaseTheHP"],
                keyboard: Keyboard.Numeric);

            if (int.TryParse(result, out int hp))
            {
                enemy.IncreaseHitPoints(hp);
                _db.UpdateMonster(new EnemyEntity(enemy));
                if (_logService != null && _logService.GetIsRoundState() == true)
                    _logService.LogAction($"{enemy.Name} збільшено HP на (+{hp})");
            }
        }

        public async Task ChangeHP(Page page, Enemy enemy) 
        {
            string action = await page.DisplayActionSheet(Translator.Instance["ChangeHP"], Translator.Instance["Cancel"], null,
                                             Translator.Instance["ChangeRegularHP"], Translator.Instance["ChangeTempHP"]);

            if (action == Translator.Instance["ChangeRegularHP"])
            {
                string result = await page.DisplayPromptAsync(Translator.Instance["ChangeHP"],
                                                         Translator.Instance["HowMuchToChangeHP"],
                                                         "OK", Translator.Instance["Cancel"],
                                                         keyboard: Keyboard.Numeric);

                if (int.TryParse(result, out int hp))
                {
                    int previousHp = enemy.HitPoints;
                    enemy.UpdateHitPoints(hp);
                    _db.UpdateMonster(new EnemyEntity(enemy));
                    if(_logService != null && _logService.GetIsRoundState() == true)
                        _logService.LogAction($"{enemy.Name} змінено HP з ({previousHp}) на ({enemy.HitPoints})");
                }
            }
            else if (action == Translator.Instance["ChangeTempHP"])
            {
                string result = await page.DisplayPromptAsync(Translator.Instance["ChangeTempHP"],
                                                         Translator.Instance["HowMuchToChangeTempHP"],
                                                         "OK", Translator.Instance["Cancel"],
                                                         keyboard: Keyboard.Numeric);

                if (int.TryParse(result, out int tempHp))
                {
                    int previousTempHp = enemy.TempHitPoints;
                    enemy.TempHitPoints = tempHp;
                    _db.UpdateMonster(new EnemyEntity(enemy));
                    if (_logService != null && _logService.GetIsRoundState() == true)
                        _logService.LogAction($"{enemy.Name} змінено тимчасові HP з ({previousTempHp}) на ({enemy.TempHitPoints})");
                }
            }
        }

        public async Task DecreaseHP(Page page, Enemy enemy)
        {
            string result = await page.DisplayPromptAsync(
                Translator.Instance["ReduceHP"],
                Translator.Instance["HowMuchToReduceTheHP"],
                "OK",
                Translator.Instance["Cancel"],
                keyboard: Keyboard.Numeric);

            if (int.TryParse(result, out int hp))
            {
                int oldHP = enemy.HitPointsLeft;
                int oldTempHP = enemy.TempHitPoints;

                enemy.DecreaseHitPoints(hp);
                _db.UpdateMonster(new EnemyEntity(enemy));

                if(_logService != null && _logService.GetIsRoundState() == true)
                {
                    string logMessage;

                    if (oldTempHP > 0)
                    {
                        int lostTemp = Math.Max(0, oldTempHP - enemy.TempHitPoints);
                        int lostReal = Math.Max(0, oldHP - enemy.HitPointsLeft);

                        if (lostTemp > 0 && lostReal > 0)
                        {
                            logMessage = $"{enemy.Name} отримав −{hp} HP: " +
                                         $"{lostTemp} тимчасових (TempHP {oldTempHP} → {enemy.TempHitPoints}) і " +
                                         $"{lostReal} звичайних (HP {oldHP} → {enemy.HitPointsLeft})";
                        }
                        else if (lostTemp > 0)
                        {
                            logMessage = $"{enemy.Name} отримав −{lostTemp} тимчасових HP (TempHP {oldTempHP} → {enemy.TempHitPoints})";
                        }
                        else
                        {
                            logMessage = $"{enemy.Name} отримав −{lostReal} HP (HP {oldHP} → {enemy.HitPointsLeft})";
                        }
                    }
                    else
                    {
                        logMessage = $"{enemy.Name} отримав −{hp} HP ({oldHP} → {enemy.HitPointsLeft})";
                    }

                    _logService.LogAction(logMessage, enemy.CreatureType.ToString());
                }
            }
        }

        public async Task ChangeName(Page page, Enemy enemy)
        {
            string result = await page.DisplayPromptAsync(
                Translator.Instance["ChangeName"],
                Translator.Instance["WhatNameWillYouChange"],
                "OK",
                Translator.Instance["Cancel"],
                initialValue: enemy.Name);

            if (!string.IsNullOrWhiteSpace(result))
            {
                if (_logService != null && _logService.GetIsRoundState() == true)
                    _logService.LogAction($"{enemy.Name} Змінено Імя на ({result})", enemy.CreatureType.ToString());

                enemy.Name = result;
                _db.UpdateMonster(new EnemyEntity(enemy));
            }
        }

        public async Task ChangeArmor(Page page, Enemy enemy)
        {
            string result = await page.DisplayPromptAsync(
                Translator.Instance["ArmorClass"],
                Translator.Instance["EnterNewAC"],
                "OK",
                Translator.Instance["Cancel"],
                maxLength: 2,
                keyboard: Keyboard.Numeric);

            if (int.TryParse(result, out int newAC))
            {
                enemy.ArmorClass = newAC;
                _db.UpdateMonster(new EnemyEntity(enemy));

                if (_logService != null && _logService.GetIsRoundState() == true)
                    _logService.LogAction($"{enemy.Name} Змінено АС на ({result})", enemy.CreatureType.ToString());
            }
        }

        public async Task ChangeCreatureType(Page page, Enemy enemy)
        {
            string selected = await page.DisplayActionSheet(
                Translator.Instance["ChangeCreatureType"],
                Translator.Instance["Cancel"],
                null,
                "Player", "Monster", "NPC");

            if (selected == null || selected == Translator.Instance["Cancel"])
                return;

            enemy.CreatureType = selected switch
            {
                "Player" => CreatureType.Player,
                "Monster" => CreatureType.Monster,
                "NPC" => CreatureType.NPC,
                _ => enemy.CreatureType
            };

            _db.UpdateMonster(new EnemyEntity(enemy));

            if (_logService != null && _logService.GetIsRoundState() == true)
                _logService.LogAction($"{enemy.Name} Змінено CreatureType на ({enemy.CreatureType})", enemy.CreatureType.ToString());
        }

        public async Task<bool> RemoveEnemy(Page page,
                                            Enemy enemy,
                                            ObservableCollection<Enemy> enemies,
                                            Func<Enemy?> getActiveEnemy = null,
                                            Action<Enemy?> setActiveEnemy = null)
        {
            if (enemy == null) return false;

            bool confirm = await page.DisplayAlert(
                Translator.Instance["Removal"],
                Translator.Instance["RemoveTheMonster"],
                Translator.Instance["Delete"],
                Translator.Instance["Cancel"]);

            if (!confirm) return false;

            int index = enemies.IndexOf(enemy);
            enemies.Remove(enemy);
            if (_logService != null && _logService.GetIsRoundState() == true)
                _logService.LogAction($"{enemy.Name} Deleted", enemy.CreatureType.ToString());
            _db.DeleteMonster(new EnemyEntity(enemy));

            if (getActiveEnemy == null || setActiveEnemy == null)
                return true;

            var activeEnemy = getActiveEnemy();

            if (enemies.Count == 0)
            {
                setActiveEnemy(null);
                return true;
            }

            if (activeEnemy == enemy)
            {
                int nextIndex = Math.Min(index, enemies.Count - 1);
                Enemy newActiveEnemy = enemies[nextIndex];
                setActiveEnemy(newActiveEnemy);
                _db.UpdateMonster(new EnemyEntity(newActiveEnemy));
            }

            return true;
        }

        public async Task AddEnemy( Page page,
                                    ObservableCollection<Enemy> enemies,
                                    Func<Enemy?> getActiveEnemy = null,
                                    Action<Enemy?> setActiveEnemy = null,
                                    int? setGroupId = null)
        {
            var typeMap = new Dictionary<string, CreatureType>
            {
                { "Player", CreatureType.Player },
                { "Monster", CreatureType.Monster },
                { "NPC", CreatureType.NPC }
            };

            string selected = await page.DisplayActionSheet(
                Translator.Instance["ChooseCreatureType"],
                Translator.Instance["Cancel"],
                null,
                typeMap.Keys.ToArray()
            );

            if (string.IsNullOrWhiteSpace(selected) ||
                selected == Translator.Instance["Cancel"] ||
                !typeMap.ContainsKey(selected))
                return;

            CreatureType selectedType = typeMap[selected];

            string name = await page.DisplayPromptAsync(
                Translator.Instance["AddACreature"],
                Translator.Instance["NameTheCreature"],
                Translator.Instance["Save"],
                Translator.Instance["Cancel"]
            );

            if (string.IsNullOrWhiteSpace(name)) return;

            string hpStr = await page.DisplayPromptAsync(
                Translator.Instance["HP"],
                Translator.Instance["HowMuchHp"],
                Translator.Instance["Save"],
                Translator.Instance["Cancel"],
                keyboard: Keyboard.Numeric,
                initialValue: "1"
            );

            if (!int.TryParse(hpStr, out int hp)) return;

            string armorStr = await page.DisplayPromptAsync(
                "Armor",
                "how much Armor",
                Translator.Instance["Save"],
                Translator.Instance["Cancel"],
                maxLength: 2,
                keyboard: Keyboard.Numeric,
                initialValue: "1"
            );

            if (!int.TryParse(armorStr, out int armor)) return;

            var entity = _db.AddMonster(name, hp, armor, setGroupId);
            entity.CreatureType = selectedType;
            _db.UpdateMonster(entity);

            var addedEnemy = new Enemy(entity);
            enemies.Add(addedEnemy);

            if (_logService != null && _logService.GetIsRoundState() == true)
                _logService.LogAction($"Додано -> {name}", entity.CreatureType.ToString());

            if (getActiveEnemy == null || setActiveEnemy == null)
                return;

            if (enemies.Count == 1)
                setActiveEnemy(addedEnemy);
        }
    }
}
