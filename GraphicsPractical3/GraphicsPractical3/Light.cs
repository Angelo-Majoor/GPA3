// Angelo Majoor - 3843726

using Microsoft.Xna.Framework;

namespace GraphicsPractical3
{
    struct Light
    {
        Vector3 position;
        Vector3 direction;
        Vector4 color;

        public Light (Vector3 lightPosition, Vector3 lightDirection, Vector4 lightColor)
        {
            direction = lightDirection;
            position = lightPosition;
            color = lightColor;
        }

        public Vector3 lightPosition
        {
            get { return position; }
        }

        public Vector3 lightDirection
        {
            get { return direction; }
        }

        public Vector4 lightColor
        {
            get { return color; }
        }
    }
}
