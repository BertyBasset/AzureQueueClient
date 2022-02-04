using System;
using System.Collections.Generic;
using Azure.Storage.Queues;         // Namespace for Queue storage types
using Azure.Storage.Queues.Models;  // Namespace for PeekedMessage

namespace ShedLab {
    public class AzureQueueClient {
        public enum QueueMode {
            Peek,
            Dequeue
        }

        QueueClient queueClient;
        QueueMode queueMode;
        int batchSize;

        // Constructor
        public AzureQueueClient(string connectionString, string queueName, QueueMode queueMode = QueueMode.Dequeue, int batchSize = 10, Logger.LogLevel logLevel = Logger.LogLevel.Verbose) {
            // Instantiate a QueueClient which will be used to create and manipulate the queue
            this.queueClient = new QueueClient(connectionString, queueName);
            this.queueMode = queueMode;
            this.batchSize = batchSize;
            Logger.logLevel = logLevel;
        }

        // Get list of items of specific type from Azure Queue
        public List<T> GetItems<T>() where T : class{
            var list = new List<T>();

            if (queueMode == QueueMode.Peek) {
                // This version does not dequeue items, and is useful for initial dev
                PeekedMessage[] peekedMessages = queueClient.PeekMessages(batchSize);

                foreach (var message in peekedMessages) {
                    var deserialised = message.As<T>();
                    
                    list.Add(deserialised);
                    // Display the message
                    Logger.Log(Logger.LogLevel.Verbose, message.AsString());
                }

                Logger.Log(Logger.LogLevel.Info, $"Peeked {peekedMessages.Length} item{(peekedMessages.Length == 1 ? "" : "s")} from queue '{queueClient.Name}'");

            } else {
                // This version deqeueues the items

                QueueMessage[] receiveMessages = queueClient.ReceiveMessages(batchSize);

                foreach (var message in receiveMessages) {
                    try {
                        var deserialised = message.As<T>();
                        list.Add(deserialised);
                        // Display the message
                        Logger.Log(Logger.LogLevel.Verbose, message.AsString());
                    } catch (Exception ex) {
                        Logger.Log(ex);
                    }
                    // Remove message from the queue
                    queueClient.DeleteMessage(message.MessageId, message.PopReceipt);

                }

                Logger.Log(Logger.LogLevel.Info, $"Received {receiveMessages.Length} item{(receiveMessages.Length == 1 ? "" : "s")} from queue '{queueClient.Name}'");

            }
            return list;
        }
    }
}
