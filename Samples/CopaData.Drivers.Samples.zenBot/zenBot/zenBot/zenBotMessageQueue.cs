using System;
using System.Collections.Generic;
using System.Text;

namespace zenBot.zenBot
{
  public class zenBotMessageQueue
  {
    public int QueueSize { get; set; }
    public List<zenBotMessage> ZenBotMessages { get; set; }
    public zenBotMessageQueue(int queueSize = 5)
    {
      QueueSize = queueSize;
    }

    
  }
}
