namespace HowItLooks;
using HowItLooks.Services;
using HowItLooks.ViewModels;

public partial class Campaigns : ContentPage
{
	public Campaigns(DatabaseService db)
	{
		InitializeComponent();
        BindingContext = new CampaignsViewModel(db);
    }
}