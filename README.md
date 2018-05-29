# Tank Exporter

## A tool to export Tanks from World of Tanks.
<a><img src="https://i.imgur.com/wNEqrW4.png" title="source: imgur.com" /></a>

#### This is an ongoing project by me.. Coffee_

## Version 43:
Decals are now on the terrain. You can edit and save changes to them. The decals come from the game's pkg files and use PBR to shade them. I am leaving version 42 GitHub incase this version is unusable by anyone.

## Version 42:
Added Shadows and controls for it. You can set 1 of the 3 lights as the shadow light source. Added terrain and skydome. The shader editor now stops the rendering while active. Rendering can be turned back on, on the editors form.
The hide show grid lines now toggels between terrain and grid.
Added a preview of the shadow mapping texture and result. This is a debug tool only.

## Version 41:
Fixed PBR shading.. Added Cube environment map.
Added menu items related to the new shading.

## Version 40:
Fixed a issue with writing the surface normals X flipped

## Version 39:
Added a FBO to use with FXAA.
Implemented FXAA.
Improved PBR and lighting in general.

## Version 38:
I Finally got a working PBR shader. Better lighting!

## Version 37:
Fixed an excpetion being thrown when mousing out of the Texture View window.

### Version 36:
Added a bar graph during the extarction of large game files so TE doesn't look like its crashed.

### Version 35:
Exporting to the res_mods folder now has an option to create a Work Area and copy the AM_HD and AO_HD to that folder in PNG format. This saves having to do it maually. You may not need to edit the AO map depending on what your doing but most likely, if you are painting on the AM map, you will need to fix the alpha change in the AO map.
When using the Texture Viewer and Show UVs is on.. mousing over triangles on the texture now highlights them in the model view window. This will make it easier to figure out where the UVs are located on the tank. The method of mousing over the tank model still works as before and highlights the UV map where that triangle is mapped.


### Version 34:
Added type icons to the tank list.
Added Component load window to select which gun and turret that were found for the tank.
Fixed a minor bug in the shader for the tanks.
Updated the help HTML.

### Version 33:
I fixed a bug with the HD textures never actually being loaded or exported to the FBX folder. I'm amazed I didn't catch this eariler.

### Version 32:
I Fixed a bug reported by vontamar at GitHub.. Normals were being written with the X flipped! THANK YOU! Reporting bugs makes TE a better tool!

### Version 32:
I fixed a bug caused by new content pgk names that stop the loading of HD textures.

### Version 30:
I reworked how tanks are found in the PKG files.
As of 4/8/2018 TE is finding 549 tanks.

### Version 29:
Added code to load the Italy Tank line.
There is currently only one tank in this line.. A preimium.
This data is not in the WoT API site so I hard coded the name for the It13_Pregetto_M35_mod_46

### Verion 28:
Removed BSP and BSP Tree from the menu.. They are no longer part of primitive files.
Fixed coding to deal with part1 and part2 content files.
I have not added code to deal with the Italy line yet so these tanks are NOT loaded in to the list.
I removed writing BSP2 data to the prmitive files as these are now contained in VT files (I think... I need to do more exploring of these files formats).

### Version 27:
Added a Simple Lighting mode for those that are having problems with rendering the tanks.
If you are having issues with textures not showing up, please create a ticket explaining the issue and video hardware you are using

### Version 26:
Changed where the Terra tank list and data is stored because of UAC problems. Updated the tank list for Terra.

### Version 25:
This version fixes a memory leak. Updated the tank list for Terra.

### Version 24:
Tank Exporter now loads all tanks in the extended pkg files.

### Version 23:
Fixed a stupid bug in the path names to where the game and res_mods.
Added some code to find more of the missing tank parts.
WG has split the tier 8 tanks in to 2 pgk files. I will need to change the code to deal with this as some point. As it is, tanks from that 2nd pkg are ignore.

### Version 22:
Paths to folders are now saved to the wot_temp folder.
You won't need to reset the game and res_mods/currentversion
paths after every update to Tank Exporter.

### Version 21:
Added a shader while viewing the FBX.
A few minor big fixes with loading textures from the res_mods folder path.

### Version 20:
This fixes a path issue when texture paths reference the a old game version folder. It nows repaces that path with the current game version path to the res_mods folder.

### Version 19:
Added support for adding more than one one object to the turret or hull.

### Version 17:
Fixed a nasty issue with the normals of added items.
They were not being translated correctly.

### Version 18:
More fixes in FBX import/export

### Version 16:
Fixes in FBX import/expore

### Version 15:
Fixed a issue while writing Turret models.
Added a model info window.
Added / Fixed Visual viewer window.
Now TE seaches in the res_mods folder for a matching tank component. If it finds it, will load it from there.
Minor Bug fixes.

### Version 14:
Added Primitive Write capability.
Now you can write primitive files the game can load!
This only works will the hull or turret models.
Read the help file for more information.

### Version 13:
Added Mouse over picking of the vertex under the mouse.
This only works if UVs are visable in the Texture Viewer.
Also.. Pressing the "C" key will center the current UV in the Texture Viewer's window.
The purpose is to make it easy to locate where the UVs are mapped to.
Updated Help HTML and added a new page and image.

### Version 12:
Added Texture Viewing and UV ploting.. also a way to save the texture as a png. This is dependent on the view settings.
Added importing of the BSP2 and the tree from the tank models.

### Version 11:
Added FBX importing. Re did the User Interface. 

### Verions 10:
Now when exporting a tank in FBX, a folder is created under the tanks name in the same directory and all the textures are placed there.
