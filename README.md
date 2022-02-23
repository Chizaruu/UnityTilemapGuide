# UnityTilemapGuide
This guide will be periodically updated to support the new, old or frustrated users of Unity Tilemaps.

Current Unity Version used : 2020.3.23f1

## How to install Unity Tilemap Package
Download the 2D Tilemap Editor package via the Package Manager [[Unity Docs]](https://docs.unity3d.com/Manual/upm-ui.html)(in Unitys top menu: Window > Package Manager)

Unity does offer presets that automatically install the Tilemap package when creating a new Project.

Congratulations! You have unlocked the following features:
- Tile Palette [[Unity Docs]](https://docs.unity3d.com/Manual/Tilemap-Palette.html)(in Unity’s top menu: Window > 2D > Tile Palette)
- Grid [[Unity Docs]](https://docs.unity3d.com/Manual/class-Grid.html)
  - Rectangular (default)
  - Hexagonal
  - Isometric
- Tilemap [[Unity Docs]](https://docs.unity3d.com/Manual/class-Tilemap.html) (in Unity’s top menu: GameObject > 2D Object > Tilemap)
  - Rectangular
  - Hexagonal Point Top [[Unity Docs]](https://docs.unity3d.com/Manual/Tilemap-Hexagonal.html)
  - Hexagonal Flat Top [[Unity Docs]](https://docs.unity3d.com/Manual/Tilemap-Hexagonal.html)
  - Isometric
  - Isomentric Z as Y
- 2D Extras (Optional : in Unity’s top menu: Window > Package Manager > 2D Tilemap Editor > Samples)
  - Scriptable Brushes [[Unity Docs]](https://docs.unity3d.com/Manual/Tilemap-ScriptableBrushes.html)
    - GameObject Brush [[Unity Docs]](https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@1.8/manual/GameObjectBrush.html)
    - Group Brush [[Unity Docs]](https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@1.8/manual/GroupBrush.html)
    - Line Brush [[Unity Docs]](https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@1.8/manual/LineBrush.html)
    - Random Brush [[Unity Docs]](https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@1.8/manual/RandomBrush.html)
  - Scriptable Tiles [[Unity Docs]](https://docs.unity3d.com/Manual/Tilemap-ScriptableTiles.html)
    - Rule Tile [[Unity Docs]](https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@1.8/manual/RuleTile.html)
    - Animated Tile [[Unity Docs]](https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@1.8/manual/AnimatedTile.html)
    - Rule Override Tile [[Unity Docs]](https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@1.8/manual/RuleOverrideTile.html)

## How to use Unity Tilemaps
This guide is essentially split into three competency levels to better cater to learning levels. (Beginner, Intermediate and Advanced)Uni
### Beginner
All right, so you have no idea what you are doing with Tilemaps or are just getting started. 

It's time to learn the ABCs of the Unity Tilemap package.

Recommended things to do before you continue:
- It's recommended that you create a "Sprites" Folder within your Assets folder.
- When importing Sprites/Tilesets, it's a good idea to follow the standard 8x8, 16x16, 32x32, Etc texture sizes (Makes life a lot easier)
#### Setting up your Tile Sprites
Before we start to use the Tilemap Palette, we need to make sure that we have our sprites imported and set up for tile creation.

##### Rectangular Sprite Creation

###### Single Sprites
A single sprite indicates an image file that has been created solely for one Sprite.

The steps are as follows to set it them up correctly
1. Import/Drag your chosen Sprites into your Assets/Sprites folder.
2. Using *Shift+LMB* (or *Ctrl+LMB* if you like to cry), select all sprites within the folder.
3. In your Inspector do the following:
    - Set *Texture Type* to *Sprite (2D and UI)*. (Cause its going to be a sprite, duh)
    - Set *Sprite Mode* to *Single* (if it wasn't already)
    - Set *Pixels Per Unit* to your texture size (E.g 8(8x8), 16(16x16), 32(32x32), etc)
    - Set *Filter Mode* to *Point*
    - Set *Max Size* to your texture size (E.g 8(8x8), 16(16x16), 32(32x32), etc)
4. Click *Apply* in the inspector

###### Sprite Tilesets
Sprite Tilesets indicates an image file that holds many textures that separate into many different sprites.
(It's a tileset full of textures.)

The steps are as follows to set it them up correctly
1. Import/Drag your chosen tilesets into your Assets/Sprites folder.
2. Using *Shift+LMB* (or *Ctrl+LMB* if you like to cry), select all tilesets within the folder.
3. In your Inspector do the following:
    - Set *Texture Type* to *Sprite (2D and UI)*. (Cause it's going to be a sprite, duh)
    - Set *Sprite Mode* to *Multiple*
    - Set *Pixels Per Unit* to your texture size (8(8x8), 16(16x16), 32(32x32), Etc)
    - Set *Filter Mode* to *Point*
4. Click the Sprite Editor Button and do the following:
    - Click the Top Menu's *Slice*
    - Set Type to *Grid By Cell* Size (Leave it at Automatic at your peril)
    - Set *Pixel Size* to your texture size (E.g 8(8x8), 16(16x16), 32(32x32), etc)
    - Click the Slice Menu's *Slice*
    - Select tiles individually and rename them to whatever you want
    - Click *Apply*
    - Close the Sprite Editor
5. Loop step 4 until every tileset has been sliced and prepared for use.
6. Click *Apply* in the inspector

Your tile sprites are now ready for the Tilemap Palette.
#### Tilemap Palette 
The Tilemap Palette essentially makes you an artist so let's make sure you understand it.

##### A Certain Set of Tools
The Palette Toolkit is pretty straightforward with seven different tool options that are the following:
1. Select Tool (S) [[Unity Docs]](https://docs.unity3d.com/Manual/Tilemap-Painting.html#Select) - Used to inspect tile properties.
2. Move Tool (M) [[Unity Docs]](https://docs.unity3d.com/Manual/Tilemap-Painting.html#Move) - Used to move selected tile/s to another tilemap position. 
3. Paintbrush Tool (B) [[Unity Docs]](https://docs.unity3d.com/Manual/Tilemap-Painting.html#Brush) - Used to both select & paint palette tiles.
Useful tips:
    - You can rotate the selected tile clockwise by pressing [ [ ]. 
    - You can rotate the selected tile counter-clockwise by pressing [ ] ].
    - You can flip the selected tile along its x-axis by pressing [Shift + [ ].
    - You can flip the selected tile along its y-axis by pressing [Shift + ] ].
4. Box Fill Tool (U) [[Unity Docs]](https://docs.unity3d.com/Manual/Tilemap-Painting.html#Rec) - Used to both select & paint palette tiles in a rectangular area. 
5. Picker Tool (I)[[Unity Docs]](https://docs.unity3d.com/Manual/Tilemap-Painting.html#Picker) - Used to select either palette or tilemap tiles before automatically changing to the Paintbrush tool.
6. Eraser Tool (D)[[Unity Docs]](https://docs.unity3d.com/Manual/Tilemap-Painting.html#Eraser) - Used to erase tiles, the area of which you can adjust within the Tilemap Palette by using the Eraser Tool to hold and drag to the desired size. (Warning! Make sure Edit is disabled if working on a Tilemap.)
7. Flood Fill Tool (G)[[Unity Docs]](https://docs.unity3d.com/Manual/Tilemap-Painting.html#Fill) - This is used to either fill empty tile cells of identical tiles with a selected tile.

Ignore the Active Brush in the bottom section. (This is to be explained later in the Advanced (Scriptable Brushes) Section).

##### Creating a Palette
TBC

#### Tilemap Creation
##### Rectangular Tilemap
TBC
##### Hexagonal Tilemap
TBC
##### Isometric Tilemap
TBC

##### Tilemap Safety
It's *IMPORTANT* that you check that you are working on the correct tilemap at all times lest you suffer from the same frustration I've felt while working on projects.

You can check that you are working on the correct tilemap by checking
the Active Tilemap that's located in the Tilemap Palette window, just below the Palette's tools.

Another *IMPORTANT* tip to keep in mind is that you should use the Focus On feature when working on multiple tilemaps as it helps to avoid confusion and clutter.

You can do this by using the following steps:
1. Select the Tilemap you wish to work on by either using the Active Tilemap dropdown or from the Hierarchy window.
2. Select *Tilemap* on the Focus On the dropdown menu, located bottom right of the Scene View
### Intermediate
You have some knowledge of how to use Tilemaps or like to take risks. Let's help you take it up a notch.

Recommended things to do before you continue:
- Create a "Resources" folder and place the "Sprites" folder within it
#### Custom Tile Class
TBC
#### Tilemap Collider
TBC
#### Saving and Loading Tilemap
TBC

Save/Load Unity Tilemaps with JSON! 

This makes use of Odin Serializer (https://github.com/TeamSirenix/odin-serializer)

Open SerializationDemo in Unity Editor to test.
#### Field of View
TBC

This allows you to make create/modify Tilemaps as you wish and not worry about how Field of View will be affected.

This uses the ShadowCast algorithm provided by Adam Milazzo (http://www.adammil.net/blog/v125_Roguelike_Vision_Algorithms.html#shadowcode)

Open FieldOfViewDemo in Unity Editor to test.

#### Level Editor
TBC

This allows you to paint/erase tiles on Tilemaps during runtime.

Open LevelEditorDemo in Unity Editor to test.
### Advanced
You are an absolute whiz or feel that you are ready to have some fun. (or at least cry from frustration)
#### Scriptable Tiles
TBC
#### Scriptable Brushes
TBC

