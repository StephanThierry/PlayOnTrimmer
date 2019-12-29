# PlayOnTrimmer
DotNet Core Console. Trim start and end of mp4 files using ffmpeg 

## Prereq:  

Download and install DOTNET Core 2+:  
https://www.microsoft.com/net/learn/get-started/windows

Download ffmpeg:  
https://www.ffmpeg.org/download.html

Change the code in `Program.cs`:  
`public static string ffmpegLocation = @"C:\dev\ffmpeg\bin\ffmpeg.exe";`  to  
`public static string ffmpegLocation = [wherevery you installed the ffmpeg.exe];`

## First:
dotnet restore  

## Run:
dotnet run "c:\media\myshowtotrim" 10 78  

The above command will create a batch file "trim.bat" that when run creates an "ok\" folder inside the target folder and trims 10 sec from the start and 78 sec from the end of every .mp4 or .avi file in the folder.
