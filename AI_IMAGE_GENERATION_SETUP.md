# AI Image Generation Setup Guide

The AI transformation feature uses your uploaded face and generates an animated "after" body image. You can configure one of several AI image generation services.

## Supported AI Services

### Option 1: Stability AI (Recommended)
1. Sign up at https://platform.stability.ai/
2. Get your API key
3. Add to `appsettings.json`:
```json
{
  "StabilityAI": {
    "ApiKey": "your-stability-api-key-here"
  }
}
```

### Option 2: Replicate
1. Sign up at https://replicate.com/
2. Get your API token
3. Add to `appsettings.json`:
```json
{
  "Replicate": {
    "ApiKey": "your-replicate-token-here"
  }
}
```

### Option 3: OpenAI DALL-E
1. Use your existing OpenAI API key
2. Add to `appsettings.json`:
```json
{
  "OpenAI": {
    "ApiKey": "your-openai-api-key-here"
  }
}
```

## Fallback Mode

If no AI service is configured, the system will:
- Use a simple gradient-based body silhouette
- Still composite your face onto it
- Apply animation-style filters

## How It Works

1. **Face Extraction**: Automatically detects and extracts your face from the uploaded photo
2. **Body Generation**: Creates an animated, stylized body image based on your fitness goals
3. **Compositing**: Merges your face onto the generated body
4. **Animation Effects**: Applies filters to make it look unreal/animated:
   - Gaussian blur for cartoon effect
   - Enhanced brightness and contrast
   - Increased saturation for vibrant colors
   - Smooth shading

## Image Prompts

The system generates body images with prompts like:
- "Animated, stylized, unreal engine style 3D render"
- "Anime/cartoon style, vibrant colors"
- "Full body portrait, no face visible"
- Customized based on your fitness goal (weight loss, muscle building, etc.)

## Notes

- Face detection uses a simplified algorithm (assumes face in upper center)
- For production, consider using ML.NET Face API for better accuracy
- Generated images are saved in `wwwroot/uploads/ai-photos/`
- Images are automatically cleaned up (you may want to add cleanup logic)



