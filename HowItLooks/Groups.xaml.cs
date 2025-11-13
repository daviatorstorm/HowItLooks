using HowItLooks.Entities;
using HowItLooks.Extension;
using HowItLooks.Models;
using HowItLooks.Services;
using HowItLooks.ViewModels;
using System.Collections.ObjectModel;

namespace HowItLooks 
{ 
    public partial class Groups : ContentPage
    {
        public Groups()
        {
            InitializeComponent();
            var db = new DatabaseService();
            BindingContext = new GroupsViewModel(db);
        }
    }
}