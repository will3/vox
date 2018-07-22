public partial class Card
{
    static class SpriteSheets
    {
        public static SpriteSheet Get(string name)
        {
            if (name == "robot")
            {
                return new RobotSpriteSheet();
            }
            return null;
        }
    }

    class RobotSpriteSheet : SpriteSheet {
        public RobotSpriteSheet() {
            idle = new string[] { "rob_0" };
            walk = new string[] { "rob_0", "rob_1", "rob_2" };
        }
    }
}
