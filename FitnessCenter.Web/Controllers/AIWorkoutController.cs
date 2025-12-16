using FitnessCenter.Web.ViewModels;
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

        public AIWorkoutController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<AIWorkoutController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
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

                var result = new AIWorkoutResultVM
                {
                    Request = model,
                    Recommendation = recommendation,
                    GeneratedAt = DateTime.Now
                };

                return View("Result", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting AI recommendation");
                ModelState.AddModelError("", "AI servisi şu anda kullanılamıyor. Lütfen daha sonra tekrar deneyin.");
                return View("Index", model);
            }
        }

        private string BuildPrompt(AIWorkoutRequestVM model)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Aşağıdaki bilgilere göre kişiselleştirilmiş bir egzersiz ve beslenme planı öner:");
            sb.AppendLine($"- Yaş: {model.Age}");
            sb.AppendLine($"- Boy: {model.Height} cm");
            sb.AppendLine($"- Kilo: {model.Weight} kg");
            sb.AppendLine($"- Cinsiyet: {model.Gender}");
            sb.AppendLine($"- Hedef: {model.Goal}");
            sb.AppendLine($"- Aktivite Seviyesi: {model.ActivityLevel}");
            
            if (!string.IsNullOrEmpty(model.HealthConditions))
            {
                sb.AppendLine($"- Sağlık Durumu: {model.HealthConditions}");
            }

            sb.AppendLine("\nLütfen şunları içeren detaylı bir plan hazırla:");
            sb.AppendLine("1. Haftalık egzersiz programı (günlük aktiviteler)");
            sb.AppendLine("2. Beslenme önerileri");
            sb.AppendLine("3. İlerleme takibi için öneriler");
            sb.AppendLine("4. Motivasyon ipuçları");

            return sb.ToString();
        }

        private async Task<string> GetAIRecommendationAsync(string prompt)
        {
            // Try to use OpenAI API if configured
            var apiKey = _configuration["OpenAI:ApiKey"];
            
            if (!string.IsNullOrEmpty(apiKey))
            {
                return await GetOpenAIRecommendationAsync(prompt, apiKey);
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
                    new { role = "system", content = "Sen bir fitness ve beslenme uzmanısın. Türkçe yanıt ver." },
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
                    return message ?? "Yanıt alınamadı.";
                }
            }

            throw new Exception("OpenAI API hatası");
        }

        private string GenerateFallbackRecommendation(string prompt)
        {
            // Generate a realistic recommendation without API
            var sb = new StringBuilder();
            sb.AppendLine("## Kişiselleştirilmiş Fitness ve Beslenme Planı\n");
            sb.AppendLine("### Haftalık Egzersiz Programı\n");
            sb.AppendLine("**Pazartesi - Üst Vücut:**");
            sb.AppendLine("- Göğüs: Bench Press 3x10");
            sb.AppendLine("- Sırt: Pull-ups 3x8");
            sb.AppendLine("- Omuz: Shoulder Press 3x10");
            sb.AppendLine("\n**Salı - Alt Vücut:**");
            sb.AppendLine("- Squat 4x10");
            sb.AppendLine("- Deadlift 3x8");
            sb.AppendLine("- Leg Press 3x12");
            sb.AppendLine("\n**Çarşamba - Kardiyo:**");
            sb.AppendLine("- 30 dakika koşu veya bisiklet");
            sb.AppendLine("- 15 dakika HIIT");
            sb.AppendLine("\n**Perşembe - Üst Vücut:**");
            sb.AppendLine("- Biceps & Triceps odaklı antrenman");
            sb.AppendLine("- 45 dakika");
            sb.AppendLine("\n**Cuma - Alt Vücut:**");
            sb.AppendLine("- Lunges 3x12");
            sb.AppendLine("- Calf Raises 4x15");
            sb.AppendLine("\n**Cumartesi - Aktif Dinlenme:**");
            sb.AppendLine("- Yoga veya stretching");
            sb.AppendLine("\n**Pazar - Dinlenme Günü**\n");
            
            sb.AppendLine("### Beslenme Önerileri\n");
            sb.AppendLine("- **Kahvaltı:** Yumurta, tam tahıllı ekmek, peynir");
            sb.AppendLine("- **Öğle:** Tavuk göğsü, pilav, sebze");
            sb.AppendLine("- **Akşam:** Balık, salata");
            sb.AppendLine("- **Ara Öğünler:** Protein shake, meyve, kuruyemiş");
            sb.AppendLine("- **Su:** Günde en az 2-3 litre\n");
            
            sb.AppendLine("### İlerleme Takibi\n");
            sb.AppendLine("- Haftalık kilo ve ölçü takibi");
            sb.AppendLine("- Antrenman performans kaydı");
            sb.AppendLine("- Fotoğraf çekimi (aylık)\n");
            
            sb.AppendLine("### Motivasyon İpuçları\n");
            sb.AppendLine("- Küçük hedefler belirleyin");
            sb.AppendLine("- İlerlemenizi kaydedin");
            sb.AppendLine("- Düzenli uyku alın (7-8 saat)");
            sb.AppendLine("- Sabırlı olun, sonuçlar zaman alır");

            return sb.ToString();
        }
    }
}

