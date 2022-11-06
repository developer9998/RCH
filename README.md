# Room Code Hider
Allows you to customise the Gorilla Tag scoreboard header text to your liking - useful for hiding or revealing codes.
The initial purpose of this mod was to allow streamers to hide codes in public lobbies, or to show codes in privates to allow people to join.

### How the mod works:
When the scoreboard text changes, instead of using the game's text, my mod overrides the text to whatever you set it to. The currently used header can be changed on the Computer Interface on its own page. On this page you can also toggle whether the mod will override the scoreboard or not. While the mod relies on Computer Interface to allow the user to change the text, the mod does not require it to run. However, without Computer Interface, you can't change the custom header in game and the scoreboard text will stay as the first line in `Custom Headers.txt`.

### Customising your headers:
Once the mod has run once, a file called `Custom Headers.txt` will appear in the same folder as `RCH.dll`. Each line in the file is a custom header that you can change. You can have as many as you like. The text surrounded by curly braces is dynamic text. The next paragraph explains how it works and how to use it.

### Dynamic text:
Dynamic text is a feature which allows you to include some values into your custom header. They are all wrapped in curly braces. Instead of displaying the input, the output will be displayed on the scoreboard.

| Input | Output | Examples |
| ------------- | ------------- | ------------- |
| `{name}` | Displays the current Room ID | 5VA9, MONKE |
| `{region}` | Displays the lobby region | US, EU |
| `{mode}` | Displays the current gamemode | INFECTION, CASUAL |
| `{public}` | Displays if the lobby is public/private | -PUBLIC-, -PRIVATE- |
| `{count}` | Displays the lobby's players | 2, 4, 6, 10 |
| `{max}` | Displays the lobby player limit | 10 |
| `{ping}` | Displays the lobby's ping | 40, 50 |
| `{pubname}` | Displays the current public Room ID | 98IE, -PRIVATE- |

### Disclaimer
This product is not affiliated with Gorilla Tag or Another Axiom LLC and is not endorsed or otherwise sponsored by Another Axiom LLC. Portions of the materials contained herein are property of Another Axiom LLC. ©2021 Another Axiom LLC.
