# hair-physics
Realistic simulation of hair physics optimized for virtual reality (120FPS). Leverages spring physics and geometry/compute shaders to create hair strands that respond to movement.

### How to run
1. Open the Build folder
2. Click on HairSimulator.exe
3. If it doesn't work, feel free to watch the video :) System requirements could make it laggy depending on the computer's graphics card

### Description
This simulator was a huge learning experience for me and turned out to be a great research assignment along with its success. My goal was to create hair that could be interacted with on a strand by strand level. This project was to be optimized for VR as it was a side-project of the NDI EM-VR project. 

Keeping up with the 90fps minimum requirement while using Unity game engine was the most difficult part of this project. Because Unity does a lot of things for you, it has a lot of overhead. Unity could only handle about 1000 springs before it became too laggy, and with their built in support for Rigid-bodies it was even slower. Thus, a custom physics body needed to be created along with custom spring physics. 

I did some research into this and managed to put together a working strand of springs. The spring calculations were then done on the compute shader, and over 500,000 points could be computed at once without affecting performance. (Only restriction at this point was Unity’s professional mode for access to more GPU cores) I knew I was on the right track but still drawing these points using the Unity Gizmos was slowing down performance. The next step was to instead create a strand geometrically based on the outputs from the compute shader. So I passed in the compute buffer into a series of vertex, geometry, and pixel shaders to generate a ‘noodle’ hair around each of the points, each frame.
