using MoneyCenter.ViewModel;
using MoneyCenter.Views.Icons;

namespace MoneyCenter.Views.Controls;

public class AddBudgetCategoryForm : ContentView
{
    public static readonly BindableProperty IsDesktopProperty = BindableProperty.Create(
        nameof(IsDesktop),
        typeof(bool),
        typeof(AddBudgetCategoryForm),
        true,
        propertyChanged: (bindable, _, _) => ((AddBudgetCategoryForm)bindable).BuildContent());

    public bool IsDesktop
    {
        get => (bool)GetValue(IsDesktopProperty);
        set => SetValue(IsDesktopProperty, value);
    }

    public AddBudgetCategoryForm()
    {
        BuildContent();
    }

    private void BuildContent()
    {
        Content = IsDesktop ? BuildDesktopForm() : BuildMobileForm();
    }

    private View BuildDesktopForm()
    {
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(2, GridUnitType.Star)),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(1.5, GridUnitType.Star)),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12
        };

        grid.Add(WrapInput(CreateCategoryPicker()), 0);
        grid.Add(WrapInput(CreateAmountEntry(), new Thickness(8, 0)), 1);
        grid.Add(CreateRecurringEditor(showLabel: true), 2);
        grid.Add(CreateActions(), 3);

        return grid;
    }

    private View BuildMobileForm()
    {
        var amountAndFrequency = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 10
        };
        amountAndFrequency.Add(WrapInput(CreateAmountEntry(), new Thickness(8, 0)), 0);
        amountAndFrequency.Add(CreateRecurringEditor(showLabel: false), 1);

        return new VerticalStackLayout
        {
            Spacing = 10,
            Children =
            {
                WrapInput(CreateCategoryPicker()),
                amountAndFrequency,
                CreateActions(LayoutOptions.End)
            }
        };
    }

    private static Picker CreateCategoryPicker()
    {
        var picker = new Picker();
        picker.SetBinding(Picker.ItemsSourceProperty, nameof(BudgetWrapper.AvailableCategories));
        picker.SetBinding(Picker.SelectedItemProperty, nameof(BudgetWrapper.SelectedCategory), mode: BindingMode.TwoWay);
        picker.ItemDisplayBinding = new Binding("Name");
        return picker;
    }

    private static Entry CreateAmountEntry()
    {
        var entry = new Entry
        {
            Keyboard = Keyboard.Numeric,
            HorizontalTextAlignment = TextAlignment.End
        };
        entry.SetBinding(Entry.TextProperty, nameof(BudgetWrapper.NewCategoryAmount), mode: BindingMode.TwoWay);
        return entry;
    }

    private static View CreateRecurringEditor(bool showLabel)
    {
        var layout = new HorizontalStackLayout
        {
            Spacing = 8,
            VerticalOptions = LayoutOptions.Center
        };

        var recurringSwitch = new Switch();
        recurringSwitch.SetBinding(Switch.IsToggledProperty, nameof(BudgetWrapper.NewCategoryIsRecurring), mode: BindingMode.TwoWay);
        layout.Children.Add(recurringSwitch);

        if (showLabel)
        {
            layout.Children.Add(new Label
            {
                Text = "Recurring",
                VerticalOptions = LayoutOptions.Center
            });
        }

        var dayEntry = new Entry
        {
            Keyboard = Keyboard.Numeric,
            HorizontalTextAlignment = TextAlignment.Center
        };
        dayEntry.SetBinding(Entry.TextProperty, nameof(BudgetWrapper.NewCategoryDayOfMonth), mode: BindingMode.TwoWay);
        var dayInput = WrapInput(dayEntry, new Thickness(8, 0), 64);
        dayInput.SetBinding(IsVisibleProperty, nameof(BudgetWrapper.NewCategoryIsRecurring));
        layout.Children.Add(dayInput);

        return layout;
    }

    private static HorizontalStackLayout CreateActions()
    {
        return CreateActions(LayoutOptions.Start);
    }

    private static HorizontalStackLayout CreateActions(LayoutOptions horizontalOptions)
    {
        var cancel = new Button
        {
            Text = MaterialIcon.Close,
            Style = (Style)Application.Current!.Resources["MaterialIconOutlineButtonStyle"]
        };
        cancel.SetBinding(Button.CommandProperty, nameof(BudgetWrapper.CancelAddCategoryCommand));

        var confirm = new Button
        {
            Text = MaterialIcon.Check,
            Style = (Style)Application.Current!.Resources["MaterialIconPrimaryButtonStyle"]
        };
        confirm.SetBinding(Button.CommandProperty, nameof(BudgetWrapper.ConfirmAddCategoryCommand));

        return new HorizontalStackLayout
        {
            Spacing = 8,
            HorizontalOptions = horizontalOptions,
            Children = { cancel, confirm }
        };
    }

    private static Border WrapInput(View content)
    {
        return WrapInput(content, new Thickness(10, 0));
    }

    private static Border WrapInput(View content, Thickness padding, double width = -1)
    {
        var border = new Border
        {
            Style = (Style)Application.Current!.Resources["InputBorderStyle"],
            Padding = padding,
            Content = content
        };

        if (width > 0)
        {
            border.WidthRequest = width;
        }

        return border;
    }
}
