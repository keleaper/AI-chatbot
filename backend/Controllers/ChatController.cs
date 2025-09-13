using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using SQLitePCL;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous] // Allow both anonymous and authenticated users
    public class ChatController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ChatBotContext _context;

        public ChatController(IHttpClientFactory httpClientFactory, ChatBotContext context)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
        }

        // POST: api/Chat
        // This endpoint handles both new sessions and existing ones
        [HttpPost]
        [AllowAnonymous] // Allow both anonymous and authenticated users
        public async Task<IActionResult> Post([FromBody] ChatRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null.");
            }

            var apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, "Missing GROQ_API_KEY in env");
            }

            // System message for AI
            var systemMessage = new
            {
                role = "system",
                content = @"You're a helpful assistant...
                Always return a response in a well-structured **Markdown** format with:
                - Proper headings (using ## or ### for section titles)
                - Bullet lists using '-' (dash) with a space after, no colons
                - Numbered lists for rankings
                - One space between list items
                - No excessive punctuation after list items
                - Clear paragraph spacing for readability"
            };

            var allMessages = new List<object> { systemMessage };
            if (request.Messages != null)
            {
                allMessages.AddRange(request.Messages.Select(m => new { role = m.role, content = m.content }));
            }

            var requestBody = new
            {
                model = "meta-llama/llama-4-scout-17b-16e-instruct",
                messages = allMessages
            };

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ChatResponse>(json);

            string reply = result?.choices.FirstOrDefault()?.message?.content ?? "No reply from AI.";

            string? sessionId = null;


            // === Save chat history ONLY if user is authenticated ===
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.Identity != null && User.Identity.IsAuthenticated && userIdClaim != null)
            {
                int userId = int.Parse(userIdClaim);

                // Save in the ChatHistory table
                await _context.ChatHistories.AddAsync(new ChatHistory
                {
                    UserId = userIdClaim,
                    Messages = JsonConvert.SerializeObject(request.Messages),
                    Timestamp = DateTime.UtcNow
                });

                ChatSession? session = null;
                if (request.SessionId.HasValue)
                {
                    session = await _context.ChatSessions
                        .Include(cs => cs.Messages)
                        .FirstOrDefaultAsync(cs => cs.Id == request.SessionId.Value && cs.UserId == userId);
                }

                if (session == null)
                {
                    session = new ChatSession
                    {
                        UserId = userId,
                        Title = $"Chat {DateTime.Now:yyyy-MM-dd HH:mm}",
                        LastUpdated = DateTime.UtcNow,
                        Messages = new List<ChatMessage>()
                    };
                    _context.ChatSessions.Add(session);
                }

                if (request.Messages != null)
                {
                    foreach (var msg in request.Messages)
                    {
                        session.Messages.Add(new ChatMessage
                        {
                            Role = msg.role ?? "user",
                            Content = msg.content ?? "",
                            Timestamp = DateTime.UtcNow
                        });
                    }
                }
                // Add all messages to the session, timestamped


                // Add AI's reply as assistant message
                session.Messages.Add(new ChatMessage
                {
                    Role = "assistant",
                    Content = reply,
                    Timestamp = DateTime.UtcNow
                });

                session.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                sessionId = session.Id.ToString();

                // Return session ID to client so it can track for later messages
                // return Ok(new { response = reply, sessionId = session.Id });
            }
            else
            {
                // Anoymous user - use a temp session ID
                sessionId = Guid.NewGuid().ToString(); 
            }

            // Anonymous user - just return response without saving
            return Ok(new { response = reply, sessionId = (int?)null });
        }
    }

}










    // Save user's latest message
    // var userMsg = request.Messages.LastOrDefault();
    // if (userMsg != null)
    // {
    //     session.Messages.Add(new ChatMessage
    //     {
    //         Role = "user",
    //         Content = userMsg.content ?? "",
    //         Timestamp = DateTime.UtcNow
    //     });
    // }

    // await _context.SaveChangesAsync();

    // Send to Groq API
    //var apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");





    // var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
    // _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);



    // Save AI's response
    // session.Messages.Add(new ChatMessage
    // {
    //     Role = "assistant",
    //     Content = reply,
    //     Timestamp = DateTime.UtcNow
    // });

    // session.LastUpdated = DateTime.UtcNow;
    // await _context.SaveChangesAsync();

    // return Ok(new { response = reply, sessionId = session.Id });
// }







        // if (request == null)
        // {
        //     Console.WriteLine("Reveived null request");
        //     return BadRequest("Request cannot be null.");
        // }
        // Console.WriteLine($"Received Messages count: {request.Messages?.Count}");
        // Console.WriteLine($"Received SessionId: {request.SessionId}");


        // var apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");
        // if (string.IsNullOrEmpty(apiKey))
        // {
        //     return StatusCode(500, "Missing GROQ_API_KEY in env");
        // }

        // var systemMessage = new // This is the system message that sets the context for the AI SO COOL YOU CAN GIVE INSTRUCTIONS AND IT WILL FOLLOW THEM
        // {
        //     role = "system",
        //     content = @"You are a helpful assistant.
        //     Always return a response in a well-structured **Markdown** format. with:
        //     - Proper headings (using ## or ### for section titles)
        //     - Bullet lists using '-' (dash) with a space after, no colons
        //     - Numbered lists for rankings
        //     - Bold movie titles and years
        //     - One space between list items
        //     - No excessive punctuation after list items
        //     - Clear paragraph spacing for readability"
        // };

        // var allMessages = new List<object> { systemMessage };
        // if (request.Messages != null)
        // {
        //     allMessages.AddRange(request.Messages.Select(m => new { role = m.role, content = m.content }));
        // }


        // var requestBody = new
        // {
        //     model = "meta-llama/llama-4-scout-17b-16e-instruct",
        //     messages = allMessages // sends entire histroy of messages without Ids
        // };

        // var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        // _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        // var response = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", requestContent);
        // var json = await response.Content.ReadAsStringAsync();


        // var result = JsonConvert.DeserializeObject<ChatResponse>(json);
        // Console.WriteLine($"Response: {json}");

        // if (result?.choices != null && result.choices.Count > 0)
        // {
        //     // message?.content "get content if message is not null
        //     // ?? "No reply from AI" meants if content is null, fallback to this string
        //     string reply = result.choices[0].message?.content ?? "No reply from AI.";
        //     return Ok(new { response = reply });
        // }
        // else
        // {
        //     return BadRequest("No valid response received.");
        // }
        //}
    //}
//}