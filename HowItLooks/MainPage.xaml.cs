using HowItLooks.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace HowItLooks
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Enemy> Enemies { get; set; }
        private Enemy? _activeEnemy;

        public MainPage()
        {
            InitializeComponent();
            //BindingContext = new EnemiesViewModel();
            Enemies = new ObservableCollection<Enemy>
            {
                new Enemy("Монстер 1",  10)
            };
            SetActiveEnemy(Enemies[0]);

            BindingContext = this;
        }

        private async void AddEnemyClicked(object sender, EventArgs e)
        {
            //Navigation.PushModalAsync(new AddEnemyModal());
            var newMonsterName = await DisplayPromptAsync("Додати монстра", "Назвіть монстра:", "Зберегти", "Відмінити");

            if (string.IsNullOrWhiteSpace(newMonsterName)) return;

            var newEnemy = new Enemy(newMonsterName, 0);
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
            enemy?.IncreaseHitPoints(hp);
        }

        private async void DecreaseHP_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            string result = await DisplayPromptAsync("Зменшити НР", "На скільки зменшити НР?", keyboard: Keyboard.Numeric);
            if (!int.TryParse(result, out int hp)) return;

            var enemy = button?.BindingContext as Enemy;
            enemy?.DecreaseHitPoints(hp);
        }

        private async void HPLabel_Clicked(object sender, EventArgs e)
        {
            var button = sender as Label;
            var enemy = button?.BindingContext as Enemy;
            string result = await DisplayPromptAsync("Змінити НР", "На скільки змінити НР?", keyboard: Keyboard.Numeric);
            if (!int.TryParse(result, out int hp)) return;
            if (enemy != null) enemy.UpdateHitPoints(hp);
        }

        private async void NameLabel_Clicked(object sender, TappedEventArgs e)
        {
            var button = sender as Label;
            var enemy = button?.BindingContext as Enemy;
            if (enemy == null) return;
            string result = await DisplayPromptAsync("Змінити НР", "На скільки змінити НР?", initialValue: enemy.Name);
            enemy.Name = result;
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

                if (Enemies.Count == 0)
                    SetActiveEnemy(null);
                else if (_activeEnemy == enemy)
                {
                    int nextIndex = Math.Min(index, Enemies.Count - 1);
                    SetActiveEnemy(Enemies[nextIndex]);
                }
            }
        }

        private async void InitiativeLabel_Clicked(object sender, EventArgs e)
        {
            var label = sender as Label;
            var enemy = label?.BindingContext as Enemy;
            if (enemy == null) return;

            string result = await DisplayPromptAsync("Зміна ініціативи", "Вкажіть нове число ініціативи:", keyboard: Keyboard.Numeric, initialValue: enemy.Initiative.ToString());
            if (int.TryParse(result, out int newInitiative))
            {
                enemy.Initiative = newInitiative;
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

            _activeEnemy = enemy;
            if (enemy != null)
                enemy.IsActive = true;
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
