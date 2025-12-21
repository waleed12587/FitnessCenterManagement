using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using SixLabors.ImageSharp.Formats.Png;
using System.Text;
using System.Text.Json;

namespace FitnessCenter.Web.Services
{
    public class AIImageService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AIImageService> _logger;
        private readonly IWebHostEnvironment _environment;

        public AIImageService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<AIImageService> logger,
            IWebHostEnvironment environment)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _environment = environment;
        }

        public async Task<string?> GenerateTransformationImageAsync(
            IFormFile originalPhoto,
            string goal,
            string gender,
            int age,
            decimal weight,
            int height)
        {
            try
            {
                // Create uploads directory if it doesn't exist
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "ai-photos");
                Directory.CreateDirectory(uploadsPath);

                // Save original photo
                var originalFileName = $"{Guid.NewGuid()}{Path.GetExtension(originalPhoto.FileName)}";
                var originalPath = Path.Combine(uploadsPath, originalFileName);
                
                using (var stream = new FileStream(originalPath, FileMode.Create))
                {
                    await originalPhoto.CopyToAsync(stream);
                }

                // Use image-to-image transformation to transform the entire photo
                // This will naturally transform the body while keeping the face
                var transformedImagePath = await TransformImageWithAIAsync(originalPath, goal, gender, age, weight, height, uploadsPath);
                
                if (string.IsNullOrEmpty(transformedImagePath))
                {
                    _logger.LogWarning("Could not transform image with AI");
                    return null;
                }

                // Apply professional enhancements to make it look alive and professional
                var finalImagePath = await ApplyProfessionalEnhancementsAsync(transformedImagePath, uploadsPath);

                // Clean up temporary files
                try
                {
                    if (File.Exists(transformedImagePath) && transformedImagePath != finalImagePath)
                    {
                        File.Delete(transformedImagePath);
                    }
                }
                catch { }

                return finalImagePath != null ? $"/uploads/ai-photos/{Path.GetFileName(finalImagePath)}" : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating transformation image");
                return null;
            }
        }

        private async Task<string?> ExtractFaceAsync(string imagePath)
        {
            try
            {
                using var image = await Image.LoadAsync(imagePath);
                
                // Improved face extraction: better detection of face region
                // Assume face is in upper center portion, but use better proportions
                var faceWidth = (int)(image.Width * 0.4); // 40% of image width
                var faceHeight = (int)(image.Height * 0.35); // 35% of image height
                var faceX = image.Width / 2 - faceWidth / 2;
                var faceY = (int)(image.Height * 0.08); // 8% from top for better face detection

                // Ensure we don't go out of bounds
                if (faceX < 0) faceX = 0;
                if (faceY < 0) faceY = 0;
                if (faceX + faceWidth > image.Width) faceWidth = image.Width - faceX;
                if (faceY + faceHeight > image.Height) faceHeight = image.Height - faceY;

                // Crop face region with better quality
                var faceImage = image.Clone(ctx => ctx
                    .Crop(new Rectangle(faceX, faceY, faceWidth, faceHeight))
                    .Resize(new ResizeOptions
                    {
                        Size = new Size(512, 512), // Higher resolution for better quality
                        Mode = ResizeMode.Crop,
                        Sampler = KnownResamplers.Lanczos3 // High quality resampling
                    }));

                // Save extracted face
                var uploadsPath = Path.GetDirectoryName(imagePath);
                var faceFileName = $"face_{Guid.NewGuid()}.png";
                var facePath = Path.Combine(uploadsPath!, faceFileName);
                
                await faceImage.SaveAsync(facePath, new PngEncoder());
                
                return facePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting face");
                return null;
            }
        }

        private async Task<string?> TransformImageWithAIAsync(string originalImagePath, string goal, string gender, int age, decimal weight, int height, string outputDirectory)
        {
            var apiKey = _configuration["StabilityAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("StabilityAI API key not configured");
                return null;
            }

            try
            {
                // Build transformation prompt
                var prompt = BuildTransformationPrompt(goal, gender, age, weight, height);

                // Read original image and convert to base64
                var imageBytes = await File.ReadAllBytesAsync(originalImagePath);
                var base64Image = Convert.ToBase64String(imageBytes);

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                // Use image-to-image endpoint for better face preservation
                var requestBody = new
                {
                    text_prompts = new[]
                    {
                        new { text = prompt, weight = 1.0 },
                        new { text = "blurry, low quality, distorted face, bad anatomy", weight = -1.0 }
                    },
                    init_image = base64Image,
                    image_strength = 0.35, // Lower strength to preserve face better (0.0-1.0)
                    cfg_scale = 7,
                    height = 1024,
                    width = 1024,
                    steps = 50, // More steps for better quality
                    samples = 1
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Use Stability AI's image-to-image endpoint
                // Note: Stability AI image-to-image might use different endpoint structure
                // Try the standard endpoint first
                var response = await client.PostAsync(
                    "https://api.stability.ai/v1/generation/stable-diffusion-xl-1024-v1-0/image-to-image", 
                    content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Stability AI image-to-image response received");
                    
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    if (result.TryGetProperty("artifacts", out var artifacts) && artifacts.GetArrayLength() > 0)
                    {
                        var transformedBase64 = artifacts[0].GetProperty("base64").GetString();
                        if (!string.IsNullOrEmpty(transformedBase64))
                        {
                            _logger.LogInformation("Transformed image received, saving to file");
                            return await SaveBase64ImageAsync(transformedBase64, "transformed");
                        }
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Stability AI API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transforming image with Stability AI");
                return null;
            }
        }

        private string BuildTransformationPrompt(string goal, string gender, int age, decimal weight, int height)
        {
            var sb = new StringBuilder();
            sb.Append("Professional fitness transformation photo, ");
            sb.Append("realistic and natural body transformation, ");
            sb.Append("high quality professional photography, ");
            sb.Append("studio lighting, ");
            
            // Add body transformation based on goal
            switch (goal.ToLower())
            {
                case "weight loss":
                    sb.Append("slim and toned physique, visible muscle definition, athletic build, ");
                    sb.Append("reduced body fat, lean muscles, ");
                    break;
                case "muscle building":
                    sb.Append("muscular and well-defined physique, strong arms and legs, ");
                    sb.Append("visible muscle mass, defined abs, broad shoulders, ");
                    break;
                case "weight gain":
                    sb.Append("healthy and strong physique, well-proportioned body, ");
                    sb.Append("athletic build, ");
                    break;
                default:
                    sb.Append("fit and healthy physique, toned muscles, ");
                    break;
            }
            
            sb.Append("natural skin tone, realistic proportions, ");
            sb.Append("preserve original face features exactly, ");
            sb.Append("seamless body transformation, ");
            sb.Append("professional portrait photography, ");
            sb.Append("sharp focus, high detail, ");
            sb.Append("vibrant and alive appearance");
            
            return sb.ToString();
        }

        private async Task<string?> GenerateBodyImageAsync(string goal, string gender, int age, decimal weight, int height)
        {
            var apiKey = _configuration["StabilityAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("StabilityAI API key not configured");
                return null;
            }

            try
            {
                // Build prompt based on goal
                var prompt = BuildBodyImagePrompt(goal, gender, age, weight, height);

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var requestBody = new
                {
                    text_prompts = new[]
                    {
                        new { text = prompt, weight = 1.0 }
                    },
                    cfg_scale = 7,
                    height = 1024,
                    width = 1024,
                    steps = 30,
                    samples = 1
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Use Stability AI's text-to-image endpoint
                var response = await client.PostAsync(
                    "https://api.stability.ai/v1/generation/stable-diffusion-xl-1024-v1-0/text-to-image", 
                    content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Stability AI response received");
                    
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    // Stability AI returns base64 encoded images in an "artifacts" array
                    if (result.TryGetProperty("artifacts", out var artifacts) && artifacts.GetArrayLength() > 0)
                    {
                        var base64Image = artifacts[0].GetProperty("base64").GetString();
                        if (!string.IsNullOrEmpty(base64Image))
                        {
                            _logger.LogInformation("Base64 image received, saving to file");
                            // Convert base64 to image file and return the path
                            return await SaveBase64ImageAsync(base64Image);
                        }
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Stability AI API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating body image with Stability AI");
                return null;
            }
        }

        private async Task<string?> SaveBase64ImageAsync(string base64Image, string prefix = "stability")
        {
            try
            {
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "ai-photos");
                Directory.CreateDirectory(uploadsPath);
                
                var fileName = $"{prefix}_{Guid.NewGuid()}.png";
                var filePath = Path.Combine(uploadsPath, fileName);
                
                // Decode base64 to bytes
                var imageBytes = Convert.FromBase64String(base64Image);
                
                // Save to file
                await File.WriteAllBytesAsync(filePath, imageBytes);
                
                _logger.LogInformation("Image saved to: {FilePath}", filePath);
                
                // Return the URL path
                return $"/uploads/ai-photos/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving base64 image");
                return null;
            }
        }

        private string BuildBodyImagePrompt(string goal, string gender, int age, decimal weight, int height)
        {
            var sb = new StringBuilder();
            sb.Append("Animated, stylized, unreal engine style 3D render of a ");
            sb.Append(gender.ToLower());
            sb.Append(" person's full body, ");
            
            // Add body characteristics based on goal
            switch (goal.ToLower())
            {
                case "weight loss":
                    sb.Append("slim and fit physique, toned muscles, athletic build, ");
                    break;
                case "muscle building":
                    sb.Append("muscular and well-defined physique, strong arms and legs, ");
                    break;
                case "weight gain":
                    sb.Append("healthy and strong physique, well-proportioned body, ");
                    break;
                default:
                    sb.Append("fit and healthy physique, ");
                    break;
            }
            
            sb.Append("no face visible (face area should be transparent or blank), ");
            sb.Append("wearing athletic clothing, ");
            sb.Append("vibrant colors, anime/cartoon style, ");
            sb.Append("professional 3D render quality, ");
            sb.Append("full body portrait, standing pose, ");
            sb.Append("studio lighting, high detail");
            
            return sb.ToString();
        }

        private async Task<string?> DownloadImageAsync(string imageUrl, string saveDirectory)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var imageBytes = await client.GetByteArrayAsync(imageUrl);
                
                var fileName = $"body_{Guid.NewGuid()}.png";
                var filePath = Path.Combine(saveDirectory, fileName);
                
                await File.WriteAllBytesAsync(filePath, imageBytes);
                
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading body image");
                return null;
            }
        }

        private async Task<string?> ApplyProfessionalEnhancementsAsync(string imagePath, string outputDirectory)
        {
            try
            {
                using var image = await Image.LoadAsync(imagePath);

                // Apply professional enhancements to make it look alive and professional
                image.Mutate(ctx => ctx
                    // Enhance sharpness for professional look
                    .GaussianSharpen(0.5f)
                    // Slight brightness boost to make it pop
                    .Brightness(1.05f)
                    // Enhance contrast for depth
                    .Contrast(1.08f)
                    // Slight saturation boost for vibrant, alive look
                    .Saturate(1.1f)
                    // Add subtle clarity - slight blur then sharpen for professional look
                    .GaussianBlur(0.2f)
                    .GaussianSharpen(0.4f));

                // Save enhanced image
                var outputFileName = $"after_{Guid.NewGuid()}.png";
                var outputPath = Path.Combine(outputDirectory, outputFileName);
                
                await image.SaveAsync(outputPath, new PngEncoder());
                
                return outputPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying professional enhancements");
                return imagePath; // Return original if enhancement fails
            }
        }

        private async Task<string?> CompositeFaceOntoBodyAsync(string faceImagePath, string bodyImagePath, string outputDirectory)
        {
            try
            {
                using var faceImage = await Image.LoadAsync(faceImagePath);
                using var bodyImage = await Image.LoadAsync(bodyImagePath);

                // Resize body image to a standard size
                bodyImage.Mutate(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size(1024, 1024),
                    Mode = ResizeMode.Max
                }));

                // Resize face to fit on body (approximately 15% of body height)
                var faceSize = (int)(bodyImage.Height * 0.15);
                faceImage.Mutate(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size(faceSize, faceSize),
                    Mode = ResizeMode.Crop
                }));

                // Calculate position for face (upper center of body)
                var faceX = bodyImage.Width / 2 - faceSize / 2;
                var faceY = (int)(bodyImage.Height * 0.1); // 10% from top

                // Draw face onto body (we'll apply effects to blend it better)
                bodyImage.Mutate(ctx => ctx
                    .DrawImage(faceImage, new Point(faceX, faceY), 1f));

                // Apply effects to the entire image for cartoon/animation style
                bodyImage.Mutate(ctx => ctx
                    .GaussianBlur(0.3f) // Slight blur for cartoon effect
                    .Brightness(1.05f) // Slight brightness increase
                    .Contrast(1.05f) // Slight contrast increase
                    .Saturate(1.15f)); // Increase saturation for vibrant colors

                // Save final image
                var outputFileName = $"after_{Guid.NewGuid()}.png";
                var outputPath = Path.Combine(outputDirectory, outputFileName);
                
                await bodyImage.SaveAsync(outputPath, new PngEncoder());
                
                return outputPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error compositing face onto body");
                return null;
            }
        }
    }
}

