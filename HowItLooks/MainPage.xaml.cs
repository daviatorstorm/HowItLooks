﻿using HowItLooks.Entities;
using HowItLooks.Models;
using HowItLooks.Services;
using System.Collections.ObjectModel;
using HowItLooks.Extension;

namespace HowItLooks
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Enemy> Enemies { get; set; }
        private Enemy? _activeEnemy;
        private readonly DatabaseService _db;

        private bool _isRoundStarted = false;
        private int _roundCounter = 1;

        public int RoundCounter
        {
            get => _roundCounter;
            set
            {
                if (_roundCounter != value)
                {
                    _roundCounter = value;
                    OnPropertyChanged(nameof(RoundCounter));
                    OnPropertyChanged(nameof(RoundDisplayText));
                }
            }
        }
        public bool IsRoundStarted
        {
            get => _isRoundStarted;
            set
            {
                if (_isRoundStarted != value)
                {
                    _isRoundStarted = value;
                    OnPropertyChanged(nameof(IsRoundStarted));
                    OnPropertyChanged(nameof(StartEndButtonText));
                }
            }
        }
        public string RoundDisplayText => string.Format(Translator.Instance["Round"], RoundCounter);
        public string StartEndButtonText =>
            IsRoundStarted ? Translator.Instance["End"] : Translator.Instance["Start"];
        public MainPage()
        {
            InitializeComponent();
            _db = new DatabaseService();
            DeviceDisplay.KeepScreenOn = true;
            var monsters = _db.GetAllMonsters().Select(x => new Enemy(x));
            Enemies = new ObservableCollection<Enemy>(monsters);
            SortEnemies();
            _activeEnemy = Enemies.FirstOrDefault(x => x.IsActive);
            //BindingContext = new EnemiesViewModel();

            BindingContext = this;
            Translator.Instance.PropertyChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(StartEndButtonText));
                OnPropertyChanged(nameof(RoundDisplayText));
            };
        }

        private async void AddEnemyClicked(object sender, EventArgs e)
        {
            var typeMap = new Dictionary<string, CreatureType>
            {
                { "Player", CreatureType.Player },
                { "Monster", CreatureType.Monster },
                { "NPC", CreatureType.NPC }
            };
            
            string selected = await DisplayActionSheet(Translator.Instance["ChooseCreatureType"],
                                                       Translator.Instance["Cancel"],
                                                       null,
                                                       typeMap.Keys.ToArray());

            if (string.IsNullOrWhiteSpace(selected) || selected == Translator.Instance["Cancel"] || !typeMap.ContainsKey(selected))
                return;

            CreatureType selectedType = typeMap[selected];

            var newMonsterName = await DisplayPromptAsync(Translator.Instance["AddACreature"],
                                                          Translator.Instance["NameTheCreature"],
                                                          Translator.Instance["Save"],
                                                          Translator.Instance["Cancel"]);

            if (string.IsNullOrWhiteSpace(newMonsterName)) return;

            var entity = _db.AddMonster(newMonsterName);
            entity.CreatureType = selectedType;
            _db.UpdateMonster(entity);
            var newEnemy = new Enemy(entity);
            Enemies.Add(newEnemy);

            if (Enemies.Count == 1)
                SetActiveEnemy(newEnemy);
        }

        private async void IncreaseHP_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            string result = await DisplayPromptAsync(Translator.Instance["IncreaseTheHP"],
                                                     Translator.Instance["HowMuchToIncreaseTheHP"],
                                                     keyboard: Keyboard.Numeric);
            if (!int.TryParse(result, out int hp)) return;

            var enemy = button?.BindingContext as Enemy;
            if (enemy != null)
            {
                enemy.IncreaseHitPoints(hp);
                _db.UpdateMonster(new EnemyEntity(enemy));
            }
        }

        private async void DecreaseHP_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            string result = await DisplayPromptAsync(Translator.Instance["ReduceHP"],
                                                     Translator.Instance["HowMuchToReduceTheHP"],
                                                     "OK",
                                                     Translator.Instance["Cancel"],
                                                     keyboard: Keyboard.Numeric);
            if (!int.TryParse(result, out int hp)) return;

            var enemy = button?.BindingContext as Enemy;
            if (enemy != null)
            {
                enemy?.DecreaseHitPoints(hp);
                _db.UpdateMonster(new EnemyEntity(enemy));
            }
        }

        private async void HPLabel_Clicked(object sender, EventArgs e)
        {
            var button = sender as Label;
            var enemy = button?.BindingContext as Enemy;
            if (enemy == null) return;

            string action = await DisplayActionSheet(Translator.Instance["ChangeHP"], Translator.Instance["Cancel"], null,
                                             Translator.Instance["ChangeRegularHP"], Translator.Instance["ChangeTempHP"]);

            if (action == Translator.Instance["ChangeRegularHP"])
            {
                string result = await DisplayPromptAsync(Translator.Instance["ChangeHP"],
                                                         Translator.Instance["HowMuchToChangeHP"],
                                                         "OK", Translator.Instance["Cancel"],
                                                         keyboard: Keyboard.Numeric);

                if (int.TryParse(result, out int hp))
                {
                    enemy.UpdateHitPoints(hp);
                    _db.UpdateMonster(new EnemyEntity(enemy));
                }
            }
            else if (action == Translator.Instance["ChangeTempHP"])
            {
                string result = await DisplayPromptAsync(Translator.Instance["ChangeTempHP"],
                                                         Translator.Instance["HowMuchToChangeTempHP"],
                                                         "OK", Translator.Instance["Cancel"],
                                                         keyboard: Keyboard.Numeric);

                if (int.TryParse(result, out int tempHp))
                {
                    enemy.TempHitPoints = tempHp;
                    _db.UpdateMonster(new EnemyEntity(enemy));
                }
            }
        }

        private async void NameLabel_Clicked(object sender, TappedEventArgs e)
        {
            var button = sender as Label;
            var enemy = button?.BindingContext as Enemy;
            if (enemy == null) return;
            string result = await DisplayPromptAsync(Translator.Instance["ChangeName"],
                                                     Translator.Instance["WhatNameWillYouChange"],
                                                     "OK",
                                                     Translator.Instance["Cancel"],
                                                     initialValue: enemy.Name);
            if (result != null)
            { 
                enemy.Name = result;
                _db.UpdateMonster(new EnemyEntity(enemy));
            }
        }

        private async void RemoveEnemy_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var enemy = button?.BindingContext as Enemy;
            var result = await DisplayAlert(Translator.Instance["Removal"],
                                            Translator.Instance["RemoveTheMonster"],
                                            Translator.Instance["Delete"],
                                            Translator.Instance["Cancel"]);
            if (result && enemy != null)
            {
                int index = Enemies.IndexOf(enemy);
                Enemies.Remove(enemy);
                _db.DeleteMonster(new EnemyEntity(enemy));

                if (Enemies.Count == 0)
                    SetActiveEnemy(null);
                else if (_activeEnemy == enemy)
                {
                    int nextIndex = Math.Min(index, Enemies.Count - 1);
                    Enemy localEnemy = Enemies[nextIndex];
                    SetActiveEnemy(localEnemy);
                    _db.UpdateMonster(new EnemyEntity(enemy));
                }
            }
        }

        private async void InitiativeLabel_Clicked(object sender, EventArgs e)
        {
            var label = sender as Label;
            var enemy = label?.BindingContext as Enemy;
            if (enemy == null) return;

            string result = await DisplayPromptAsync(Translator.Instance["ChangeOfInitiative"],
                                                     Translator.Instance["EnterNewInitiative"],
                                                     "OK",
                                                     Translator.Instance["Cancel"],
                                                     keyboard: Keyboard.Numeric);
            if (int.TryParse(result, out int newInitiative))
            {
                enemy.Initiative = newInitiative;
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
                {
                    Enemies.Move(Enemies.IndexOf(sorted[i]), i);
                }
            }
        }

        private void SetActiveEnemy(Enemy? enemy)
        {
            foreach (var e in Enemies)
                e.IsActive = false;

            if (_activeEnemy != null)
                _db.UpdateMonster(new EnemyEntity(_activeEnemy));

            _activeEnemy = enemy;
            if (enemy != null)
                enemy.IsActive = true;

            if (_activeEnemy != null)
                _db.UpdateMonster(new EnemyEntity(_activeEnemy));

        }

        private void PreviousEnemy_Clicked(object sender, EventArgs e)
        {
            if (_activeEnemy == null || Enemies.Count == 0) return;

            var sorted = Enemies.OrderByDescending(en => en.Initiative).ToList();
            int index = sorted.IndexOf(_activeEnemy);
            int previousIndex = (index - 1 + sorted.Count) % sorted.Count;

            if (_isRoundStarted && index == 0 && previousIndex == sorted.Count - 1)
            {
                if (RoundCounter > 1)
                {
                    RoundCounter--;
                    OnPropertyChanged(nameof(StartEndButtonText));
                }
            }
            SetActiveEnemy(sorted[previousIndex]);
        }

        private void NextEnemy_Clicked(object sender, EventArgs e)
        {
            if (_activeEnemy == null || Enemies.Count == 0) return;

            var sorted = Enemies.OrderByDescending(en => en.Initiative).ToList();
            int index = sorted.IndexOf(_activeEnemy);
            int nextIndex = (index + 1) % sorted.Count;

            if (_isRoundStarted && index == sorted.Count - 1 && nextIndex == 0)
            {
                RoundCounter++;
                OnPropertyChanged(nameof(StartEndButtonText));
            }
            SetActiveEnemy(sorted[nextIndex]);
        }

        private void StartRound_Clicked(object sender, EventArgs e)
        {
            RoundCounter++;
            if (Enemies.Count > 0)
                SetActiveEnemy(Enemies.OrderByDescending(e => e.Initiative).First());
        }

        private void StartEndButton_Clicked(object sender, EventArgs e)
        {
            if (!_isRoundStarted)
            {
                _isRoundStarted = true;
                RoundCounter = 1;
                RoundCounterBorder.IsVisible = true;

                var sorted = Enemies.OrderByDescending(e => e.Initiative).ToList();
                if (sorted.Count > 0)
                    SetActiveEnemy(sorted[0]);
            }
            else
            {
                _isRoundStarted = false;
                RoundCounterBorder.IsVisible = false;
            }
            OnPropertyChanged(nameof(StartEndButtonText));
        }

        private async void ArmorClassLabel_Clicked(object sender, EventArgs e)
        {
            var label = sender as Label;
            var enemy = label?.BindingContext as Enemy;
            
            string result = await DisplayPromptAsync(Translator.Instance["ArmorClass"],
                                                     Translator.Instance["EnterNewAC"],
                                                     "OK",
                                                     Translator.Instance["Cancel"],
                                                     maxLength: 2,
                                                     keyboard: Keyboard.Numeric);
            if (!int.TryParse(result, out int newAC)) return;

            if (enemy != null)
            {
                enemy.ArmorClass = newAC;
                _db.UpdateMonster(new EnemyEntity(enemy));
            }

        }

        private async void CreatureTypeLabel_Clicked(object sender, EventArgs e)
        {
            var label = sender as Label;
            var enemy = label?.BindingContext as Enemy;
            if (enemy == null) return;

            string selected = await DisplayActionSheet(Translator.Instance["ChangeCreatureType"],
                                                       Translator.Instance["Cancel"],
                                                       null,
                                                       "Player",
                                                       "Monster",
                                                       "NPC");

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
    }
}
