# EightSeneca Assignment

This solution includes two applications: a WPF system tray app and a registry command-line tool.

## Projects

### 1. Registry Tool (EightSeneca.RegistryTool)

A command-line utility to manage registry settings for the WPF app.

**Commands:**
- `create` - Create registry keys with default values
- `list` - List all keys and values
- `change <key> <value>` - Change a key's value
- `reset <key>` - Reset a key to default
- `remove` - Remove all registry keys

**Example Usage:**
```cmd
EightSeneca.RegistryTool create
EightSeneca.RegistryTool change BrowserUrl "https://example.com"
EightSeneca.RegistryTool list
```

### 2. WPF Application (EightSeneca.WpfApp)
A system tray application that displays a fullscreen window with a web browser and video players.

**Features:**

- Starts as system tray icon with context menu (Show/Hide, Close)

- Double-click tray icon to show/hide window

- Fullscreen window (no borders) with 70% web browser and 30% video players

- Alt+Q to hide window

- Supports two web engines (WebView2 and CefSharp) configurable via registry

- Three video players (using LibVLC) in the right panel

- Registry settings automatically monitored and applied

- Registry Settings (HKEY_CURRENT_USER\Software\EightSeneca):

- BrowserUrl: URL for the web browser

- WebEngine: "WebView2" or "CefSharp"

- EnableZoom: "true" or "false"

- EnableTouch: "true" or "false"

- AllowExternalLinks: "true" or "false"

- Video1Path, Video2Path, Video3Path: Paths to video files

**How to Run**
- Build the solution in x64.

- Run Registry Tool first to create registry settings: EightSeneca.RegistryTool create

- Run WPF App: EightSeneca.WpfApp.exe

- Use the system tray icon to control the app.

**Notes**
- The app is designed for FullHD (1920x1080) resolution.

- Videos must be in MP4 format (H.264/H.265) and stored locally.

- If CefSharp fails to initialize, WebView2 will be used as fallback.

- CefSharp has a little bit issue when cannot display content at this moment although the URL is valid.

# Media Engine Selection: Why LibVLC?

## Decision Summary

**Selected Engine:** LibVLCSharp (LibVLC wrapper for .NET)

**Version:** 3.9.0 with VideoLAN.LibVLC.Windows 3.0.21

---

## Rationale

### ✅ Why LibVLC Was Chosen

#### 1. **Mature & Battle-Tested**
- Based on VLC Media Player (20+ years development)
- Used by millions of users worldwide
- Proven stability and reliability in production environments
- Extensive real-world testing and bug fixes

#### 2. **Native Codec Support**
- **Built-in H.264 and H.265 decoders** - no external codec packs needed
- No dependency on Windows Media Foundation codecs
- Consistent playback across different Windows versions
- Zero configuration required for supported formats

#### 3. **Excellent WPF Integration**
- Dedicated **LibVLCSharp.WPF** package with native `VideoView` control
- Direct integration with WPF visual tree
- Hardware-accelerated rendering support
- No manual frame rendering or bitmap conversion required

#### 4. **Superior Looping Performance**
- Built-in `EndReached` event for seamless loops
- No visible gaps between loop iterations
- Reliable loop mechanism for 24/7 operation
- Perfect for digital signage use cases

#### 5. **Resource Efficiency**
- Hardware-accelerated video decoding (GPU offload)
- Efficient memory management
- Low CPU usage during playback
- Can handle multiple simultaneous video streams

#### 6. **Cross-Platform Compatibility**
- Same API works on Windows, Linux, macOS
- Future-proof if cross-platform support needed
- Consistent behavior across platforms

#### 7. **Free & Open Source**
- LGPL 2.1 license (free for commercial use)
- No licensing costs
- Active community support
- Regular updates and security patches

---

## ❌ Why NOT FFmpeg

### Challenges with FFmpeg Integration

#### 1. **Complex WPF Integration**
- No native WPF controls
- Requires manual frame extraction
- Must convert frames to WPF-compatible bitmaps
- Additional rendering overhead

#### 2. **More Code Required**
```csharp
// FFmpeg approach (pseudocode)
while (decoding)
{
    AVFrame* frame = av_read_frame(context);
    Bitmap bitmap = ConvertFrameToBitmap(frame);
    Dispatcher.Invoke(() => UpdateImageControl(bitmap));
    // Memory management, synchronization, etc.
}
```

#### 3. **Wrapper Library Dependency**
- Need FFmpeg.AutoGen or similar wrapper
- Additional layer of abstraction
- Potential compatibility issues
- Less straightforward API

#### 4. **No Built-in Loop Support**
- Must manually detect end-of-file
- Implement custom seek-to-beginning logic
- Handle state transitions
- More error-prone

---
