namespace DSGameLaunch;

public partial class AddEnemyModal : ContentPage
{
	public AddEnemyModal()
	{
		InitializeComponent();
	}

    private async void OnCloseButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(); // Close the modal page
    }
}