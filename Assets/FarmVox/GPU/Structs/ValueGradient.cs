namespace FarmVox.GPU.Structs
{
    public struct ValueGradient
    {
        public float[] Keys;
        public float[] Values;
        public int Banding;
        public int Size;

        public static int Stride
        {
            get { return sizeof(float) * 8 + sizeof(float) * 8 + sizeof(int) * 2; }
        }
    }
}