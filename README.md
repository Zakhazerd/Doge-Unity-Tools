A small collection of tools for easier vrc avatar creation

Heirchy Appending:
This takes the typed in string and adds it to the heirchy of an animation clip.

Example:
Animation clip heirchy "Gameobject/particle"
String to append "Collection"
Result "Collection/Gameobject/particle"

Useful if you have an animation clip effecting many objects and you move change their heirchy.


Folder Copy:
This takes the selected folder in the tool and copies the contents to the current folder open in the project tab and appends the name of the current folder to them.


Normalize and set anchor:
This will unify your bounding boxes to the same size aswell as set the anchor override to all be the same.

Anchor is any transform in your hierchy if left blank will auto to hips.
Avatar put the top most object of your avatar the first children will all be checked for skinned meshes
NOTE anthing above armature will not have their anchor override set if it left armature. Anthing below will be set to hips.
