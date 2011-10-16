using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMP
{
    public class Chunker
    {
        private Queue<ChunkGenQueue> genQueue;
        private List<ChunkSendQueue> sendQueue;
        private List<ChunkGenQueue> generated;
        private Thread genThread, sendThread;


        public Chunker()
        {
            genQueue = new Queue<ChunkGenQueue>();
            sendQueue = new List<ChunkSendQueue>();
            generated = new List<ChunkGenQueue>();
        }

        public void Start()
        {
            genThread = new Thread(new ThreadStart(delegate
            {
                Parallel.For(0, Server.genThreads, delegate(int wtf) // The int is so the compiler will shut up.
                {
                    ChunkGenQueue cgq; Chunk ch;
                    while (!Server.s.shuttingDown)
                    {
                        try
                        {
                            if (genQueue.Count < 1) { Thread.Sleep(100); continue; }
                            lock (genQueue)
                                cgq = genQueue.Dequeue();
                            lock (generated)
                                generated.Add(cgq);
                            ch = cgq.world.GenerateChunk(cgq.x, cgq.z);
                            Point pt = new Point(cgq.x, cgq.z);
                            lock (cgq.world.chunkData)
                                if (!cgq.world.chunkData.ContainsKey(pt))
                                    cgq.world.chunkData.Add(pt, ch);
                            lock (generated)
                                generated.Remove(cgq);
                        }
                        catch { }
                    }
                });
            }));
            genThread.Start();

            sendThread = new Thread(new ThreadStart(delegate
            {
                while (!Server.s.shuttingDown)
                {
                    try
                    {
                        if (sendQueue.Count < 1) { Thread.Sleep(100); continue; }
                        sendQueue.ForEach(delegate(ChunkSendQueue csq)
                        {
                            if (csq.player.level.chunkData.ContainsKey(new Point(csq.x, csq.z)))
                            {
                                //Console.WriteLine("SENT " + csq.x + "," + csq.z);
                                csq.player.SendChunk(csq.player.level.chunkData[new Point(csq.x, csq.z)]);
                                sendQueue.Remove(csq);
                                Thread.Sleep(2);
                            }
                        });
                    }
                    catch { }
                }
            }));
            sendThread.Start();
        }

        public void QueueChunk(Point point, World world)
        {
            QueueChunk(point.x, point.z, world);
        }
        public void QueueChunk(int x, int z, World world)
        {
            ChunkGenQueue cgq = new ChunkGenQueue(x, z, world);
            lock (generated)
                if (generated.Contains(cgq)) return;
            lock (genQueue)
                genQueue.Enqueue(cgq);
        }

        public void QueueChunkSend(Point point, Player player)
        {
            QueueChunkSend(point.x, point.z, player);
        }
        public void QueueChunkSend(int x, int z, Player player)
        {
            ChunkSendQueue csq = new ChunkSendQueue(x, z, player);
            if (sendQueue.Contains(csq)) return;
            sendQueue.Add(csq);
        }
    }


    public struct ChunkGenQueue
    {
        public int x, z;
        public World world;

        public ChunkGenQueue(Point point, World world) : this(point.x, point.z, world) { }
        public ChunkGenQueue(int x, int z, World world)
        {
            this.x = x;
            this.z = z;
            this.world = world;
        }
    }

    public struct ChunkSendQueue
    {
        public int x, z;
        public Player player;

        public ChunkSendQueue(Point point, Player player) : this(point.x, point.z, player) { }
        public ChunkSendQueue(int x, int z, Player player)
        {
            this.x = x;
            this.z = z;
            this.player = player;
        }
    }
}
