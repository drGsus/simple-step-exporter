namespace SimpleStepWriter.Helper
{
    public class Color
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public Color()
        {
            R = 0.5f;
            G = 0.5f;
            B = 0.5f;
            A = 1f;
        }

        public Color(float r, float g, float b, float alpha)
        {
            R = r;
            G = g;
            B = b;
            A = alpha;
        }

        public static Color Red { get; } = new Color(1f, 0f, 0f, 1f);
        public static Color Blue { get; } = new Color(0f, 0.1f, 1f, 1f);
        public static Color Yellow { get; } = new Color(1f, 0.9f, 0.1f, 1f);
        public static Color Green { get; } = new Color(0f, 0.8f, 0f, 1f);
        public static Color White { get; } = new Color(1f, 1f, 1f, 1f);
        public static Color Black { get; } = new Color(0f, 0f, 0f, 1f);
    }
}
