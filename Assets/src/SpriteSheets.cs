public partial class Card
{
    static class SpriteSheets
    {
        public static SpriteSheet Get(string name)
        {
            if (name == "archer") {
                return new ArcherSpriteSheet();
            }

            return null;
        }

        public static string[] NamesWithPrefix(string prefix, int count) {
            var list = new string[count];
            for (var i = 0; i < count; i++) {
                list[i] = prefix + count;
            }
            return list;
        }
    }

    class SpriteSheet
    {
        public string[] idle = { };
        public string[] walk = { };
        public string[] attack = { };
        public string prefix = "";
    }

    class ArcherSpriteSheet: SpriteSheet {
        public ArcherSpriteSheet() {
            prefix = "archer/";
            idle = new string[] { "archer_attack_0" };
            attack = SpriteSheets.NamesWithPrefix("archer_attack_", 7);
        }
    }
}
