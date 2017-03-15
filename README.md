# WindowsDock

Mac OS X like application launcher.

![Preview](http://www.neptuo.com/Content/Images/Projects/windows-dock-01.jpg)

### Build 1.2.22
  * Minor fix (that ugly border around shortcut on main panel vanished)

### Build 1.2.21 
  * Global hotkeys for shortcuts (for only in combination with Windows key)

### Build 1.1.20
  * Configuration for small bubbles on top of shortcuts on main panel
  * Basic setting for shortcut icon size

### Build 1.1.19
  * Taskbar height + posibility to use this as offset from bottom

### Build 1.1.18
  * Dockable WindowsDock, you can dock main panel to one edge of screen and make it always visible
  * Some minor fixes

### Build 1.0.17
  * Colors for application buttons (Config+Close)
  * Posibility to hide them from main panel
  * Context menu on main panel as substitute from these buttons

### Build 1.0.16

  * KeyShortcut to show desktop in Explorer

### Build 1.0.15

  * Added support for pin to right and bottom edge of primary screen

### Build 1.0.14

  * Posibility to configure activation key (Win+selected key)
  * App restart after locale change nearly not needed (with exception for some resources - like datetime pattern)
  * Partial internal configuration changes

### Build 1.0.13

  * FIXED: Program doesn't crash when one instance is running and you try to run another (caused by binding global hotkey)
  * Minor internal changes

### Build 1.0.12

  * Translated items in configuration dropdown lists

### Build 1.0.11

  * FIXED: Restart not needed after position change (left,top)
  * Minor configuration changes

### Build 1.0.10

  * Add support for setuping border, thickness and color
  * Border radius can also be set from configuration window

### Build 1.0.9

  * Added possibility to change (or disable) application hotkeys (Win+W remains as application activation key, but yout can change T,S,B,D,X and Z keys to run extensions and etc.

### Build 1.0.8

  * Added possibility to place dock on left edge of primary screen
  * Added window align to screen edge + offset setup

### Build 1.0.7

  * Add support for localization, right now we support Czech and English, but you can easily add support for another language, simply open application installation folder, go to Resources a then copy one of files there and name it 'Resources' + underscore + language (and country) code, like 'Resources_cs.txt' or 'Resources_fr-FR.txt'. If you do so, please send to me localized file and will add it to the application

### Build 1.0.6

  * Space added as posible key to shortcut assignment
  * Some minor fixes

### Build 1.0.5

  * Fix of show/hide bug
  * Removed opacity (use alpha channel in background color
  * After start "autohide"

### Build 1.0.4

  * Some minor fixes

### Build 1.0.3

  * Hotkeys in Shortcuts edit
    * ENTER for opening shortcut detail (also DOUBLECLICK works)
    * DELETE for deleting shortcut
    * SHIFT+UP/DOWN for moving shortcut
  * Working directory can be set for shortcut+script (defaults to file directory)

### Build 1.0.2

  * Some minor fixes, released in 1.0.3

### Build 1.0.1

  * Hotkeys
    * Dock can be activated pressing *Win+W*
    * each shortcut can have hotkey (letters+num) to run that shortcut
    * shortcuts can share same hotkey to run multiple programs pressing single key
    * Extensions shortcuts
      * Browser - B
      * Desktop - D
      * Scripts - S
      * Textnotes - T
  * Configuration
    * buttons for manual saving/loading configuration
    * button for replacing current configuration
    * button for copying current configuration file
    * HiddenOffset - number of pixels that are always visible to top of display
  * Issues
    * 1 - solved
    * 2 - solved


### Build 1.0.0

## Program shortcuts

  * simply drag and drop any file on dock and get a shortcut
  * or open configuration window and manage shortcuts more advanced
  * not only program, but even any file can be pinned as shortcut

## Extensions

  * Textnotes & alarms
    * management for simple textnotes
    * support for assigning alarm for each textnote
  * Scripts
    * management for creating shortcuts for favourite scripts
  * Browser
    * simple folder browser
    * write C:\ and you will get all folders under C:\, then click on item or pres arrow up/down and enter to navigate to selected folder, then enter again for execute default command
    * browser has support for user defined commands (one can be selected as default - activated on enter press) - buttons on right side. In Confiration, section Commands can be these commands managed, in args field {0} is replaced by selected folder.
  * Desktop browser
    * this extension enables you to browse your items on desktop

## Rich configuration
  * Background color
  * Hide/Show duration
  * Opacity
  * Alarm sound

## Screens

### Build 1.0.12

![Build 1.0.12](http://i51.tinypic.com/2mot25u.png)

### Build 1.0.1

![Build 1.0.1](http://i55.tinypic.com/2mo5bh3.png)

### Build 1.0.0

![Build 1.0.0](http://i55.tinypic.com/9ap7pv.png)
