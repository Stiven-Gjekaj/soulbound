# The Inventory object

### The Inventory object [E/M/W]

Items can be deleted and the Inventory System has been simplified. Here are the functions
created to help you with items.

Note: All of Undertale's consumable items, weapons and armors are already implemented in
CYF. You can use them in your mod if you want to, all you need to do is:

- Add an item using its long name. You have to add an item with the name
  `"Butterscotch Pie"` if you want the Player to have one, not `"ButtsPie"`.
- Avoid running the `BattleDialog` function in `HandleCustomCommand` for these items.

This system extends to any item added to Create Your Frisk's internals.

Some of these functions can only be used in `HandleItem`. Examples are included at the
bottom of this page.

### **boolean** `Inventory.AddItem(string name, number index = 8)`

Attempts to add the item `name` to the player's inventory.

- `name` = The name of the item to add. Must have been set with
  `Inventory.AddCustomItems`.
- `index` = The position to place the item in, starting from 1. Will "push away" items if
  placed in the same position as one. If greater than the number of items in the
  inventory, the item will just be added to the end.

Can not make a player's inventory exceed 8 items.

Returns `true` if the item was successfully added, `false` otherwise.

### `Inventory.RemoveItem(number index)`

Removes the item in position `index` from the player's inventory.

- `index` = Index to remove the item from. The first item is position 1.

### **string** `Inventory.GetItem(number index)`

Returns the name of the item in the inventory at the given index.

- `index` = Index of the chosen item. The first item is number 1.

### **number** `Inventory.GetType(number index)`

Returns the type of the item in the inventory at the given index.

- `index` = Index of the chosen item. The first item is number 1.

Types:

- `0` = Consumable. Will be deleted upon use.
- `1` = Weapon. You will be able to equip this item as a weapon.
- `2` = Armor. You will be able to equip this item as armor.
- `3` = Special. This item will not be deleted upon use.

### `Inventory.SetItem(number index, string name)`

Sets the inventory item at `index` to the item `name`.

- `index` = Index to put the item in. The first item is position 1.
- `name` = Name of the item to put in the inventory.

### `Inventory.UseItem(number index, boolean silent)`

Uses the inventory item at index `index`. Will throw an error if you have no item at the
given index, so check if it exists before using this function.

- `index` = Index of the item to use, starting from 1.
- `silent` = Whether or not using the item will result in writing battle text.

### `Inventory.AddCustomItems({table of string} names, {table of number} types)`

If you want to add custom items, this has to be used before `SetInventory`. This adds all
items in `names` to the inventory, where each item matches up with a type in `types`. If
you don't do this, the engine will not recognise your newly created items.

Usage: `Inventory.AddCustomItems({"item1", "item2"}, {1, 0})`

- `names` = The names of your custom items.
- `types` = The item types of your custom items. This array must be same size as `names`.

Types:

- `0` = Consumable. Will be deleted upon use.
- `1` = Weapon. You will be able to equip this item as a weapon.
- `2` = Armor. You will be able to equip this item as armor.
- `3` = Special. This item will not be deleted upon use.

### `Inventory.SetInventory({table of string} names)`

Sets the player's inventory. To use custom items, this must be used after
`AddCustomItems`.

This function is used like this: `Inventory.SetInventory({"item1", "item2"})`

- `names` = The names of the items.

To empty the player's inventory, use `Inventory.SetInventory({})`.

### **number** `Inventory.ItemCount`

Returns the number of items the player has in their inventory. Read-only.

### **boolean** `Inventory.NoDelete`

In the encounter function `HandleItem` only, setting this to true will make the last used
item stay in the inventory.

### `Inventory.SetAmount(number amount)`

Used with Weapon and Armor items. If the item is a Weapon, this sets its ATK. If the item
is armor, this sets its DEF.

- `amount`: the amount of ATK or DEF the item will have.

## Examples

Example of a healing item:

```lua
function EncounterStarting()
    Inventory.AddCustomItems({"Starfait"}, {0})
    Inventory.SetInventory({"Starfait"})
end

function HandleItem(ItemID)
    if ItemID == "STARFAIT" then
        Player.Heal(14)
        BattleDialog({"You eat the Starfait.[w:10]\nYou recovered 14 HP!"})
    end
end
```

Example of a weapon:

```lua
function EncounterStarting()
    Inventory.AddCustomItems({"Shotgun"}, {1})
    Inventory.SetInventory({"Shotgun"})
end

function HandleItem(ItemID)
    if ItemID == "SHOTGUN" then
        Inventory.SetAmount(16777215)
        BattleDialog({"You equipped the Shotgun."})
    end
end
```

Example of armor:

```lua
function EncounterStarting()
    Inventory.AddCustomItems({"Shield"}, {2})
    Inventory.SetInventory({"Shield"})
end

function HandleItem(ItemID)
    if ItemID == "SHIELD" then
        Inventory.SetAmount(8)
        BattleDialog({"You equipped the rusty shield."})
    end
end
```

Example of a special item:

```lua
function EncounterStarting()
    Inventory.AddCustomItems({"Music Box"}, {3})
    Inventory.SetInventory({"Music Box"})
end

function HandleItem(ItemID)
    if ItemID == "MUSIC BOX" then
        BattleDialog({"[noskip]You wind up the music box.[w:10]\nA haunting melody fills the room.", "The enemy's DEF drops!"})
        enemies[1].SetVar("def", enemies[1].GetVar("def") / 2)
    end
end
```

Example of a persistent item:

```lua
function EncounterStarting()
    Inventory.AddCustomItems({"Test"}, {0})
    Inventory.SetInventory({"Test"})
end

function HandleItem(ItemID)
    if ItemID == "TEST" then
        Inventory.NoDelete = true
        BattleDialog({"This item won't be\rdeleted!"})
    end
end
```

Many items:

```lua
function EncounterStarting()
    Inventory.AddCustomItems({"Test", "Test2", "Shotgun", "Shield", "PsnPotion", "Life Roll", "Nothing"}, {0, 0, 1, 2, 0, 0, 3})
    Inventory.SetInventory({"Test", "Test2", "Shotgun", "Shield", "PsnPotion", "Life Roll", "Nothing"})
end

function HandleItem(ItemID)
    if (ItemID == "TEST") then
        Inventory.NoDelete = true
        BattleDialog({"This is a persistent item.[w:5]\nThe item will be in the\rinventory in the next turn!"})
    elseif (ItemID == "TEST2") then
        BattleDialog({"This is a normal item.[w:10]\nThe item will be gone\rby the next turn!"})
    elseif (ItemID == "SHOTGUN") then
        Inventory.SetAmount(16777215)
        BattleDialog({"You equipped the Shotgun."})
    elseif (ItemID == "SHIELD") then
        Inventory.SetAmount(30)
        AllowPlayerDef(true)
        BattleDialog({"You equipped the shield."})
    elseif (ItemID == "PSNPOTION") then
        BattleDialog({"You drank the Poison Potion.","[noskip][waitall:10]...[waitall:1][w:20]\rThat was a bad idea.[w:20][health:kill]"})
    elseif (ItemID == "LIFE ROLL") then
        BattleDialog({"Your HP goes to 1[waitall:10]...[waitall:1][health:1,set]now.[w:20]\nNow, byebye![w:20][health:-1, killable]"})
    elseif (ItemID == "NOTHING") then
        BattleDialog({"You use Nothing. [w:10]Did you really\rthink that something would\rhappen?"})
    end
end
```
