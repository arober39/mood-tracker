using Microsoft.Maui.Controls.Shapes;
using mood_tracker.Models;

namespace mood_tracker.Controls;

// A circular pastel orb that renders an emotive face for a given MoodScore.
// The face is two small dots and a quadratic mouth path whose curvature
// reflects the score (frown for Rough → smile for Great).
public class MoodOrbView : Border
{
    public static readonly BindableProperty ScoreProperty = BindableProperty.Create(
        nameof(Score),
        typeof(MoodScore),
        typeof(MoodOrbView),
        MoodScore.Okay,
        propertyChanged: (b, _, _) => ((MoodOrbView)b).Redraw());

    public static readonly BindableProperty ShowFaceProperty = BindableProperty.Create(
        nameof(ShowFace),
        typeof(bool),
        typeof(MoodOrbView),
        true,
        propertyChanged: (b, _, _) => ((MoodOrbView)b).Redraw());

    public static readonly BindableProperty OrbSizeProperty = BindableProperty.Create(
        nameof(OrbSize),
        typeof(double),
        typeof(MoodOrbView),
        64d,
        propertyChanged: (b, _, _) => ((MoodOrbView)b).Redraw());

    public MoodScore Score
    {
        get => (MoodScore)GetValue(ScoreProperty);
        set => SetValue(ScoreProperty, value);
    }

    public bool ShowFace
    {
        get => (bool)GetValue(ShowFaceProperty);
        set => SetValue(ShowFaceProperty, value);
    }

    public double OrbSize
    {
        get => (double)GetValue(OrbSizeProperty);
        set => SetValue(OrbSizeProperty, value);
    }

    public MoodOrbView()
    {
        StrokeThickness = 0;
        Padding = 0;
        Redraw();
    }

    void Redraw()
    {
        var size = OrbSize;
        WidthRequest = size;
        HeightRequest = size;
        StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(size / 2) };
        BackgroundColor = Score.Color();

        if (!ShowFace || size < 28)
        {
            Content = null;
            return;
        }

        // Lay out the face on a 100×100 grid scaled to the orb's real size.
        var grid = new Grid
        {
            WidthRequest = size,
            HeightRequest = size,
        };

        double eyeSize = size * 0.09;
        double eyeY = size * 0.42;
        double eyeOffsetX = size * 0.18;
        double eyeCx = size / 2;

        grid.Children.Add(Dot(eyeSize, eyeCx - eyeOffsetX, eyeY));
        grid.Children.Add(Dot(eyeSize, eyeCx + eyeOffsetX, eyeY));
        grid.Children.Add(Mouth(size));

        Content = grid;
    }

    static Ellipse Dot(double diameter, double cx, double cy) => new()
    {
        WidthRequest = diameter,
        HeightRequest = diameter,
        Fill = new SolidColorBrush(Color.FromArgb("#2A2823")),
        HorizontalOptions = LayoutOptions.Start,
        VerticalOptions = LayoutOptions.Start,
        TranslationX = cx - diameter / 2,
        TranslationY = cy - diameter / 2,
    };

    Microsoft.Maui.Controls.Shapes.Path Mouth(double size)
    {
        // Quadratic curve on a 100×100 viewbox. Control-point Y picks the mouth shape:
        // score 1 (Rough) curves downward (frown), score 5 (Great) curves upward (smile).
        int score = (int)Score;
        double cpY = 70 - (score - 1) * 6; // 70 (frown) → 46 (smile)
        var geometry = (Geometry)new PathGeometryConverter().ConvertFromInvariantString(
            $"M30,60 Q50,{cpY:F1} 70,60")!;

        return new Microsoft.Maui.Controls.Shapes.Path
        {
            Data = geometry,
            Stroke = new SolidColorBrush(Color.FromArgb("#2A2823")),
            StrokeThickness = 6,
            StrokeLineCap = PenLineCap.Round,
            StrokeLineJoin = PenLineJoin.Round,
            Aspect = Stretch.Uniform,
            WidthRequest = size * 0.6,
            HeightRequest = size * 0.3,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            TranslationY = size * 0.12,
        };
    }
}
