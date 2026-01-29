using System;
using System.Collections.Generic;
using System.Text;

namespace MauiRangeControl.Controls
{
    public class RangeSlider : GraphicsView
    {
        // ===== VALUE PROPERTIES =====

        public static readonly BindableProperty LabelFormatterProperty =
        BindableProperty.Create(nameof(LabelFormatter), typeof(Func<double, string>), typeof(RangeSlider), default(Func<double, string>), propertyChanged: OnInvalidate);

        public static readonly BindableProperty LabelTextAlignmentProperty =
            BindableProperty.Create(nameof(LabelTextAlignment), typeof(HorizontalAlignment), typeof(RangeSlider), HorizontalAlignment.Center, propertyChanged: OnInvalidate);

        public static readonly BindableProperty MinimumProperty =
            BindableProperty.Create(nameof(Minimum), typeof(double), typeof(RangeSlider), 0.0, propertyChanged: OnInvalidate);

        public static readonly BindableProperty MaximumProperty =
            BindableProperty.Create(nameof(Maximum), typeof(double), typeof(RangeSlider), 1000.0, propertyChanged: OnInvalidate);

        public static readonly BindableProperty SelectedMinProperty =
            BindableProperty.Create(nameof(SelectedMin), typeof(double), typeof(RangeSlider), 100.0,
                BindingMode.TwoWay, propertyChanged: OnInvalidate);

        public static readonly BindableProperty SelectedMaxProperty =
            BindableProperty.Create(nameof(SelectedMax), typeof(double), typeof(RangeSlider), 900.0,
                BindingMode.TwoWay, propertyChanged: OnInvalidate);

        // ===== APPEARANCE =====
        public static readonly BindableProperty TrackColorProperty =
            BindableProperty.Create(nameof(TrackColor), typeof(Color), typeof(RangeSlider), Colors.LightGray, propertyChanged: OnInvalidate);

        public static readonly BindableProperty RangeColorProperty =
            BindableProperty.Create(nameof(RangeColor), typeof(Color), typeof(RangeSlider), Colors.Green, propertyChanged: OnInvalidate);

        public static readonly BindableProperty TrackHeightProperty =
            BindableProperty.Create(nameof(TrackHeight), typeof(float), typeof(RangeSlider), 6f, propertyChanged: OnInvalidate);

        public static readonly BindableProperty ThumbRadiusProperty =
            BindableProperty.Create(nameof(ThumbRadius), typeof(float), typeof(RangeSlider), 12f, propertyChanged: OnInvalidate);

        public static readonly BindableProperty ThumbColorProperty =
            BindableProperty.Create(nameof(ThumbColor), typeof(Color), typeof(RangeSlider), Colors.White, propertyChanged: OnInvalidate);

        public static readonly BindableProperty ThumbBorderColorProperty =
            BindableProperty.Create(nameof(ThumbBorderColor), typeof(Color), typeof(RangeSlider), Colors.Gray, propertyChanged: OnInvalidate);

        public static readonly BindableProperty LabelBackgroundColorProperty =
            BindableProperty.Create(nameof(LabelBackgroundColor), typeof(Color), typeof(RangeSlider), Colors.Black, propertyChanged: OnInvalidate);

        public static readonly BindableProperty LabelTextColorProperty =
            BindableProperty.Create(nameof(LabelTextColor), typeof(Color), typeof(RangeSlider), Colors.White, propertyChanged: OnInvalidate);

