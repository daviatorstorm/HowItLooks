using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HowItLooks.Entities;
using HowItLooks.Extension;
using HowItLooks.Models;
using HowItLooks.Services;
using System.Collections.ObjectModel;
using static SQLite.SQLite3;

namespace HowItLooks.ViewModels
{
    public partial class GroupDetailsViewModel : ObservableObject
    {
        private readonly DatabaseService _db;

        [ObservableProperty]
        private ObservableCollection<Enemy> enemies;

        [ObservableProperty]
        private GroupEntity group;

        public GroupDetailsViewModel(DatabaseService db)
        {
            _db = db;
        }
        public void Init(GroupEntity group)
        {
            Group = group;
            var enemyEntities = _db.GetEnemiesByGroupId(group.Id);
            Enemies = new ObservableCollection<Enemy>(enemyEntities.Select(e => new Enemy(e)));
        }

        [RelayCommand]
        public async Task AddEnemyAsync()
        {
            var typeMap = new Dictionary<string, CreatureType>
            {
                { "Player", CreatureType.Player },
                { "Monster", CreatureType.Monster },
                { "NPC", CreatureType.NPC }
            };

            string selected = await Shell.Current.DisplayActionSheet(
                Translator.Instance["ChooseCreatureType"],
                Translator.Instance["Cancel"],
                null,
                typeMap.Keys.ToArray()
            );

            if (string.IsNullOrWhiteSpace(selected) || selected == Translator.Instance["Cancel"] || !typeMap.ContainsKey(selected))
                return;

            string name = await Shell.Current.DisplayPromptAsync(
                Translator.Instance["AddACreature"],
                Translator.Instance["NameTheCreature"],
                Translator.Instance["Save"],
                Translator.Instance["Cancel"]);

            if (string.IsNullOrWhiteSpace(name))
                return;

            name = char.ToUpper(name[0]) + name.Substring(1);

            string hpStr = await Shell.Current.DisplayPromptAsync(
                Translator.Instance["HP"],
                Translator.Instance["HowMuchHp"],
                Translator.Instance["Save"],
                Translator.Instance["Cancel"],
                keyboard: Keyboard.Numeric,
                initialValue: "1");

            if (!int.TryParse(hpStr, out int hp)) return;

            string armorStr = await Shell.Current.DisplayPromptAsync(
                "Armor",
                "how much Armor",
                Translator.Instance["Save"],
                Translator.Instance["Cancel"],
                keyboard: Keyboard.Numeric,
                initialValue: "1");

            if (!int.TryParse(armorStr, out int armor)) return;

            var entity = _db.AddMonster(name, hp, armor, Group.Id);
            entity.CreatureType = typeMap[selected];
            entity.GroupId = Group.Id;
            _db.UpdateMonster(entity);

            var newEnemy = new Enemy(entity);
            Enemies.Add(newEnemy);
        }

        [RelayCommand]
        public async Task RemoveEnemyAsync(Enemy enemy)
        {
            bool confirm = await Shell.Current.DisplayAlert(
                Translator.Instance["Removal"],
                Translator.Instance["RemoveTheMonster"],
                Translator.Instance["Delete"],
                Translator.Instance["Cancel"]);

            if (!confirm) return;

            _db.DeleteMonster(new EnemyEntity(enemy));
            Enemies.Remove(enemy);
        }

        [RelayCommand]
        public async Task ChangeNameAsync(Enemy enemy)
        {
            string result = await Shell.Current.DisplayPromptAsync(
                Translator.Instance["ChangeName"],
                Translator.Instance["WhatNameWillYouChange"],
                "OK",
                Translator.Instance["Cancel"],
                initialValue: enemy.Name);

            if (string.IsNullOrWhiteSpace(result)) return;
            
            result = char.ToUpper(result[0]) + result.Substring(1);
            enemy.Name = result;
            _db.UpdateMonster(new EnemyEntity(enemy));
        }

        [RelayCommand]
        public async Task ChangeInitiativeAsync(Enemy enemy)
        {
            string result = await Shell.Current.DisplayPromptAsync(
                Translator.Instance["ChangeOfInitiative"],
                Translator.Instance["EnterNewInitiative"],
                "OK",
                Translator.Instance["Cancel"],
                keyboard: Keyboard.Numeric);

            if (int.TryParse(result, out int newInit))
            {
                enemy.Initiative = newInit;
                _db.UpdateMonster(new EnemyEntity(enemy));
                SortEnemies();
            }
        }

