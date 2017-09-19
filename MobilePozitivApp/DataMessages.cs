using System;
using System.Collections.Generic;

namespace MobilePozitivApp
{
    public class Message
    {
        public bool DirectIn { get; set; }
        public long MessageID { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string UserRef { get; set; }
        public string UserStr { get; set; }
        public bool Recive { get; set; }
        public bool Read { get; set; }
        public bool Fix { get; set; }
    }

    class DataMessages
    {
        static List<Message> mMessages;

        public DataMessages()
        {
            mMessages = new List<Message>();
        }

        public void Add(Message message)
        {
            mMessages.Add(message);
        }

        public int NumMessages
        {
            get { return mMessages.Count; }
        }

        public Message this[int i]
        {
            get { return mMessages[i]; }
        }
    }
}