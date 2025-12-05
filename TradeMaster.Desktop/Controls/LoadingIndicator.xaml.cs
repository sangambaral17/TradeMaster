using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TradeMaster.Desktop.Controls;

/// <summary>
/// A reusable loading indicator control with spinning animation.
/// Supports customizable size, color, and overlay mode.
/// </summary>
public partial class LoadingIndicator : UserControl
{
    private Storyboard? _spinAnimation;

    public LoadingIndicator()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    #region Dependency Properties

    /// <summary>
    /// Gets or sets the size of the spinner (width and height).
    /// </summary>
    public static readonly DependencyProperty SpinnerSizeProperty =
        DependencyProperty.Register(
            nameof(SpinnerSize),
            typeof(double),
            typeof(LoadingIndicator),
            new PropertyMetadata(50.0));

    public double SpinnerSize
    {
        get => (double)GetValue(SpinnerSizeProperty);
        set => SetValue(SpinnerSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the spinner.
    /// </summary>
    public static readonly DependencyProperty SpinnerColorProperty =
        DependencyProperty.Register(
            nameof(SpinnerColor),
            typeof(Brush),
            typeof(LoadingIndicator),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(52, 152, 219)))); // Default blue

    public Brush SpinnerColor
    {
        get => (Brush)GetValue(SpinnerColorProperty);
        set => SetValue(SpinnerColorProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the overlay background (for blocking mode).
    /// </summary>
    public static readonly DependencyProperty IsOverlayModeProperty =
        DependencyProperty.Register(
            nameof(IsOverlayMode),
            typeof(bool),
            typeof(LoadingIndicator),
            new PropertyMetadata(false, OnIsOverlayModeChanged));

    public bool IsOverlayMode
    {
        get => (bool)GetValue(IsOverlayModeProperty);
        set => SetValue(IsOverlayModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the loading message to display.
    /// </summary>
    public static readonly DependencyProperty LoadingMessageProperty =
        DependencyProperty.Register(
            nameof(LoadingMessage),
            typeof(string),
            typeof(LoadingIndicator),
            new PropertyMetadata("Loading..."));

    public string LoadingMessage
    {
        get => (string)GetValue(LoadingMessageProperty);
        set => SetValue(LoadingMessageProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the loading message.
    /// </summary>
    public static readonly DependencyProperty ShowMessageProperty =
        DependencyProperty.Register(
            nameof(ShowMessage),
            typeof(bool),
            typeof(LoadingIndicator),
            new PropertyMetadata(true));

    public bool ShowMessage
    {
        get => (bool)GetValue(ShowMessageProperty);
        set => SetValue(ShowMessageProperty, value);
    }

    #endregion

    private static void OnIsOverlayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LoadingIndicator indicator)
        {
            indicator.OverlayBackground.Visibility = (bool)e.NewValue 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Start the spinning animation
        _spinAnimation = (Storyboard)Resources["SpinAnimation"];
        _spinAnimation?.Begin();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        // Stop the animation to prevent memory leaks
        _spinAnimation?.Stop();
    }
}
