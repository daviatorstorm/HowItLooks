using HowItLooks.Entities;
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
        private readonly EnemyActionsService _enemyActions;
        private ActionLogService _logService;

        private bool _isRoundStarted = false;
        private int _roundCounter = 1;
        private DateTime _battleStartTime;

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
            var monsters = _db.GetAllMonsters()
                                .Where(x => x.GroupId == null)
                                .Select(x => new Enemy(x));
            Enemies = new ObservableCollection<Enemy>(monsters);
            SortEnemies();
            _activeEnemy = Enemies.FirstOrDefault(x => x.IsActive);
            //BindingContext = new EnemiesViewModel();
            _logService = new ActionLogService(ActionLogDrawer, Overlay, ActionLogContainer);
            _enemyActions = new EnemyActionsService(_logService);

            BindingContext = this;
            Translator.Instance.PropertyChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(StartEndButtonText));
                OnPropertyChanged(nameof(RoundDisplayText));
            };
        }

        private async void AddEnemyClicked(object sender, EventArgs e)
        {
            await _enemyActions.AddEnemy(
                  page: this,
                  enemies: Enemies,
                  getActiveEnemy: () => _activeEnemy,
                  setActiveEnemy: SetActiveEnemy
           );
        }

        private async void IncreaseHP_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var enemy = button?.BindingContext as Enemy;
            if (enemy == null) return;

            await _enemyActions.IncreaseHP(this, enemy);
        }

        private async void DecreaseHP_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var enemy = button?.BindingContext as Enemy;

            if (enemy == null) return;
            await _enemyActions.DecreaseHP(this, enemy);
        }

        private async void HPLabel_Clicked(object sender, EventArgs e)
        {
            var button = sender as Label;
            var enemy = button?.BindingContext as Enemy;
            if (enemy == null) return;

            await _enemyActions.ChangeHP(this, enemy);
        }

        private async void NameLabel_Clicked(object sender, TappedEventArgs e)
        {
            var button = sender as Label;
            var enemy = button?.BindingContext as Enemy;
            if (enemy == null) return;

            await _enemyActions.ChangeName(this, enemy);
        }

        private async void RemoveEnemy_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var enemy = button?.BindingContext as Enemy;
            if (enemy == null) return;

            await _enemyActions.RemoveEnemy(page: this,
                                            enemy: enemy,
                                            enemies: Enemies,
                                            getActiveEnemy: () => _activeEnemy,
                                            setActiveEnemy: SetActiveEnemy);
        }

        private async void InitiativeLabel_Clicked(object sender, EventArgs e)
        {
            var label = sender as Label;
            var enemy = label?.BindingContext as Enemy;
            if (enemy == null) return;

            await _enemyActions.ChangeInitiative(this, enemy, SortEnemies);
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
                _logService.LogAction($"🔁 Початок нового раунду #{RoundCounter}");
                OnPropertyChanged(nameof(StartEndButtonText));
            }
            SetActiveEnemy(sorted[nextIndex]);

            if (_isRoundStarted)
                _logService.LogAction($"Хід: {_activeEnemy.Name}", _activeEnemy.CreatureType.ToString());
            //RenderActionLog();
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

                _logService.SetRoundState(true);
                _battleStartTime = DateTime.Now;
                _logService.LogAction($"🔁 Початок нового раунду #{RoundCounter}");

                var sorted = Enemies.OrderByDescending(e => e.Initiative).ToList();
                if (sorted.Count > 0)
                    SetActiveEnemy(sorted[0]);

                _logService.LogAction($"Хід: {_activeEnemy?.Name}", _activeEnemy.CreatureType.ToString());
            }
            else
            {
                _isRoundStarted = false;
                RoundCounterBorder.IsVisible = false;

                _logService.LogAction("🔴 Бій завершено!");
                _logService.SetRoundState(false);
                ShowBattleSummary();
            }
            OnPropertyChanged(nameof(StartEndButtonText));
        }

        private async void ArmorClassLabel_Clicked(object sender, EventArgs e)
        {
            var label = sender as Label;
            var enemy = label?.BindingContext as Enemy;
            if (enemy == null) return;

            await _enemyActions.ChangeArmor(this, enemy);

        }

        private async void CreatureTypeLabel_Clicked(object sender, EventArgs e)
        {
            var label = sender as Label;
            var enemy = label?.BindingContext as Enemy;
            if (enemy == null) return;

            await _enemyActions.ChangeCreatureType(this, enemy);
        }

        private async void ActionLogButton_Clicked(object sender, EventArgs e)
        {
            await _logService.ToggleAsync();
        }
        private void ClearLog_Clicked(object sender, EventArgs e)
        {
            _logService.ClearLog();
        }

        private async void ShowBattleSummary()
        {
            if (_logService != null)
                await _logService.ShowBattleSummary(this, RoundCounter, _battleStartTime);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            System.Diagnostics.Debug.WriteLine("🔵 OnAppearing called - GroupDetailsPage");

            MessagingCenter.Subscribe<GroupDetailsPage>(this, "EnemiesUpdated", (sender) =>
            {
                RefreshEnemiesFromDatabase();
                SortEnemies();
            });
            MessagingCenter.Subscribe<Groups>(this, "EnemiesUpdated", (sender) =>
            {
                RefreshEnemiesFromDatabase();
                SortEnemies();
            });
        }
        //protected override void OnDisappearing()
        //{
        //    base.OnDisappearing();
        //    //_logger.LogInformation("OnAppearing called");
        //    System.Diagnostics.Debug.WriteLine("🔵 OnDisappearing called - GroupDetailsPage");
        // 
        //    MessagingCenter.Unsubscribe<GroupDetailsPage>(this, "EnemiesUpdated");
        //}

        private void RefreshEnemiesFromDatabase()
        {
            var allEnemies = _db.GetAllMonsters().Where(e => e.GroupId == null);

            var existingIds = Enemies.Select(e => e.Id).ToHashSet();

            foreach (var entity in allEnemies)
            {
                if (!existingIds.Contains(entity.Id) && entity.GroupId == null)
                {
                    Enemies.Add(new Enemy(entity));
                }
            }
        }
    }
}
