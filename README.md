**Led and show list** 
* Groups can be created to contain leds

* Leds and Groups support drag and drop reordering

* Right click renaming and deleting of leds, groups and shows

* Right click to duplicate led(very helpful for similar leds) or show will copy properties of led or show to a new led or show

* Right click a show to export it to .lampshow file it will be created in the same folder alongside the .json lampshows

* Selecting led highlights it on playfield in blue and shows its properties


**Properties View** 
* Select image to use as playfield image (browse button)

* Scale playfield to adjust size of leds compared with playfield image. Use this first to match led size as close as possible and then fine tune individual leds

* Enable/disable visibility of playfield to more easily see leds

* Properties of led including: name, angle, shape, scale, single colour or RGB (RGB support coming in v0.2)

* Select colour of led

* Add an event to the selected led show by entering start and end frame and pressing add event


**Playfield View** 
* Resizes playfield image and leds to match available screen size

* Left click to select a led

* Drag a led to move it into position

* leds update their colour as show is played


**Timeline View** 
* Lists leds that are involved in the selected show

* Right click led to remove it from show

* Added events appear alongside corresponding led at appropriate frame intervals

* Play/Pause/Step and jump to functionality

* Right click to delete an event from the timeline (more timeline direct event editing to come in v0.2)


On first startup an example using Indy will load. It contains 2 example led shows and a bunch of leds.
It is best to have an individual folder for each playfield but the folder can be located anywhere. However a ledshows folder must reside in the same folder as the config file.