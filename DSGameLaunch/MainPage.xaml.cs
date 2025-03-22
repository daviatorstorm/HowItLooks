using DSGameLaunch.Models;
using System.Collections.ObjectModel;

namespace DSGameLaunch
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Enemy> Enemies { get; set; }

        public MainPage()
        {
            InitializeComponent();
            //BindingContext = new EnemiesViewModel();
            Enemies = new ObservableCollection<Enemy>
            {
                new Enemy("Монстер 1",  10)
            };

            BindingContext = this;
        }

        private async void AddEnemyClicked(object sender, EventArgs e)
        {
            //Navigation.PushModalAsync(new AddEnemyModal());
            var newMonsterName = await DisplayPromptAsync("Додати монстра", "Назвіть монстра:", "Зберегти", "Відмінити");

            Enemies.Add(new Enemy(newMonsterName, 0));
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
            enemy.UpdateHitPoints(hp);
        }

        private async void NameLabel_Clicked(object sender, TappedEventArgs e)
        {
            var button = sender as Label;
            var enemy = button?.BindingContext as Enemy;
            string result = await DisplayPromptAsync("Змінити НР", "На скільки змінити НР?", initialValue: enemy.Name);
            enemy.Name = result;
        }

        private async void RemoveEnemy_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var enemy = button?.BindingContext as Enemy;
            var result = await DisplayAlert("Видалення", "Ви дійсно бажаєте видалити монстра?", "Видалити", "Відмінити");
            if (result)
                Enemies.Remove(enemy);
        }

        //private void OnCounterClicked(object sender, EventArgs e)
        //{
        //    count++;

        //    if (count == 1)
        //        CounterBtn.Text = $"Clicked {count} time";
        //    else
        //        CounterBtn.Text = $"Clicked {count} times";
        //}
    }
}
