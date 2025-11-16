using FmgLib.MauiMarkup;
using MoneyCenter.Views.Extensions;
using MoneyCenter.Views.Icons;
using System.Runtime.CompilerServices;

namespace MoneyCenter.Views.Controls
{
    public class AccordionControl : Border
    {
        #region Bindable Properties

        public static readonly BindableProperty HeaderProperty = BindableProperty.Create(
            nameof(Header),
            typeof(View),
            typeof(AccordionControl)
        );

        public View Header
        {
            get => (View)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly BindableProperty AccordionContentProperty = BindableProperty.Create(
            nameof(AccordionContent),
            typeof(View),
            typeof(AccordionControl)
        );

        public View AccordionContent
        {
            get => (View)GetValue(AccordionContentProperty);
            set => SetValue(AccordionContentProperty, value);
        }

        public static readonly BindableProperty IsCollapsedProperty = BindableProperty.Create(
            nameof(IsCollapsed),
            typeof(bool),
            typeof(AccordionControl),
            true
        );

        public bool IsCollapsed
        {
            get => (bool)GetValue(IsCollapsedProperty);
            set => SetValue(IsCollapsedProperty, value);
        }

        public static readonly BindableProperty IconSizeProperty = BindableProperty.Create(
            nameof(IconSize),
            typeof(int),
            typeof(AccordionControl),
            48
        );

        public int IconSize
        {
            get => (int)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        #endregion Bindable Properties

        #region UI Definitions

        private readonly VerticalStackLayout _ContentLayout = new VerticalStackLayout().Spacing(0);
        private readonly Grid _Header = new();
        private readonly Image _Chevron = new Image().Margin(8);

        //private readonly BoxView _Divider = new BoxView().MakeDivider(Colors.DarkGray);
        private readonly TapGestureRecognizer _CollapseTap = new TapGestureRecognizer().NumberOfTapsRequired(1);

        #endregion UI Definitions

        #region Constructor

        public AccordionControl()
        {
            this
                .Content(_ContentLayout)
                .Margin(0);
            _Header
                .ColumnDefinitions(defs => defs.Star().Absolute(IconSize))
                .GestureRecognizers(_CollapseTap);
            _CollapseTap.Tapped += OnCollapseTap;
        }

        #endregion Constructor

        #region Methods

        private async void OnCollapseTap(object? sender, EventArgs e)
        {
            //await _Header.BackgroundColorTo(Colors.Green.WithAlpha(0.25f), 10);
            //_ = _Header.BackgroundColorTo(Colors.Transparent, 10);
            IsCollapsed = !IsCollapsed;
        }

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == HeaderProperty.PropertyName ||
                propertyName == AccordionContentProperty.PropertyName)
            {
                _Header.Clear();
                _Header.Add(Header.Column(0));
                _Header.Add(_Chevron.Center().Column(1));

                _ContentLayout.Clear();
                _ContentLayout.Add(_Header);
                //_ContentLayout.Add(_Divider);
                _ContentLayout.Add(AccordionContent);
                ApplyCollapseLogic();
            }
            else if (propertyName == IsCollapsedProperty.PropertyName)
            {
                ApplyCollapseLogic();
            }
            else if (propertyName == StrokeProperty.PropertyName && Stroke is SolidColorBrush solidBrush)
            {
                //_Divider.MakeDivider(solidBrush.Color);
                _Chevron.ApplyMaterialIcon(
                    IsCollapsed ? MaterialIcon.Keyboard_arrow_down : MaterialIcon.Keyboard_arrow_up,
                    solidBrush.Color,
                    IconSize);
            }
            else if (propertyName == IconSizeProperty.PropertyName)
            {
                var stroke = Colors.DarkGray;
                if (Stroke is SolidColorBrush solidColorBrush)
                    stroke = solidColorBrush.Color;
                _Header.ColumnDefinitions.Clear();
                _Header.ColumnDefinitions(defs => defs.Star().Absolute(IconSize));
                _Chevron.ApplyMaterialIcon(
                    IsCollapsed ? MaterialIcon.Keyboard_arrow_down : MaterialIcon.Keyboard_arrow_up,
                    stroke,
                    IconSize).Column(1).Center();
            }
        }

        private async void ApplyCollapseLogic()
        {
            Color stroke = Colors.DarkGray;
            if (Stroke is SolidColorBrush solidBrush)
                stroke = solidBrush.Color;

            // Set the Chevron icon depending on the collapsed state.
            _Chevron.ApplyMaterialIcon(
                IsCollapsed ? MaterialIcon.Keyboard_arrow_down : MaterialIcon.Keyboard_arrow_up,
                stroke,
                IconSize
            );

            //_Divider.IsVisible = !IsCollapsed;

            if (AccordionContent != null)
            {
                // Animate the height change smoothly.
                if (IsCollapsed)
                {
                    // Collapse the content with animation.
                    await AccordionContent.AnimateHeightAsync(0, 250); // 250ms duration
                }
                else
                {
                    // Expand the content with animation to its original height.
                    AccordionContent.IsVisible = true; // Make sure it's visible before expanding.
                    await AccordionContent.AnimateHeightAsync(-1, 250); // Auto height with animation.
                }
            }
        }

        #endregion Methods
    }

    public static class ViewExtensions
    {
        public static Task AnimateHeightAsync(this View view, double toHeight, uint duration)
        {
            double fromHeight = view.Height;

            // Ensure the view is laid out correctly to get the initial height.
            if (fromHeight == -1) fromHeight = view.Measure(double.PositiveInfinity, double.PositiveInfinity).Request.Height;

            var animation = new Animation(v => view.HeightRequest = v, fromHeight, toHeight);
            var tcs = new TaskCompletionSource<bool>();

            // Commit the animation with the provided duration.
            animation.Commit(view, "HeightAnimation", 16, duration, Easing.Linear, (v, c) => tcs.SetResult(true));

            return tcs.Task;
        }
    }
}