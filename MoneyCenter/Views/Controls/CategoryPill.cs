using MoneyCenter.ViewModel.Objects;

namespace MoneyCenter.Views.Controls;

public class CategoryPill : Border
{
    private readonly Label _label = new();

    public static readonly BindableProperty TypeProperty = BindableProperty.Create(
        nameof(Type),
        typeof(string),
        typeof(CategoryPill),
        CategoryKind.Expense.ToStorageValue(),
        propertyChanged: (bindable, _, _) => ((CategoryPill)bindable).ApplyType());

    public string Type
    {
        get => (string)GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    public CategoryPill()
    {
        StrokeThickness = 1;
        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 20 };
        Padding = new Thickness(10, 2);
        HorizontalOptions = LayoutOptions.Start;
        VerticalOptions = LayoutOptions.Center;

        _label.FontSize = 12;
        _label.FontAttributes = FontAttributes.Bold;
        Content = _label;

        ApplyType();
    }

    private void ApplyType()
    {
        BackgroundColor = CategoryKindMetadata.GetFillColor(Type);
        Stroke = CategoryKindMetadata.GetStrokeColor(Type);
        _label.Text = CategoryKindMetadata.GetDisplayText(Type);
        _label.TextColor = CategoryKindMetadata.GetTextColor(Type);
    }
}
