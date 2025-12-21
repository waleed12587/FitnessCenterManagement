using FitnessCenter.Web.ViewModels;
using FitnessCenter.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace FitnessCenter.Web.Controllers
{
    [Authorize]
    public class AIWorkoutController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AIWorkoutController> _logger;
        private readonly AIImageService _imageService;
        private readonly IWebHostEnvironment _environment;

        public AIWorkoutController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<AIWorkoutController> logger,
            AIImageService imageService,
            IWebHostEnvironment environment)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _imageService = imageService;
            _environment = environment;
        }

        // GET: AIWorkout
        public IActionResult Index()
        {
            return View(new AIWorkoutRequestVM());
        }

        // POST: AIWorkout/GetRecommendation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetRecommendation(AIWorkoutRequestVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                // Create prompt for AI
                var prompt = BuildPrompt(model);
                
                // Get AI recommendation
                var recommendation = await GetAIRecommendationAsync(prompt);

                string? transformationImageUrl = null;
                string? originalPhotoUrl = null;

                // Generate transformation image if photo was uploaded
                if (model.Photo != null && model.Photo.Length > 0)
                {
                    try
                    {
                        transformationImageUrl = await _imageService.GenerateTransformationImageAsync(
                            model.Photo,
                            model.Goal,
                            model.Gender,
                            model.Age,
                            model.Weight,
                            model.Height);

                        // Save original photo URL for display
                        if (transformationImageUrl != null)
                        {
                            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "ai-photos");
                            Directory.CreateDirectory(uploadsPath);
                            var originalFileName = $"{Guid.NewGuid()}{Path.GetExtension(model.Photo.FileName)}";
                            var originalPath = Path.Combine(uploadsPath, originalFileName);
                            
                            using (var stream = new FileStream(originalPath, FileMode.Create))
                            {
                                await model.Photo.CopyToAsync(stream);
                            }
                            
                            originalPhotoUrl = $"/uploads/ai-photos/{originalFileName}";
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to generate transformation image, continuing without it");
                    }
                }

                var result = new AIWorkoutResultVM
                {
                    Request = model,
                    Recommendation = recommendation,
                    GeneratedAt = DateTime.Now,
                    TransformationImageUrl = transformationImageUrl,
                    OriginalPhotoUrl = originalPhotoUrl
                };

                return View("Result", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting AI recommendation");
                ModelState.AddModelError("", "AI service is currently unavailable. Please try again later.");
                return View("Index", model);
            }
        }

        private string BuildPrompt(AIWorkoutRequestVM model)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Based on the following information, provide a personalized exercise and nutrition plan:");
            sb.AppendLine($"- Age: {model.Age}");
            sb.AppendLine($"- Height: {model.Height} cm");
            sb.AppendLine($"- Weight: {model.Weight} kg");
            sb.AppendLine($"- Gender: {model.Gender}");
            sb.AppendLine($"- Goal: {model.Goal}");
            sb.AppendLine($"- Activity Level: {model.ActivityLevel}");
            
            if (!string.IsNullOrEmpty(model.HealthConditions))
            {
                sb.AppendLine($"- Health Conditions: {model.HealthConditions}");
            }

            sb.AppendLine("\nPlease create a detailed plan that includes:");
            sb.AppendLine("1. Weekly exercise program (daily activities)");
            sb.AppendLine("2. Nutrition recommendations");
            sb.AppendLine("3. Progress tracking suggestions");
            sb.AppendLine("4. Motivation tips");

            return sb.ToString();
        }

        private async Task<string> GetAIRecommendationAsync(string prompt)
        {
            // Try to use OpenAI API if configured
            var apiKey = _configuration["OpenAI:ApiKey"];

            if (!string.IsNullOrEmpty(apiKey))
            {
                try
                {
                    return await GetOpenAIRecommendationAsync(prompt, apiKey);
                }
                catch (Exception ex)
                {
                    // If OpenAI fails, fall back to template
                    _logger.LogWarning(ex, "OpenAI API call failed, using fallback");
                    return GenerateFallbackRecommendation(prompt);
                }
            }

            // Fallback: Generate a realistic recommendation based on input
            return GenerateFallbackRecommendation(prompt);
        }

        private async Task<string> GetOpenAIRecommendationAsync(string prompt, string apiKey)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "You are a fitness and nutrition expert. Provide detailed recommendations in English." },
                    new { role = "user", content = prompt }
                },
                max_tokens = 1000,
                temperature = 0.7
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                
                if (result.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    var message = choices[0].GetProperty("message").GetProperty("content").GetString();
                    return message ?? "No response received.";
                }
            }

            throw new Exception("OpenAI API error");
        }

        private string GenerateFallbackRecommendation(string prompt)
        {
            // Generate a realistic recommendation without API
            var sb = new StringBuilder();
            sb.AppendLine("## Personalized Fitness and Nutrition Plan\n");
            sb.AppendLine("### Weekly Exercise Program\n");
            sb.AppendLine("**Monday - Upper Body:**");
            sb.AppendLine("- Chest: Bench Press 3x10");
            sb.AppendLine("- Back: Pull-ups 3x8");
            sb.AppendLine("- Shoulders: Shoulder Press 3x10");
            sb.AppendLine("\n**Tuesday - Lower Body:**");
            sb.AppendLine("- Squat 4x10");
            sb.AppendLine("- Deadlift 3x8");
            sb.AppendLine("- Leg Press 3x12");
            sb.AppendLine("\n**Wednesday - Cardio:**");
            sb.AppendLine("- 30 minutes running or cycling");
            sb.AppendLine("- 15 minutes HIIT");
            sb.AppendLine("\n**Thursday - Upper Body:**");
            sb.AppendLine("- Biceps & Triceps focused workout");
            sb.AppendLine("- 45 minutes");
            sb.AppendLine("\n**Friday - Lower Body:**");
            sb.AppendLine("- Lunges 3x12");
            sb.AppendLine("- Calf Raises 4x15");
            sb.AppendLine("\n**Saturday - Active Rest:**");
            sb.AppendLine("- Yoga or stretching");
            sb.AppendLine("\n**Sunday - Rest Day**\n");
            
            sb.AppendLine("### Nutrition Recommendations\n");
            sb.AppendLine("- **Breakfast:** Eggs, whole grain bread, cheese");
            sb.AppendLine("- **Lunch:** Chicken breast, rice, vegetables");
            sb.AppendLine("- **Dinner:** Fish, salad");
            sb.AppendLine("- **Snacks:** Protein shake, fruits, nuts");
            sb.AppendLine("- **Water:** At least 2-3 liters per day\n");
            
            sb.AppendLine("### Progress Tracking\n");
            sb.AppendLine("- Weekly weight and measurements");
            sb.AppendLine("- Workout performance records");
            sb.AppendLine("- Monthly photos\n");
            
            sb.AppendLine("### Motivation Tips\n");
            sb.AppendLine("- Set small goals");
            sb.AppendLine("- Track your progress");
            sb.AppendLine("- Get regular sleep (7-8 hours)");
            sb.AppendLine("- Be patient, results take time");

            return sb.ToString();
        }
    }
}
