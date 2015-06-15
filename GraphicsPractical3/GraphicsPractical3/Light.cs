// Angelo Majoor - 3843726

using Microsoft.Xna.Framework;

namespace GraphicsPractical3
{
    struct Light
    {
        Vector3 position;
        Vector4 color;

        public Light (Vector3 lightPosition, Vector4 lightColor)
        {
            position = lightPosition;
            color = lightColor;
        }

        public Vector3 lightPosition
        {
            get { return position; }
        }

        public Vector4 lightColor
        {
            get { return color; }
        }
    }
}
