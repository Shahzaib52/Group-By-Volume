# Group By Volume

I ran into this problem where I had several game objects placed together, but they weren't grouped correctly.

Now, you can group objects using Unity's **"Create Empty Parent"** feature that creates a new game object and parents the selected game objects to it but this solution falls apart if game objects in question are linked to a prefab, in which case, it just refuses to group objects together because game objects that are linked to a prefab cannot be moved or parented to another game object for that matter.

So, I pieced together a small script that groups objects by volume.

**How To Use**

1. After downloading and importing the package into your Unity project, open the **"Group By Volume"** window by clicking **"Tools/FYG/Group By Volume"** and dock the window somewhere 
2. Select the game object that you want the smaller game objects to be grouped with
3. Click **"Group Overlapping Objects"** in the **"Group By Volume"** window

And just like that, you have saved yourself from the trouble of manually selecting the game objects and grouping them by hand.

**"Group By Volume"** script respects the prefabs and is completely non-destructive; it doesn't destroy the game objects it is grouping.

Instead, these objects are duplicated, and then these new instances are parented by the selected game object; the original game objects are disabled after duplication.

I have also implemented the undo/redo function, so it should work as expected.

Let me know what you think.

Happy Coding!!
