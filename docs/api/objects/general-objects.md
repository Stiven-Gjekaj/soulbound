# General object functions and the CYFObject object

Usually, in CYF, you handle given objects such as text objects, sprites and the like.

However, in certain situations, you may have to manipulate some elements which you can't
normally use, such as internal elements used by Unity.

The `CYFObject` object's goal is to be able to manipulate these objects, mostly by moving
the various elements of the battle's hierarchy. All of its functions and variables are
also usable for `text objects`, `sprites` and `bullets` unless said otherwise.

### **string** `object.name` (readonly)

The name of the object in Unity. Different from sprites' `spritename` variable.

### **number** `object.childIndex`

The position of this object in its parent's children list, between 1 and `childCount`. The
first child will be displayed under all other children, while the last child will be
displayed above all others.

For example, if an object has 5 children and this value is 2, then this object is its
parent's second child, and will appear above the parent's first child and below all of its
other children.

Setting this value will change the current object's child index, reordering it in its
parent's children list.

### **number** `object.childCount` (readonly)

The number of children this object has.

### `object.GetParent()` returns **object**

Returns the parent of this object. Can be either a `sprite`, a `bullet` or a `CYFObject`.

If you don't know in advance the type of this object's parent, you can use the function
`tostring` on the parent to know which kind of object it is. The values are as follows:

- Sprites will return `"LuaSpriteController"`,
- Bullets will return `"ProjectileController"`,
- CYFObjects will return `"LuaCYFObject"`.

### `object.SetParent(object otherObject)`

Parents `object` to `otherObject`.

This will make the original object move along with the object it's parented to.

You can parent any `sprite`, `bullet`, `CYFObject` or `text object` to `sprites`,
`bullets` and `CYFObjects`, although `text objects` cannot be the parent of another
object.

Text object letter sprites can *only* be parented to other Text object letter sprites.

### `object.GetChild(number childIndex)` returns **object**

Returns the child at index `childIndex` of this object. Can be either a `sprite`, a
`bullet`, a `text object` or a `CYFObject`.

If you don't know in advance the type of this object's child, you can use the function
`tostring` on the child to know which kind of object it is. The values are as follows:

- Sprites will return `"LuaSpriteController"`,
- Bullets will return `"ProjectileController"`,
- Text objects will return `"LuaTextManager"`,
- CYFObjects will return `"LuaCYFObject"`.

### `object.GetChildren()` returns **{table of object}**

Returns a table with all of this object's children. See `object.GetChild()` for more
information.
