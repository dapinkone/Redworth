# Redworth

## Purpose


Redworth is a work-in-progress, GUI IRC client in C#. It uses my event-driven IRC protocol library, IRCHandler. My goal is for this to be a fully-useable, mature project over the next few months(as of Sept 2021). 

## Current Features:

* GUI does work. Connection to ZNC seems to work fine.
* Displays incoming privmsgs
- IRC functions:
	- Joins channels on /join #chan; updates gui accordingly.

## Planned Features:
### IRC functions:
- Part, Part Messages.
- Quit, Quit messages.
- Support for /me
- Nick change/display of current nick
- notices
- ctcp, both outgoing and replies
- user/channel modes.
	- Display thereof on GUI 
	- /mode command, update/storage of internal state.
- Support for multiple connections
- Support for multiple channel/topic/userlist "buffers"

### GUI flavor:
- channel /list
- nicklist on right side of gui
	- option to move nicklist
- Menus to toggle display of various UI elements.
- Properly anchor UI elements to resize with window properly.
- Menus/options for color customization.
- Nick colorizing.
- Ability to swap channels via swapping selection on channel list.
- Ability to leave channel/server via right-click menu.

###	Other:
- "Secrets" file
- Local database/config file for settings storage
- log storage
- timestamps