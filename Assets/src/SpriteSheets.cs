public partial class Card
{
    static class SpriteSheets
    {
        public static SpriteSheet Get(string name)
        {
            if (name == "monster")
            {
                return new MonsterSpriteSheet();
            }
            return null;
        }
    }
}
