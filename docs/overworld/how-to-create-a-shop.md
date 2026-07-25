# `<CYF>` How to create a shop

You may know what a shop is in Undertale. Here you'll learn how to make one in Create Your
Frisk.

Shop scripts contain some of the same functions and objects that are in all scripts, such as
the `Audio` object, the `NewAudio` object, the `Inventory` object, the `Input` object, the
`Misc` object, and the `Time` object, as well as `RealGlobals` and `AlMightyGlobals`.

The variables `isCYF`, `safe`, `windows` and `CYFversion` are also accessible (see
[Overworld basics](basics.md)).

Finally, the functions `CreateSprite`, `CreateLayer` and `CreateText` are all accessible
(see [Overworld basics](basics.md) for differences in functionality), as well as the
[Game event](../api/game-events.md) `OnTextAdvance`.

**Caution: You will need to code in C# for some aspects of this feature.** Items are defined
in the engine's source. If you wish to use this feature, you will need to read
[Unity setup](../basics/unity-setup.md) to learn how to open this project in Unity, edit its
source code, and build it.

## Shop states

Shop scripts have their own State system. You can't directly access them, but here is a list
of the shop's states:

- `MENU` - The main menu of the Shop script.
- `BUY` - The Buy menu of the Shop script.
- `BUYCONFIRM` - State for confirming a purchase when buying an item.
- `SELL` - The Sell menu of the Shop script.
- `SELLCONFIRM` - State for confirming a sale when selling an item.
- `TALK` - The Talk menu of the Shop script.
- `TALKINPROGRESS` - Entered when a Talk option is chosen and its assigned text is displayed.
- `EXIT` - Entered while showing the exit text of the shop, just before leaving.
- `INTERRUPT` - Entered when interrupting the action of any other state. See the function
  `Interrupt()` for more details.

## Setting up your shop for use in the overworld

CYF's shops are different from what you've seen so far. Your shop scripts are to be put in
the directory `YOURMOD/Lua/Shops/`.

A shop can be loaded with this function:

### `General.EnterShop(string scriptName)`

Enters a shop scene from the overworld.

`scriptName` should be the name of your shop's lua file, without the extension. So, if you
want to load `YOURMOD/Lua/Shops/shop.lua`, you would use `General.EnterShop("shop")`.

## Shop variables

- **sprite** `background` - A sprite object used as the background of the shop. Optimally,
  this sprite should be 640x240 pixels.
- **string** `returnscene` - The name of the scene the Player will go back to when exiting
  the shop.
- **{table of number, number}** `returnpos` - A table containing the position the Player will
  appear at in the map when leaving the shop.
- **number** `returndir` - Direction the Player will face when returned to the overworld.
  Possible values:
  - `2`: Down
  - `4`: Left
  - `6`: Right
  - `8`: Up
- **string** `music` - Name of the music file the shop will use. The music file must be .ogg
  or .wav and must be in `YOURMOD/Audio`.
- **{table of {table of string}, {table of string}, {table of number}}** `buylist` - A table
  defining which items the shop will have in store. This table is composed of 3 other tables:
  - The first table contains the names of the items to be sold. The items must be in the item
    database (`Assets/Scripts/Inventory/Inventory.cs`).
  - The second table contains the descriptions for each item.
  - The third table contains the prices of each item. Use -1 to use the price set in the
    database (`Assets/Scripts/Inventory/Inventory.cs`). Use 0 to make the item display as
    "--- SOLD OUT ---" like in Undertale. The player will not be able to buy it.
- **{table of {table of string}, {table of {table of string}} or string}** `talklist` - A
  table containing the list of all the talk options in the shop. This table is composed of 2
  other tables:
  - The first table contains the name of each TALK option the Player can pick.
  - The second table contains the text associated with these TALK actions. These can each be
    a single string or a table of strings.

  Example setup:

  ```lua
  talklist = {
      { "Job", "Hobbies", "Threaten", "Sell?" },
      {
          { "Me?", "I'm just a shopkeeper." },
          "Just a shopkeeper.",
          { "Threats? I'm not impressed, I'm just a shopkeeper." },
          { "So you have items to sell?",
            "I guess some of them could be useful to me...",
            "Alright, I'll see what you have next time you want to sell something to me!" }
      }
  }
  ```
- **string** `maintalk` - The text displayed in the main Shop menu.
- **string** `buytalk` - The text displayed when entering the Buy menu.
- **string** `talktalk` - The text displayed when entering the Talk menu.
- **string** `exittalk` - The text displayed when entering the Exit menu.
- **boolean** `playerskipdocommand` - False by default. If this value is set to true, text
  commands will be called even if the player skips the text, except for `[w]` and `[letters]`
  commands, and commands with the tag "`skipover`".

## Shop events

These events work the same way as events work in-battle. See
[Game events](../api/game-events.md).

### `Start()`

This event is called when entering the shop. It is advised to set the background's image
here.

### `Update()`

Every well-functioning script needs an Update() function. This function is called once per
frame.

### `EnterMenu()`

Entered when entering the main Shop menu.

### `EnterBuy()`

Entered when entering the Buy menu.

### `SuccessBuy(string itemName)`

Entered when an item is successfully purchased. `itemName` is the name of the item bought.

### `FailBuy(string error)`

Entered when the Player tries to buy an item, but can't. `error` has three possible values:

- `"gold"` - If the Player doesn't have enough Gold.
- `"full"` - If the Player's inventory is full.
- `"soldout"` - If the shop is sold out of this item. See `buylist` for how to accomplish
  this.

### `ReturnBuy()`

Entered when the player selects an item to buy, then chooses "No" or presses the Cancel
button.

### `EnterSell()`

Entered when entering the Sell menu. To prevent the player from selling, use the function
`Interrupt()` here (see below).

### `SuccessSell(string itemName)`

Entered when an item is successfully sold. `itemName` is the name of the item sold.

### `FailSell(string error)`

Entered when failing to enter or use the Sell menu. `error` has two possible values:

- `"empty"` - If the Player's inventory is empty.
- `"cantsell"` - If the Player tries to sell an item, but it is unsellable. In order to
  accomplish this, set the item's sell price to 0 in the database
  (`Assets/Scripts/Inventory/Inventory.cs`).

### `ReturnSell()`

Entered when the player selects an item to sell, then chooses "No" or presses the Cancel
button.

### `EnterTalk()`

Entered when entering the Talk menu.

### `SuccessTalk(string talkOption)`

Entered when a Talk option is successfully selected. `talkOption` is the name of the option
selected.

### `EnterExit()`

Entered when choosing the Exit option.

### `Interrupt({table of string} text, string newState = "MENU")`

Interrupts any current state, except the `INTERRUPT` state, to display the message given in
`text`. At the end of the message, the state `newState` will be entered.

*It is advised not to use this to enter the states* `BUYCONFIRM`, `SELLCONFIRM` *or*
`TALKINPROGRESS`.

### `OnInterrupt(string newState)`

This function is called when entering the Interrupt state. `newState` is the name of the
state the Player will enter when the Interrupt state finishes its text.

## Additional info

The layers in the Shop scene aren't the same as everywhere else. This is the Shop script's
layering system:

- `"Bottom"`: Under everything, even the background.
- `"BelowUI"`: Above the background.
- `"BelowPlayer"`: Above the background and the UI.
- `"Top"`: Above everything.
