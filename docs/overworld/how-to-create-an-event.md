# `<CYF>` How to create an event in Unity

If you want to make a good map, creating events is something that you might want to get used
to. Making events is not complicated, though, you just need to know what to tweak.

- The first thing that you'll have to do is, of course, to open the map you want to add the
  event in.
- As you may have seen in the previous tutorial, Unity uses "prefabs". These are generic
  objects that can be used in different scenes across the project.

  Here, you'll want to go to the Project tab and go to `Assets/Resources/prefabs` to see
  them. The one that interests us here is the `Event1` prefab. Click and drag it to the
  GameObject list to create an instance of this prefab.

Now, as you can see, there are two different objects, both named "Event1".

You **absolutely need to** rename the parent GameObject to `"SpritePivot"` in order for the
event to be correctly layered.

This is a workaround used to correctly layer the sprites. The position (transform data) of
the event must be held by the parent GameObject in order for its layer to be correctly set.
You can modify the position of the child GameObject if you want to, and it'll move the layer
point accordingly.

[For more information about this workaround and how it works, go here.](https://forum.unity3d.com/threads/official-2d-sorting-feedback.376707/#post-3167142)

## Configuring the event

Now the event is ready. However, you'll want to modify it now. Keeping the object like this
is not recommended. What follows is a list of things that you can modify.

- `GameObject`:
  - `Name`: All events must have a different name, so find one that fits this event.
- `RectTransform`:
  - `PosX`, `PosY`: The position of the event.
  - `Anchors`: The anchors of the event. Must have the same value.
  - `Pivot`: Pivot of the event. It changes the object's position.
- `SpriteRenderer`:
  - `Sprite`: This is not what will be used to display the sprite. But, you can still use
    this to see if the event's sprite looks good and to modify its hitbox.

  Note: In order to make your Event appear in the same place in-game as in the editor, use
  Unity to change the "Pivot" property of the imported sprite image to "Bottom" instead of
  "Center". Otherwise, in the editor, you'll only be able to tell an Event's actual position
  by its hitbox and not its visual location.
- `BoxCollider`:
  - `Edit Collider`: Lets you modify the event's hitbox.
  - `Is Trigger`: Must be activated to allow Touch events.
  - `Offset`, `Size`: Same as the `Edit Collider` Button.
- `EventOW`:
  - `Script to Load`: Name of the event script to load when triggering this event. The event
    must be in the directory `MAPMOD/Lua/Events/`.
  - `Actual Page`: The first page of the event, and in-game, the current page of the event.
  - `Event Triggers`: This part MUST NOT be forgotten. It is the most important part of the
    event.

    In your event's Lua code, you will need to create `EventPage0`, followed by one function
    for each event page you want, following the pattern `"EventPage" + Page number`,
    starting at 0.

    For instance: `EventPage0`, followed by `EventPage1`, `EventPage2`, until you have all
    the functions you want.

    **`EventPage0`** is a special event that is **always automatically called** as soon as
    the map loads. So any custom events that you want to use the Event Trigger system below
    should be in `EventPage1` and after.

    Back in Unity, for each X value in the `Event Trigger` table, you need to give the Page
    number, and for each matching Y value, you'll need to input the trigger type of the
    event:

    - `0`: **Confirm Button**. Activates when the player presses `Z` on this event.
    - `1`: **Touch**. Activates when the player collides with this event's hitbox.

      Note: In order to have a Touch event, you must check **`Is Trigger`** in the event's
      `BoxCollider` component, and also use `Event.IgnoreCollision` in `EventPage0`. See
      [The Event object](objects/event.md).
    - `2`: **Automatic**. Activates instantly, as soon as the map loads.
    - `3`: **Parallel Process**. Also called a coroutine. Activates on every frame, identical
      to `Update` in Mods.
  - `Move Speed`: The speed of the event in px/frame, if you want to move the event.
- `AutoloadResourcesFromRegistry`: Used to select a "default" sprite for this Event. Think of
  it the same as how a sprite path is necessary to use `CreateSprite`.
  - `Sprite Path`: Path of the sprite from `MAPMOD/Sprites`.

Now you'll need to create your event's script. It needs to be located at
`MAPMOD/Lua/Events`.

## Notes

If you want to permanently disable an event in-game, set its event page to -1. It'll be
automatically deleted when you enter the map.

All other values must not be changed, unless you know what you're doing, except for some
self-explanatory values like `SpriteRenderer`'s `FlipX` and `FlipY`.