        // ===== CLR PROPERTIES =====
        public double Minimum { get => (double)GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
        public double Maximum { get => (double)GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
        public double SelectedMin { get => (double)GetValue(SelectedMinProperty); set => SetValue(SelectedMinProperty, value); }
        public double SelectedMax { get => (double)GetValue(SelectedMaxProperty); set => SetValue(SelectedMaxProperty, value); }

        public Color TrackColor { get => (Color)GetValue(TrackColorProperty); set => SetValue(TrackColorProperty, value); }
        public Color RangeColor { get => (Color)GetValue(RangeColorProperty); set => SetValue(RangeColorProperty, value); }
        public float TrackHeight { get => (float)GetValue(TrackHeightProperty); set => SetValue(TrackHeightProperty, value); }

        public float ThumbRadius { get => (float)GetValue(ThumbRadiusProperty); set => SetValue(ThumbRadiusProperty, value); }
        public Color ThumbColor { get => (Color)GetValue(ThumbColorProperty); set => SetValue(ThumbColorProperty, value); }
        public Color ThumbBorderColor { get => (Color)GetValue(ThumbBorderColorProperty); set => SetValue(ThumbBorderColorProperty, value); }

        public Color LabelBackgroundColor { get => (Color)GetValue(LabelBackgroundColorProperty); set => SetValue(LabelBackgroundColorProperty, value); }
        public Color LabelTextColor { get => (Color)GetValue(LabelTextColorProperty); set => SetValue(LabelTextColorProperty, value); }
        public HorizontalAlignment LabelTextAlignment { get => (HorizontalAlignment)GetValue(LabelTextAlignmentProperty); set => SetValue(LabelTextAlignmentProperty, value); }
        public Func<double, string> LabelFormatter { get => (Func<double, string>)GetValue(LabelFormatterProperty); set => SetValue(LabelFormatterProperty, value); }

        bool dragMin, dragMax;

        public RangeSlider()
        {
            Drawable = new RangeSliderDrawable(this);

            StartInteraction += OnStart;
            DragInteraction += OnDrag;
            EndInteraction += (_, __) => dragMin = dragMax = false;
        }

        void OnStart(object? sender, TouchEventArgs e)
        {
            double range = Maximum - Minimum;
            double usable = Width - ThumbRadius * 2;

            double x = e.Touches[0].X;
            double minX = ThumbRadius + (SelectedMin - Minimum) / range * usable;
            double maxX = ThumbRadius + (SelectedMax - Minimum) / range * usable;

            dragMin = Math.Abs(x - minX) < Math.Abs(x - maxX);
            dragMax = !dragMin;
        }

        void OnDrag(object? sender, TouchEventArgs e)
        {
            double range = Maximum - Minimum;
            double usable = Width - ThumbRadius * 2;

            double x = Math.Clamp(e.Touches[0].X - ThumbRadius, 0, usable);
            double value = Minimum + (x / usable) * range;

            if (dragMin) SelectedMin = Math.Min(value, SelectedMax);
            else SelectedMax = Math.Max(value, SelectedMin);
        }

        static void OnInvalidate(BindableObject b, object o, object n)
            => ((RangeSlider)b).Invalidate();
    }
    public class RangeSliderDrawable : IDrawable
    {
        readonly RangeSlider s;
        const float LabelHeight = 24f;
        const float LabelPadding = 6f;

        public RangeSliderDrawable(RangeSlider slider) => s = slider;

        public void Draw(ICanvas canvas, RectF rect)
        {
            float usable = rect.Width - s.ThumbRadius * 2;
            float centerY = rect.Height / 2 + 10;
            double range = s.Maximum - s.Minimum;

            float minX = s.ThumbRadius + (float)((s.SelectedMin - s.Minimum) / range * usable);
            float maxX = s.ThumbRadius + (float)((s.SelectedMax - s.Minimum) / range * usable);

            canvas.FillColor = s.TrackColor;
            canvas.FillRoundedRectangle(s.ThumbRadius, centerY - s.TrackHeight / 2, usable, s.TrackHeight, s.TrackHeight / 2);

            canvas.FillColor = s.RangeColor;
            canvas.FillRoundedRectangle(minX, centerY - s.TrackHeight / 2, maxX - minX, s.TrackHeight, s.TrackHeight / 2);

            DrawLabel(canvas, minX, centerY - 30, s.SelectedMin);
            DrawLabel(canvas, maxX, centerY - 30, s.SelectedMax);

            DrawThumb(canvas, minX, centerY);
            DrawThumb(canvas, maxX, centerY);
        }

        void DrawThumb(ICanvas c, float x, float y)
        {
            c.FillColor = s.ThumbColor;
            c.StrokeColor = s.ThumbBorderColor;
            c.StrokeSize = 2;
            c.FillCircle(x, y, s.ThumbRadius);
            c.DrawCircle(x, y, s.ThumbRadius);
        }

        void DrawLabel(ICanvas c, float x, float y, double value)
        {
            // Use formatter if set, else default to currency
            string text = s.LabelFormatter != null ? s.LabelFormatter(value) : $"${value:F0}";

            c.FontSize = 12;
            c.FontColor = s.LabelTextColor;

            var size = c.GetStringSize(text, null, 12);
            float width = size.Width + LabelPadding * 2;

            float rectX = x - width / 2;
            float rectY = y - LabelHeight;
            float rectWidth = width;
            float rectHeight = LabelHeight;

            c.FillColor = s.LabelBackgroundColor;
            c.FillRoundedRectangle(rectX, rectY, rectWidth, rectHeight, 6);

            float drawX = x;
            switch (s.LabelTextAlignment)
            {
                case HorizontalAlignment.Left:
                    drawX = rectX + LabelPadding;
                    break;
                case HorizontalAlignment.Center:
                    drawX = rectX + rectWidth / 2;
                    break;
                case HorizontalAlignment.Right:
                    drawX = rectX + rectWidth - LabelPadding;
                    break;
            }

            float drawY = rectY + rectHeight / 2 + size.Height / 4; // vertical center
            c.DrawString(text, drawX, drawY, s.LabelTextAlignment);
        }
    }

}
