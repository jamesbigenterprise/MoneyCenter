namespace MoneyCenter.ViewModel.Objects
{
    public enum CategoryKind
    {
        Expense,
        Income,
        Savings
    }

    public static class CategoryKindExtensions
    {
        public static string ToStorageValue(this CategoryKind kind)
        {
            switch (kind)
            {
                case CategoryKind.Income:
                    return "income";
                case CategoryKind.Savings:
                    return "savings";
                default:
                    return "expense";
            }
        }

        public static CategoryKind FromStorageValue(string? value)
        {
            var normalized = (value ?? string.Empty).Trim().ToLowerInvariant();
            switch (normalized)
            {
                case "income":
                    return CategoryKind.Income;
                case "savings":
                    return CategoryKind.Savings;
                default:
                    return CategoryKind.Expense;
            }
        }
    }
}
