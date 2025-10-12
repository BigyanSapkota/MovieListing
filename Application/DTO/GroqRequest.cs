using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class GroqRequest
    {
        public string Model { get; set; }
        public string Prompt { get; set; }
        public int MaxTokens { get; set; } = 100;
    }


    public class ChatMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    public class ChatCompletionRequest
    {
        public string Model { get; set; }
        public List<ChatMessage> Messages { get; set; }
    }

    public class ChatCompletionChoice
    {
        public int Index { get; set; }
        public ChatMessage Message { get; set; }
        public string Finish_Reason { get; set; }
    }

    public class ChatsCompletionResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }
        public string Model { get; set; }
        public List<ChatCompletionChoice> Choices { get; set; }
    }



}
