using Microsoft.Maui.Graphics;

namespace MoneyCenter.ViewModel.Objects;

public static class CategoryKindMetadata
{
    public static IReadOnlyList<string> StorageValues { get; } =
    [
        CategoryKind.Expense.ToStorageValue(),
        CategoryKind.Income.ToStorageValue(),
        CategoryKind.Savings.ToStorageValue()
    ];

    public static string GetDisplayText(string? storageValue)
    {
        return CategoryKindExtensions.FromStorageValue(storageValue).ToStorageValue();
    }

    public static Color GetFillColor(string? storageValue)
    {
        switch (CategoryKindExtensions.FromStorageValue(storageValue))
        {
            case CategoryKind.Income:
                return Color.FromArgb("#5B8C5A");
            case CategoryKind.Savings:
                return Color.FromArgb("#EFE6D8");
            default:
                return Color.FromArgb("#EF4444");
        }
    }

    public static Color GetStrokeColor(string? storageValue)
    {
        switch (CategoryKindExtensions.FromStorageValue(storageValue))
        {
            case CategoryKind.Income:
                return Color.FromArgb("#5B8C5A");
            case CategoryKind.Savings:
                return Color.FromArgb("#A8B99E");
            default:
                return Color.FromArgb("#EF4444");
        }
    }

    public static Color GetTextColor(string? storageValue)
    {
        switch (CategoryKindExtensions.FromStorageValue(storageValue))
        {
            case CategoryKind.Income:
            case CategoryKind.Expense:
                return Colors.White;
            default:
                return Color.FromArgb("#5A4D39");
        }
    }
}
