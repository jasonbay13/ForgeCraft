using System;
using System.Collections.Generic;
using System.Threading;

namespace SMP
{
    public class Data
    {
        public byte Block;
        public byte time;
        public ushort x;
        public ushort y;
        public ushort z;
        public Data(byte block, ushort x, ushort y, ushort z) { Block = block; this.x = x; this.y = y; this.z = z; }
    }
    public class Physics
    {
        public PSetting setting;
        private Thread physthread;
        private List<Data> Checks = new List<Data>();
        public Physics()
        {
            setting = PSetting.Normal;
        }
        public Physics(PSetting setting)
        {
            this.setting = setting;
        }
        public void Stop()
        {
            Checks.Clear();
            if (physthread != null)
            {
                physthread.Abort();
                physthread.Join();
            }
            setting = PSetting.None;
        }
        public void Start()
        {
            physthread = new Thread(new ThreadStart(CalcPhysics));
        }
        private void CalcPhysics()
        {

        }
    }
}