        private void SortEnemies()
        {
            var sorted = Enemies.OrderByDescending(e => e.Initiative).ToList();
            for (int i = 0; i < sorted.Count; i++)
            {
                if (Enemies[i] != sorted[i])
                    Enemies.Move(Enemies.IndexOf(sorted[i]), i);
            }
        }

        [RelayCommand]
        public async Task ChangeHPAsync(Enemy enemy)
        {
            string action = await Shell.Current.DisplayActionSheet(
                Translator.Instance["ChangeHP"],
                Translator.Instance["Cancel"],
                null,
                Translator.Instance["ChangeRegularHP"],
                Translator.Instance["ChangeTempHP"]);

            if (action == Translator.Instance["ChangeRegularHP"])
            {
                string result = await Shell.Current.DisplayPromptAsync(
                    Translator.Instance["ChangeHP"],
                    Translator.Instance["HowMuchToChangeHP"],
                    "OK",
                    Translator.Instance["Cancel"],
                    keyboard: Keyboard.Numeric);

                if (int.TryParse(result, out int hp))
                {
                    enemy.UpdateHitPoints(hp);
                    _db.UpdateMonster(new EnemyEntity(enemy));
                }
            }
            else if (action == Translator.Instance["ChangeTempHP"])
            {
                string result = await Shell.Current.DisplayPromptAsync(
                    Translator.Instance["ChangeTempHP"],
                    Translator.Instance["HowMuchToChangeTempHP"],
                    "OK",
                    Translator.Instance["Cancel"],
                    keyboard: Keyboard.Numeric);

                if (int.TryParse(result, out int tempHp))
                {
                    enemy.TempHitPoints = tempHp;
                    _db.UpdateMonster(new EnemyEntity(enemy));
                }
            }
        }

        [RelayCommand]
        public async Task ChangeArmorAsync(Enemy enemy)
        {
            string result = await Shell.Current.DisplayPromptAsync(
                Translator.Instance["ArmorClass"],
                Translator.Instance["EnterNewAC"],
                "OK",
                Translator.Instance["Cancel"],
                keyboard: Keyboard.Numeric);

            if (int.TryParse(result, out int ac))
            {
                enemy.ArmorClass = ac;
                _db.UpdateMonster(new EnemyEntity(enemy));
            }
        }

        [RelayCommand]
        public async Task ChangeCreatureTypeAsync(Enemy enemy)
        {
            string selected = await Shell.Current.DisplayActionSheet(
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
        }

        [RelayCommand]
        public async Task AddSingleEnemyToMainPageAsync(Enemy enemy)
        {
            bool confirm = await Shell.Current.DisplayAlert(
                $"{Translator.Instance["AddToBattle"]}?",
                $"{enemy.Name} {Translator.Instance["ToTheMainPage"]}?",
                Translator.Instance["Yes"],
                Translator.Instance["No"]);

            if (!confirm) return;

            var entity = _db.AddMonster(enemy.Name);
            entity.HitPoints = enemy.HitPoints;
            entity.HitPointsLeft = enemy.HitPointsLeft;
            entity.Initiative = enemy.Initiative;
            entity.ArmorClass = enemy.ArmorClass;
            entity.TempHitPoints = enemy.TempHitPoints;
            entity.CreatureType = enemy.CreatureType;
            entity.GroupId = null;

            _db.UpdateMonster(entity);
            MessagingCenter.Send(this, "EnemiesUpdated");

            await Shell.Current.DisplayAlert(
                Translator.Instance["Success"],
                $"{enemy.Name} {Translator.Instance["AddedInMainPage"]}",
                "OK");
        }

        [RelayCommand]
        public async Task AddGroupToMainPageAsync()
        {
            if (Enemies.Count == 0)
            {
                await Shell.Current.DisplayAlert(
                    Translator.Instance["Info"],
                    Translator.Instance["ThisGroupHasNoEnemies"],
                    "OK");
                return;
            }

            foreach (var enemy in Enemies)
            {
                var entity = _db.AddMonster(enemy.Name);
                entity.HitPoints = enemy.HitPoints;
                entity.HitPointsLeft = enemy.HitPointsLeft;
                entity.Initiative = enemy.Initiative;
                entity.ArmorClass = enemy.ArmorClass;
                entity.TempHitPoints = enemy.TempHitPoints;
                entity.CreatureType = enemy.CreatureType;
                entity.GroupId = null;
                _db.UpdateMonster(entity);
            }

            MessagingCenter.Send(this, "EnemiesUpdated");

            await Shell.Current.DisplayAlert(
                Translator.Instance["Success"],
                $"{Enemies.Count} {Translator.Instance["AddedInMainPage"]}",
                "OK");
        }
    }
}
