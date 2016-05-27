# Audio File
_Digital Signal Processing research project_

This was a term-long school assignment, running during fall of 2015.  It was an opportunity to explore a wide range of concepts, many of which were completely new to me.  By the end of this project I had my first experiences in:
* Low-level Win32 multimedia functions
* The C# language
* Integrating C code with C# code
* The Windows clipboard

The concepts researched included:
* The discrete Fourier tranform (DFT)
* Filtering via DFT, Convolution
* Convolution and Filtering faster
* Windowing your filters and DFT (just for viewing)
* Finite and Infinite Impulse Response Filters and filtering in real-time!
* Time and pitch shifting


### Controls:

Mixer:
- Buttons to click for primary actions: New, Open, Save, Record, Stop, Play/Pause
- Menu items for changing sampling rate, bit depth, channels.  (note: only 8-bit and 16-bit are fully supported)
- Menu items for special effects (reverse, amplify, 

WaveForm:
- Can be resized, Time/Freq domain panels can be resized as well by dragging divider.
- Additional information about selection and wave file found in status bar at bottom.
- Select in time or freq domain with mouse -> click & drag
- Scroll mousewheel to zoom in/out in time domain
- Playback plays only current selection, or current selection to the end if a single point.
- Filter and Window by (first selecting, and then) right-clicking in the time domain. Select frequencies to curtail.
- Hotkeys:
	- &lt;spacebar&gt;: play
	- &lt;shift&gt; (home | end): expand or move the current selection marker to the start/end of the timeline (time domain)
	- &lt;delete&gt;: Delete selected frequencies in the frequency plot. (uses default filter: convolution)


### Assets:

#### Code Files:

| File       |  Description
|------------|------------------------------|
| Complex.cs | Complex numbers. Used in DFT |
| DSP.cs     | DSP algorithms, and effects. |
| FourierPanel.cs	| UI element, displays frequency domain. Also manages frequency selection. |
| Fraction.cs	    | For converting a double to a low-magnitude fraction. Used in resampling. |
| Mixer.cs        | Main UI window, has menu and buttons. Initiates most functionality. Controls, playback, but only via a WaveForm |
| Program.cs      | Main function. Starts Mixer. |
| WaveFile.cs     | Object that contains wave header and samples. Controls reading/writing. Has cut, copy, paste functions. |
| WaveForm.cs     | Main wave UI window. containes WavePanel and FourierPanel. Manages playback, user interaction, clipboard actions |
| WavePanel.cs    | Draws a sample set to screen. Allows interaction: zooming, scrolling, selecting. |
| WavePlayer.cs   | Spawns a thread to handle playback. All C#, using some win32 functions. |
| WaveRecorder.cs | Spawns a thread to handle recording. All C#, using some win32 functions. |
| WinmmHook.cs    | Definitions for WaveHeader and WaveFormat structs. Imports win32 function calls |

#### Images:
New, Open, Save, Play, Pause, Stop, Record.
Disabled versions of all but pause.
All in png format, created by Joe.
