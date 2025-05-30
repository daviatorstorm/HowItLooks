using HowItLooks.Entities;
using HowItLooks.Models;
using HowItLooks.Services;
using System.Collections.ObjectModel;

namespace HowItLooks
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Enemy> Enemies { get; set; }
        private Enemy? _activeEnemy;
        private readonly DatabaseService _db;

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
        }

        private async void AddEnemyClicked(object sender, EventArgs e)
        {
            //Navigation.PushModalAsync(new AddEnemyModal());
            var newMonsterName = await DisplayPromptAsync("Додати монстра", "Назвіть монстра:", "Зберегти", "Відмінити");

            if (string.IsNullOrWhiteSpace(newMonsterName)) return;

            var entity = _db.AddMonster(newMonsterName);
            var newEnemy = new Enemy(entity);
            Enemies.Add(newEnemy);

            if (Enemies.Count == 1)
                SetActiveEnemy(newEnemy);
        }

        private async void IncreaseHP_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            string result = await DisplayPromptAsync("Збільшити НР", "На скільки збільшити НР?", keyboard: Keyboard.Numeric);
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
            string result = await DisplayPromptAsync("Зменшити НР", "На скільки зменшити НР?", keyboard: Keyboard.Numeric);
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
            string result = await DisplayPromptAsync("Змінити НР", "На скільки змінити НР?", keyboard: Keyboard.Numeric);
            if (!int.TryParse(result, out int hp)) return;
            if (enemy != null)
            {
                enemy.UpdateHitPoints(hp);
                _db.UpdateMonster(new EnemyEntity(enemy));
            }
        }

        private async void NameLabel_Clicked(object sender, TappedEventArgs e)
        {
            var button = sender as Label;
            var enemy = button?.BindingContext as Enemy;
            if (enemy == null) return;
            string result = await DisplayPromptAsync("Змінити НР", "На скільки змінити НР?", initialValue: enemy.Name);
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
            var result = await DisplayAlert("Видалення", "Ви дійсно бажаєте видалити монстра?", "Видалити", "Відмінити");
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

            string result = await DisplayPromptAsync("Зміна ініціативи", "Вкажіть нове число ініціативи:", keyboard: Keyboard.Numeric);
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
            SetActiveEnemy(sorted[previousIndex]);
        }

        private void NextEnemy_Clicked(object sender, EventArgs e)
        {
            if (_activeEnemy == null || Enemies.Count == 0) return;

            var sorted = Enemies.OrderByDescending(en => en.Initiative).ToList();
            int index = sorted.IndexOf(_activeEnemy);
            int nextIndex = (index + 1) % sorted.Count;
            SetActiveEnemy(sorted[nextIndex]);
        }
    }
}
