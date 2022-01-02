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

"Anchor" is any transform in your hierchy if left blank will auto to hips when avatar is added.
"Avatar" put the top most object of your avatar the first children will all be checked for skinned meshes.
If "Search for all meshes" is checked will search entire heirchy for for both skinned meshes and regular meshes.


Set Fx Layer Weights to 1:
Will take the animator from Fx Layer in the VRC Avatar Descriptor and set the weigts of all layers to 1.
Excludes Hai's GCE layers.


Quick Bool:
Takes the fx layer from vrc avatar descriptor adds a layer and paramater taken from "Parameter Name" and creates a simple bool back and forth.
Adds parameter to selected expression parameter menu and creates a new toggle control on selected expression menu.
All fields + Avatar must be filled in order to use.

Quick Bool Animation:
Creates 2 frame animation of selected game objects of the opposite of their current active state.
Places it in current open folder in Project.
All fields + Avatar must be filled in order to use.
