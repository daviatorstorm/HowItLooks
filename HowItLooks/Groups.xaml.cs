using HowItLooks.Services;
using HowItLooks.ViewModels;

namespace HowItLooks 
{ 
    public partial class Groups : ContentPage
    {
        public Groups(DatabaseService db)
        {
            InitializeComponent();
            BindingContext = new GroupsViewModel(db);
        }
    }
}