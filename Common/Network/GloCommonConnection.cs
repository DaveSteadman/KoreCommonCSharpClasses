using System.Collections.Generic;
using System.Collections.Concurrent;

namespace GloNetworking
{
    public abstract class GloCommonConnection
    {
        public string Name { set; get; } = "DefaultConnection";

        public abstract string type();
        public abstract string connectionDetailsString();

        public abstract void sendMessage(string msgData);

        public abstract void startConnection();
        public abstract void stopConnection();

        // ========================================================================================
        // Incoming message queue
        // ========================================================================================

        public BlockingCollection<GloMessageText> IncomingQueue = new BlockingCollection<GloMessageText>();
        public List<GloMessageText> IncomingMessageLog = new List<GloMessageText>();

        public void setupIncomingQueue(BlockingCollection<GloMessageText> newIncomingQueue)
        {
            IncomingQueue = newIncomingQueue;
            IncomingMessageLog = new List<GloMessageText>();
        }

        public void QueueIncomingMessage(string msgData)
        {
            GloMessageText newMsg = new GloMessageText();

            newMsg.connectionName = Name;
            newMsg.msgData = msgData;

            IncomingMessageLog.Add(newMsg);
            IncomingQueue.Add(newMsg);
        }

        public void QueueIncomingMessage(GloMessageText newMsg)
        {
            IncomingMessageLog.Add(newMsg);
            IncomingQueue.Add(newMsg);
        }

        public bool hasIncomingMessage()
        {
            return IncomingQueue.Count > 0;
        }

        public GloMessageText getNextIncomingMessage()
        {
            return IncomingQueue.Take();
        }

    }

}
