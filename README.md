# SquibTracker
SquibTracker is a customize-able tracker for games. It is flexible and works for both Randomizer and Bingo tracking needs.

Release 1.0 of SquibTracker comes with the Ultima 4 Randomizer tracker data and assets loaded as default. As the name implies, this program is used to help track your progress when playing Ultima 4 Randomizer (https://github.com/Gilmok/U4Rando).

Other games supported currenty are A Link to the Past Randomizer and Ultima 9 Bingo. The tracker can be edited to support any game simply by supplying new graphics and creating a new datafile for the tracker data.

## Controls - Ultima 4

Item Tracking
- Left mouse will toggle the state of the items between On and Off.
- Right mouse will always toggle them to the Off state.

Virtue Tracking
- Left mouse will cycle between the virtue states of Very Poorly, Poorly, Well, Very Well, Worthy, and Partial Avatar.
- Right mouse will toggle them to the Off / Unknown state.

## Settings

### Window Scale
The window can be scaled with the mouse however this value is not saved. If you want to have the window load at a desired size, adjust the **SCALEFACTOR** setting inside the **assets/data/settings.xml** file.

### Data Files
As this is a release of SquibTracker, other tracker data files written for SquibTracker will work with this release as well. You only need the data files and any assets associated with them and then to point to the correct datafile using the **DATAFILE** setting inside the **assets/data/settings.xml** file.

### Tracker Adjustments
If you wish to change the look of the images, simply edit the images in the **assets/images_ultima4** directory.
If you wish to adjust the text, layout, images, or functionality of the grid you can edit the tracker data in **assets/data/tracker_ultima4_randomizer_01**.

## Special Thanks
Special thanks to Gilmok for creating the Ultima 4 Randomizer for which this tracker was designed for, which can be found at https://github.com/Gilmok/U4Rando.

## Known Issues
- If using the mouse to rescale and not the scalefactor in the settings file, rescaling the internals of the window only works via adjusting the window width. Height is adjusted seperately but will not rescale the internals. To rescale drag left to right to get the correct size then drag up and down to fix the blank space.
