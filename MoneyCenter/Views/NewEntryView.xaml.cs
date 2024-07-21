#if __ANDROID__
using Android.App;
using Android.Content;
using Android.Views;
using AndroidX.Activity;
using AndroidX.Core.View;
#endif

using MoneyCenter.ViewModel;

namespace MoneyCenter.Views;

public partial class NewEntryView : ContentPage
{
	public NewEntryView(NewEntryViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
        ConfigureScrollViewForKeyboard();


    }
    private void ConfigureScrollViewForKeyboard()
    {
#if __ANDROID__

            // Explicitly specifying type arguments for AppendToMapping
            Microsoft.Maui.Handlers.ScrollViewHandler.Mapper.AppendToMapping("AdjustScrollViewForKeyboard", (handler, view) =>
            {   
                var context = (Context)handler.MauiContext.Context;
                var activity = (Activity)context;
                activity.Window.SetSoftInputMode(SoftInput.AdjustResize);
            });
            
#endif
    }

}