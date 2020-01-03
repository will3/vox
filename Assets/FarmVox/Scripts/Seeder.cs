using UnityEngine;
using Random = System.Random;

namespace FarmVox.Scripts
{
    [DefaultExecutionOrder(-100)]
    public class Seeder : MonoBehaviour
    {
        public int seed = 1599434415;
        public Ground ground;
        public Waterfalls waterfalls;
        public Trees trees;
        
        private Random _r;
        private bool _first = true;

        private void Start()
        {
            var groundConfig = ground.config;
            groundConfig.heightNoise.seed = NextSeed();
            groundConfig.grassNoise.seed = NextSeed();
            groundConfig.rockColorNoise.seed = NextSeed();
            
            waterfalls.random = new Random(NextSeed());

            trees.config.noise.seed = NextSeed();
            trees.config.random = new Random(NextSeed());
        }

        private int NextSeed()
        {
            if (_first)
            {
                _first = false;
                return seed;
            }

            if (_r == null)
            {
                _r = new Random(seed);
            }

            return _r.Next();
        }
    }
}