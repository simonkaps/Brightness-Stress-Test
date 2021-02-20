# Brightness Stress Test

![Brightness Stress Test application](BST.jpg?raw=true "Brightness Stress Test")

This test in certain people might cause epilepsia.
Therefore if you suspect you have epilepsia DO NOT run this program.

I wrote this program to minimally stress test laptops to check
how much battery time they have until shutdown.
It does this by setting brightness level from 100% to 1% and
vice versa from 1% to 100% while playing back
in repeat mode a video file that should be placed in current
directory as this program with the filename set
as video.mp4.

For playing back the video I use the well known
and trusted mplayer binary.
This video file can be any video your system supports,
I have also included a torrent file to a well known
funny animation that is 1080p Full HD.

To start the process fill for how many minutes it should run,
how many brightness steps to do every
1 second(1000ms) and press Start.

If you decide to stop at any time just press ESC key to exit the
fullscreen video and then press Stop button
to set brightness back to 100%.

To avoid potential problems the lowest brightness level will be at 1%.
More steps means more smooth changes but some systems might
not support this.

Every 300 seconds(5 minutes) this program will write in current
directory a file named log.txt a date - time per line so if
your notebook/laptop shutdowns immediately because of empty
battery you can measure an estimate of how much time your
battery lasted the next time you open Windows.

I'm assuming you have video file saved in programs current
directory named as video.mp4.
It can be any video ofcourse that your system can support.
I'm also assuming that your Windows have been configured to
never go to Sleep or Hibernation as this
will beat the purpose of such a test.
By using this program you agree that I'm not responsible
for any damage it may cause.

Written by Simon Kapsalis just for the LULz!
For more find me at simonkapsal@gmail.com

Written in C# .Net WinForms platform therefore
should be able to run on all Windows from
Windows 7 - 10 that have .NET Framework >= 4.5.1
Alongside this readme.md should be included
together the following files:

 - readme.md
 - BST.exe
 - mplayer.exe
 - bigbuckbunny_download.torrent
