using DSGameLaunch.Models;
using System.Collections.ObjectModel;

namespace DSGameLaunch.ViewModels;

public class EnemiesViewModel
{
    public ObservableCollection<Enemy> Enemies { get; set; }

    public EnemiesViewModel()
    {
        //Enemies = new ObservableCollection<Enemy>
        //{
        //    new Enemy { Name = "Item 1", Description = "This is the first item." },
        //    new Enemy { Name = "Item 2", Description = "This is the second item." },
        //    new Enemy { Name = "Item 3", Description = "This is the third item." }
        //};
    }
}
