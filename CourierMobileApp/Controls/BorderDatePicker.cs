namespace CourierMobileApp.Controls;

class BorderDatePicker : DatePicker
{
    public static BindableProperty CornerRadiusProperty = 
        BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(BorderDatePicker), 0);
    public static BindableProperty BorderWidthProperty = 
        BindableProperty.Create(nameof(BorderWidth), typeof(int), typeof(BorderDatePicker), 0);
    public static BindableProperty PaddingProperty = 
        BindableProperty.Create(nameof(Padding), typeof(Thickness), typeof(BorderDatePicker), new Thickness(0));
    public static BindableProperty BorderColorProperty = 
        BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(BorderDatePicker), Colors.Transparent);
    public static BindableProperty CustomHeightProperty =
            BindableProperty.Create(nameof(CustomHeight), typeof(int), typeof(BorderEntry), 0);


    public int CornerRadius
    {
        get => (int)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }
    public int BorderWidth
    {
        get => (int)GetValue(BorderWidthProperty);
        set => SetValue(BorderWidthProperty, value);
    }

    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }
    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }
    public int CustomHeight
    {
        get => (int)GetValue(CustomHeightProperty);
        set => SetValue(CustomHeightProperty, value);
    }
}
