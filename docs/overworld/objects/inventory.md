# `<CYF>` The Inventory Overworld object

Note: This Inventory object is not the same as the one used in battle. This one is specific
to the overworld.

### `Inventory.SetWeapon(string weapon)`

This function is an alias of `Player.SetWeapon`. Makes the player equip the given weapon, but
only if it's in their inventory.

### `Inventory.SetArmor(string armor)`

This function is an alias of `Player.SetArmor`. Makes the player equip the given armor, but
only if it's in their inventory.

### `Inventory.AddItem(string name)`

Adds the given item to the player's inventory.

### `Inventory.RemoveItem(number id)`

Removes the item number `id` from the inventory.

### `Inventory.IsItemInTheInventory(string name)` returns **boolean**

If the inventory contains this item, this will be true. It will be false otherwise.

### `Inventory.ItemExists(string name)` returns **boolean**

If this item exists in the engine's item list, meaning you created or defined it already,
this will be true. It will be false otherwise.

### `Inventory.GetItemID(string name)` returns **number**

Gets the id of the first item with the same name as `name`. If the item couldn't be found,
this returns -1 instead.

### `Inventory.GetItemCount()` returns **number**

Returns the number of items the player has in their inventory.

### `Inventory.SpawnBoxMenu()`

This function opens Undertale's "box" menu, the chest that appears throughout the game with
10 slots.

All items within this box are saved to CYF's save file upon using `General.Save`.
